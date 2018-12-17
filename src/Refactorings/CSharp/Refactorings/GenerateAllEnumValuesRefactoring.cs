// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class GenerateAllEnumValuesRefactoring
    {
        internal static readonly string EquivalenceKey = Roslynator.EquivalenceKey.Join(RefactoringIdentifiers.GenerateEnumValues, "OverwriteExistingValues");

        public static void ComputeRefactoring(
            RefactoringContext context,
            EnumDeclarationSyntax enumDeclaration)
        {
            SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = enumDeclaration.Members;

            if (!members.Any())
                return;

            if (members.All(f => f.EqualsValue?.Value == null))
                return;

            Document document = context.Document;

            context.RegisterRefactoring(
                "Declare explicit values (overwrite existing values)",
                ct => RefactorAsync(document, enumDeclaration, cancellationToken: ct),
                EquivalenceKey);
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            EnumDeclarationSyntax enumDeclaration,
            CancellationToken cancellationToken)
        {
            SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = enumDeclaration.Members;

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            INamedTypeSymbol enumSymbol = semanticModel.GetDeclaredSymbol(enumDeclaration, cancellationToken);

            ulong value = 0;

            IEnumerable<EnumMemberDeclarationSyntax> newMembers = (enumSymbol.HasAttribute(MetadataNames.System_FlagsAttribute))
                ? members.Select(CreateNewFlagsMember)
                : members.Select(CreateNewMember);

            EnumDeclarationSyntax newEnumDeclaration = enumDeclaration.WithMembers(newMembers.ToSeparatedSyntaxList());

            return await document.ReplaceNodeAsync(enumDeclaration, newEnumDeclaration, cancellationToken).ConfigureAwait(false);

            EnumMemberDeclarationSyntax CreateNewFlagsMember(EnumMemberDeclarationSyntax enumMember)
            {
                if (!ConvertHelpers.CanConvert(value, enumSymbol.EnumUnderlyingType.SpecialType))
                    return enumMember;

                IFieldSymbol fieldSymbol = semanticModel.GetDeclaredSymbol(enumMember, cancellationToken);

                if (fieldSymbol.HasConstantValue
                    && FlagsUtility<ulong>.Instance.IsComposite(SymbolUtility.GetEnumValueAsUInt64(fieldSymbol.ConstantValue, enumSymbol)))
                {
                    return enumMember;
                }

                EqualsValueClauseSyntax equalsValue = EqualsValueClause(NumericLiteralExpression(value, enumSymbol.EnumUnderlyingType.SpecialType));

                value = (value == 0) ? 1 : value * 2;

                return enumMember
                    .WithEqualsValue(equalsValue)
                    .WithFormatterAnnotation();
            }

            EnumMemberDeclarationSyntax CreateNewMember(EnumMemberDeclarationSyntax enumMember)
            {
                if (!ConvertHelpers.CanConvert(value, enumSymbol.EnumUnderlyingType.SpecialType))
                    return enumMember;

                EqualsValueClauseSyntax equalsValue = EqualsValueClause(NumericLiteralExpression(value, enumSymbol.EnumUnderlyingType.SpecialType));

                value++;

                return enumMember
                    .WithEqualsValue(equalsValue)
                    .WithFormatterAnnotation();
            }
        }
    }
}