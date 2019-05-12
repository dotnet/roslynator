// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.Comparers;
using Roslynator.CSharp.Refactorings;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EnumDeclarationCodeFixProvider))]
    [Shared]
    public class EnumDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.AddNewLineBeforeEnumMember,
                    DiagnosticIdentifiers.SortEnumMembers,
                    DiagnosticIdentifiers.EnumShouldDeclareExplicitValues);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out EnumDeclarationSyntax enumDeclaration))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.AddNewLineBeforeEnumMember:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Add newline",
                                cancellationToken => AddNewLineBeforeEnumMemberRefactoring.RefactorAsync(context.Document, enumDeclaration, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.SortEnumMembers:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                $"Sort '{enumDeclaration.Identifier}' members",
                                cancellationToken => SortEnumMembersAsync(context.Document, enumDeclaration, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.EnumShouldDeclareExplicitValues:
                        {
                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            INamedTypeSymbol enumSymbol = semanticModel.GetDeclaredSymbol(enumDeclaration, context.CancellationToken);

                            EnumSymbolInfo enumInfo = EnumSymbolInfo.Create(enumSymbol);

                            ImmutableArray<ulong> values = enumInfo
                                .Fields
                                .Where(f => f.HasValue && ((EnumMemberDeclarationSyntax)f.Symbol.GetSyntax(context.CancellationToken)).EqualsValue != null)
                                .Select(f => f.Value)
                                .ToImmutableArray();

                            Optional<ulong> optional = FlagsUtility<ulong>.Instance.GetUniquePowerOfTwo(values);

                            if (!optional.HasValue
                                || !ConvertHelpers.CanConvert(optional.Value, enumSymbol.EnumUnderlyingType.SpecialType))
                            {
                                return;
                            }

                            CodeAction codeAction = CodeAction.Create(
                                "Declare explicit values",
                                ct => DeclareExplicitValueAsync(context.Document, enumDeclaration, enumSymbol, values, semanticModel, ct),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }

        private static async Task<Document> SortEnumMembersAsync(
            Document document,
            EnumDeclarationSyntax enumDeclaration,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            SpecialType enumSpecialType = semanticModel.GetDeclaredSymbol(enumDeclaration).EnumUnderlyingType.SpecialType;

            SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = enumDeclaration.Members;

            SeparatedSyntaxList<EnumMemberDeclarationSyntax> newMembers = members
                .OrderBy(f => GetConstantValue(f, semanticModel, cancellationToken), EnumValueComparer.GetInstance(enumSpecialType))
                .ToSeparatedSyntaxList();

            if (AreSeparatedWithEmptyLine(members))
            {
                for (int i = 0; i < newMembers.Count; i++)
                {
                    newMembers = newMembers.ReplaceAt(i, newMembers[i].TrimLeadingTrivia());
                }

                for (int i = 0; i < newMembers.Count - 1; i++)
                {
                    SyntaxToken separator = newMembers.GetSeparator(i);

                    newMembers = newMembers.ReplaceSeparator(
                        separator,
                        separator.TrimTrailingTrivia().AppendToTrailingTrivia(new SyntaxTrivia[] { NewLine(), NewLine() }));
                }
            }

            if (newMembers.SeparatorCount == members.SeparatorCount - 1)
            {
                SyntaxNodeOrTokenList newMembersWithSeparators = newMembers.GetWithSeparators();

                newMembersWithSeparators = newMembersWithSeparators.Add(CommaToken());

                newMembers = newMembersWithSeparators.ToSeparatedSyntaxList<EnumMemberDeclarationSyntax>();
            }

            MemberDeclarationSyntax newNode = enumDeclaration
                .WithMembers(newMembers)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(enumDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }

        private static bool AreSeparatedWithEmptyLine(SeparatedSyntaxList<EnumMemberDeclarationSyntax> members)
        {
            int count = members.Count;

            if (members.SeparatorCount < count - 1)
                return false;

            for (int i = 1; i < count; i++)
            {
                if (!members[i].GetLeadingTrivia().Any(SyntaxKind.EndOfLineTrivia))
                    return false;
            }

            for (int i = 0; i < count - 1; i++)
            {
                if (!members.GetSeparator(i).TrailingTrivia.Any(SyntaxKind.EndOfLineTrivia))
                    return false;
            }

            return true;
        }

        private static object GetConstantValue(
            EnumMemberDeclarationSyntax enumMemberDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            return semanticModel.GetDeclaredSymbol(enumMemberDeclaration, cancellationToken)?.ConstantValue;
        }

        private static async Task<Document> DeclareExplicitValueAsync(
            Document document,
            EnumDeclarationSyntax enumDeclaration,
            INamedTypeSymbol enumSymbol,
            ImmutableArray<ulong> values,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            bool isFlags = enumSymbol.HasAttribute(MetadataNames.System_FlagsAttribute);

            List<ulong> reservedValues = values.ToList();

            SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = enumDeclaration.Members;

            SeparatedSyntaxList<EnumMemberDeclarationSyntax> newMembers = members;

            for (int i = 0; i < members.Count; i++)
            {
                if (members[i].EqualsValue == null)
                {
                    IFieldSymbol fieldSymbol = semanticModel.GetDeclaredSymbol(members[i], cancellationToken);

                    ulong? value = null;

                    if (isFlags)
                    {
                        Optional<ulong> optional = FlagsUtility<ulong>.Instance.GetUniquePowerOfTwo(reservedValues);

                        if (optional.HasValue
                            && ConvertHelpers.CanConvert(optional.Value, enumSymbol.EnumUnderlyingType.SpecialType))
                        {
                            value = optional.Value;
                        }
                    }
                    else
                    {
                        value = SymbolUtility.GetEnumValueAsUInt64(fieldSymbol.ConstantValue, enumSymbol);
                    }

                    if (value != null)
                    {
                        reservedValues.Add(value.Value);

                        EqualsValueClauseSyntax equalsValue = EqualsValueClause(NumericLiteralExpression(value.Value, enumSymbol.EnumUnderlyingType.SpecialType));

                        EnumMemberDeclarationSyntax newMember = members[i].WithEqualsValue(equalsValue);

                        newMembers = newMembers.ReplaceAt(i, newMember);
                    }
                }
            }

            EnumDeclarationSyntax newEnumDeclaration = enumDeclaration.WithMembers(newMembers);

            return await document.ReplaceNodeAsync(enumDeclaration, newEnumDeclaration, cancellationToken).ConfigureAwait(false);
        }
    }
}
