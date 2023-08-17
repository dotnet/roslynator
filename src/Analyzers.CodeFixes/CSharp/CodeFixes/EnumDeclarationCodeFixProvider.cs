// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
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
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EnumDeclarationCodeFixProvider))]
[Shared]
public sealed class EnumDeclarationCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get
        {
            return ImmutableArray.Create(
                DiagnosticIdentifiers.SortEnumMembers,
                DiagnosticIdentifiers.EnumShouldDeclareExplicitValues,
                DiagnosticIdentifiers.UseBitShiftOperator);
        }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(root, context.Span, out EnumDeclarationSyntax enumDeclaration))
            return;

        Document document = context.Document;

        foreach (Diagnostic diagnostic in context.Diagnostics)
        {
            switch (diagnostic.Id)
            {
                case DiagnosticIdentifiers.SortEnumMembers:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            $"Sort '{enumDeclaration.Identifier}' members",
                            ct => SortEnumMembersAsync(document, enumDeclaration, ct),
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
                            .Where(f => f.HasValue && ((EnumMemberDeclarationSyntax)f.Symbol.GetSyntax(context.CancellationToken)).EqualsValue is not null)
                            .Select(f => f.Value)
                            .ToImmutableArray();

                        Optional<ulong> optional = FlagsUtility<ulong>.Instance.GetUniquePowerOfTwo(values);

                        if (!optional.HasValue
                            || !ConvertHelpers.CanConvertFromUInt64(optional.Value, enumSymbol.EnumUnderlyingType.SpecialType))
                        {
                            return;
                        }

                        bool isFlags = enumSymbol.HasAttribute(MetadataNames.System_FlagsAttribute);

                        CodeAction codeAction = CodeAction.Create(
                            "Declare explicit values",
                            ct => DeclareExplicitValueAsync(document, enumDeclaration, enumSymbol, isFlags, useBitShift: false, values, semanticModel, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);

                        if (isFlags)
                        {
                            CodeAction codeAction2 = CodeAction.Create(
                                "Declare explicit values (and use '<<' operator)",
                                ct => DeclareExplicitValueAsync(document, enumDeclaration, enumSymbol, isFlags, useBitShift: true, values, semanticModel, ct),
                                GetEquivalenceKey(diagnostic, "BitShift"));

                            context.RegisterCodeFix(codeAction2, diagnostic);
                        }

                        break;
                    }
                case DiagnosticIdentifiers.UseBitShiftOperator:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            "Use '<<' operator",
                            ct => UseBitShiftOperatorAsync(document, enumDeclaration, ct),
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
        SpecialType enumSpecialType = semanticModel.GetDeclaredSymbol(enumDeclaration, cancellationToken).EnumUnderlyingType.SpecialType;
        SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = enumDeclaration.Members;

        List<EnumMemberDeclarationSyntax> sortedList = members
            .OrderBy(f => GetConstantValue(f, semanticModel, cancellationToken), EnumValueComparer.GetInstance(enumSpecialType))
            .ToList();

        bool hasTrailingSeparator = members.HasTrailingSeparator();
        int lastIndex = sortedList.Count - 1;

        if (!hasTrailingSeparator)
        {
            int index = members.IndexOf(sortedList.Last());

            SyntaxTriviaList trailingTrivia = (index == members.Count - 1)
                ? members[index].GetTrailingTrivia()
                : members.GetSeparator(index).TrailingTrivia;

            sortedList[sortedList.Count - 1] = sortedList[sortedList.Count - 1].WithTrailingTrivia(trailingTrivia);
            lastIndex--;
        }

        List<SyntaxNodeOrToken> sortedMembers = sortedList.ConvertAll(member => (SyntaxNodeOrToken)member);

        for (int i = lastIndex; i >= 0; i--)
        {
            int oldIndex = members.IndexOf((EnumMemberDeclarationSyntax)sortedMembers[i]);

            if (oldIndex == 0
                && members[i].GetLeadingTrivia().FirstOrDefault().IsEndOfLineTrivia())
            {
                sortedMembers[i] = sortedMembers[i].WithLeadingTrivia(sortedMembers[i].GetLeadingTrivia().Insert(0, NewLine()));
            }

            SyntaxTriviaList trailingTrivia;
            if (!hasTrailingSeparator
                && oldIndex == members.Count - 1)
            {
                trailingTrivia = members.Last().GetTrailingTrivia();
                sortedMembers[i] = sortedMembers[i].WithoutTrailingTrivia();
            }
            else
            {
                trailingTrivia = members.GetSeparator(oldIndex).TrailingTrivia;
            }

            sortedMembers.Insert(i + 1, Token(SyntaxKind.CommaToken).WithTrailingTrivia(trailingTrivia));
        }

        SyntaxTriviaList leadingTrivia = sortedMembers[0].GetLeadingTrivia();

        if (leadingTrivia.FirstOrDefault().IsEndOfLineTrivia())
            sortedMembers[0] = sortedMembers[0].WithLeadingTrivia(leadingTrivia.RemoveAt(0));

        MemberDeclarationSyntax newEnumDeclaration = enumDeclaration
            .WithMembers(sortedMembers.ToSeparatedSyntaxList<EnumMemberDeclarationSyntax>())
            .WithFormatterAnnotation();

        return await document.ReplaceNodeAsync(enumDeclaration, newEnumDeclaration, cancellationToken).ConfigureAwait(false);
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
        bool isFlags,
        bool useBitShift,
        ImmutableArray<ulong> values,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        List<ulong> reservedValues = values.ToList();

        SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = enumDeclaration.Members;

        SeparatedSyntaxList<EnumMemberDeclarationSyntax> newMembers = members;

        for (int i = 0; i < members.Count; i++)
        {
            if (members[i].EqualsValue is null)
            {
                IFieldSymbol fieldSymbol = semanticModel.GetDeclaredSymbol(members[i], cancellationToken);

                ulong? value = null;

                if (isFlags)
                {
                    Optional<ulong> optional = FlagsUtility<ulong>.Instance.GetUniquePowerOfTwo(reservedValues);

                    if (optional.HasValue
                        && ConvertHelpers.CanConvertFromUInt64(optional.Value, enumSymbol.EnumUnderlyingType.SpecialType))
                    {
                        value = optional.Value;
                    }
                }
                else
                {
                    value = SymbolUtility.GetEnumValueAsUInt64(fieldSymbol.ConstantValue, enumSymbol);
                }

                if (value is not null)
                {
                    reservedValues.Add(value.Value);

                    ExpressionSyntax expression;

                    if (useBitShift
                        && value.Value > 1)
                    {
                        var power = (int)Math.Log(Convert.ToDouble(value.Value), 2);

                        expression = LeftShiftExpression(NumericLiteralExpression(1), NumericLiteralExpression(power));
                    }
                    else
                    {
                        expression = NumericLiteralExpression(value.Value, enumSymbol.EnumUnderlyingType.SpecialType);
                    }

                    EnumMemberDeclarationSyntax newMember = members[i].Update(
                        members[i].AttributeLists,
                        members[i].Modifiers,
                        members[i].Identifier.WithoutTrailingTrivia(),
                        EqualsValueClause(expression).WithTrailingTrivia(members[i].Identifier.TrailingTrivia));

                    newMembers = newMembers.ReplaceAt(i, newMember);
                }
            }
        }

        EnumDeclarationSyntax newEnumDeclaration = enumDeclaration.WithMembers(newMembers);

        return await document.ReplaceNodeAsync(enumDeclaration, newEnumDeclaration, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<Document> UseBitShiftOperatorAsync(
        Document document,
        EnumDeclarationSyntax enumDeclaration,
        CancellationToken cancellationToken)
    {
        SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

        EnumDeclarationSyntax newEnumDeclaration = enumDeclaration.ReplaceNodes(
            GetExpressionsToRewrite(),
            (expression, _) =>
            {
                Optional<object> constantValue = semanticModel.GetConstantValue(expression, cancellationToken);

                var power = (int)Math.Log(Convert.ToDouble(constantValue.Value), 2);

                BinaryExpressionSyntax leftShift = LeftShiftExpression(NumericLiteralExpression(1), NumericLiteralExpression(power));

                return leftShift.WithTriviaFrom(expression);
            });

        return await document.ReplaceNodeAsync(enumDeclaration, newEnumDeclaration, cancellationToken).ConfigureAwait(false);

        IEnumerable<ExpressionSyntax> GetExpressionsToRewrite()
        {
            foreach (EnumMemberDeclarationSyntax member in enumDeclaration.Members)
            {
                ExpressionSyntax expression = member.EqualsValue?.Value.WalkDownParentheses();

                if (expression is not null
                    && semanticModel.GetDeclaredSymbol(member, cancellationToken) is IFieldSymbol fieldSymbol
                    && fieldSymbol.HasConstantValue)
                {
                    EnumFieldSymbolInfo fieldInfo = EnumFieldSymbolInfo.Create(fieldSymbol);

                    if (fieldInfo.Value > 1
                        && !fieldInfo.HasCompositeValue())
                    {
                        yield return expression;
                    }
                }
            }
        }
    }
}
