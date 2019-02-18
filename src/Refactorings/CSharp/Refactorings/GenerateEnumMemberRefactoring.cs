// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class GenerateEnumMemberRefactoring
    {
        internal const string EquivalenceKey = RefactoringIdentifiers.GenerateEnumMember;

        internal static readonly string StartFromHighestExistingValueEquivalenceKey = Roslynator.EquivalenceKey.Join(EquivalenceKey, "StartFromHighestExistingValue");

        public static void ComputeRefactoring(
            RefactoringContext context,
            EnumDeclarationSyntax enumDeclaration,
            SemanticModel semanticModel)
        {
            Document document = context.Document;

            INamedTypeSymbol enumSymbol = semanticModel.GetDeclaredSymbol(enumDeclaration, context.CancellationToken);

            if (enumSymbol?.HasAttribute(MetadataNames.System_FlagsAttribute) == true)
            {
                ImmutableArray<ulong> values = GetConstantValues(enumSymbol);

                Optional<ulong> optional = FlagsUtility<ulong>.Instance.GetUniquePowerOfTwo(values);

                if (optional.HasValue
                    && ConvertHelpers.CanConvert(optional.Value, enumSymbol.EnumUnderlyingType.SpecialType))
                {
                    context.RegisterRefactoring(
                        "Generate enum member",
                        ct => RefactorAsync(document, enumDeclaration, enumSymbol, optional.Value, ct),
                        EquivalenceKey);

                    Optional<ulong> optional2 = FlagsUtility<ulong>.Instance.GetUniquePowerOfTwo(values, startFromHighestExistingValue: true);

                    if (optional2.HasValue
                        && ConvertHelpers.CanConvert(optional2.Value, enumSymbol.EnumUnderlyingType.SpecialType)
                        && optional.Value != optional2.Value)
                    {
                        context.RegisterRefactoring(
                            $"Generate enum member (with value {optional2.Value})",
                            ct => RefactorAsync(document, enumDeclaration, enumSymbol, optional2.Value, ct),
                            StartFromHighestExistingValueEquivalenceKey);
                    }
                }
            }
            else
            {
                context.RegisterRefactoring(
                    "Generate enum member",
                    ct => RefactorAsync(document, enumDeclaration, enumSymbol, null, ct),
                    EquivalenceKey);
            }
        }

        private static ImmutableArray<ulong> GetConstantValues(INamedTypeSymbol enumSymbol)
        {
            ImmutableArray<ulong>.Builder values = ImmutableArray.CreateBuilder<ulong>();

            foreach (ISymbol member in enumSymbol.GetMembers())
            {
                if (member.Kind == SymbolKind.Field)
                {
                    var fieldSymbol = (IFieldSymbol)member;

                    if (fieldSymbol.HasConstantValue)
                        values.Add(SymbolUtility.GetEnumValueAsUInt64(fieldSymbol.ConstantValue, enumSymbol));
                }
            }

            return values.ToImmutableArray();
        }

        private static Task<Document> RefactorAsync(
            Document document,
            EnumDeclarationSyntax enumDeclaration,
            INamedTypeSymbol enumSymbol,
            ulong? value,
            CancellationToken cancellationToken)
        {
            EqualsValueClauseSyntax equalsValue = null;

            if (value != null)
                equalsValue = EqualsValueClause(CSharpFactory.NumericLiteralExpression(value.Value, enumSymbol.EnumUnderlyingType.SpecialType));

            string name = NameGenerator.Default.EnsureUniqueEnumMemberName(DefaultNames.EnumMember, enumSymbol);

            SyntaxToken identifier = Identifier(name).WithRenameAnnotation();

            EnumMemberDeclarationSyntax newEnumMember = EnumMemberDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                identifier,
                equalsValue);

            EnumDeclarationSyntax newNode = enumDeclaration.AddMembers(newEnumMember);

            return document.ReplaceNodeAsync(enumDeclaration, newNode, cancellationToken);
        }
    }
}