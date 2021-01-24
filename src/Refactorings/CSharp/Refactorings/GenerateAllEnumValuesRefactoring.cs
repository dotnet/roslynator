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
            EnumDeclarationSyntax enumDeclaration,
            SemanticModel semanticModel)
        {
            SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = enumDeclaration.Members;

            if (!members.Any())
                return;

            if (members.All(f => f.EqualsValue?.Value == null))
                return;

            INamedTypeSymbol enumSymbol = semanticModel.GetDeclaredSymbol(enumDeclaration, context.CancellationToken);

            bool isFlags = enumSymbol.HasAttribute(MetadataNames.System_FlagsAttribute);

            if (!AreNewValuesDifferentFromExistingValues())
                return;

            Document document = context.Document;

            context.RegisterRefactoring(
                "Declare explicit values (overwrite existing values)",
                ct => RefactorAsync(document, enumDeclaration, enumSymbol, semanticModel, ct),
                EquivalenceKey);

            bool AreNewValuesDifferentFromExistingValues()
            {
                ulong value = 0;

                foreach (EnumMemberDeclarationSyntax member in enumDeclaration.Members)
                {
                    IFieldSymbol fieldSymbol = semanticModel.GetDeclaredSymbol(member, context.CancellationToken);

                    EnumFieldSymbolInfo fieldSymbolInfo = EnumFieldSymbolInfo.Create(fieldSymbol);

                    if (!fieldSymbolInfo.HasValue)
                        return true;

                    if (isFlags
                        && fieldSymbolInfo.HasCompositeValue())
                    {
                        continue;
                    }

                    if (value != fieldSymbolInfo.Value)
                        return true;

                    if (isFlags)
                    {
                        value = (value == 0) ? 1 : value * 2;
                    }
                    else
                    {
                        value++;
                    }

                    if (!ConvertHelpers.CanConvertFromUInt64(value, enumSymbol.EnumUnderlyingType.SpecialType))
                        return false;
                }

                return false;
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            EnumDeclarationSyntax enumDeclaration,
            INamedTypeSymbol enumSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ulong value = 0;

            SpecialType numericType = enumSymbol.EnumUnderlyingType.SpecialType;

            IEnumerable<EnumMemberDeclarationSyntax> newMembers = (enumSymbol.HasAttribute(MetadataNames.System_FlagsAttribute))
                ? enumDeclaration.Members.Select(f => CreateNewFlagsMember(f))
                : enumDeclaration.Members.Select(f => CreateNewMember(f));

            EnumDeclarationSyntax newEnumDeclaration = enumDeclaration.ReplaceNodes(
                enumDeclaration.Members,
                (f, _) =>
                {
                    return (enumSymbol.HasAttribute(MetadataNames.System_FlagsAttribute))
                        ? CreateNewFlagsMember(f)
                        : CreateNewMember(f);
                });

            return document.ReplaceNodeAsync(enumDeclaration, newEnumDeclaration, cancellationToken);

            EnumMemberDeclarationSyntax CreateNewFlagsMember(EnumMemberDeclarationSyntax enumMember)
            {
                if (!ConvertHelpers.CanConvertFromUInt64(value, numericType))
                    return enumMember;

                IFieldSymbol fieldSymbol = semanticModel.GetDeclaredSymbol(enumMember, cancellationToken);

                if (fieldSymbol.HasConstantValue
                    && FlagsUtility<ulong>.Instance.IsComposite(SymbolUtility.GetEnumValueAsUInt64(fieldSymbol.ConstantValue, enumSymbol)))
                {
                    return enumMember;
                }

                EnumMemberDeclarationSyntax newEnumMember = CreateNewEnumMember(enumMember, value, numericType);

                value = (value == 0) ? 1 : value * 2;

                return newEnumMember;
            }

            EnumMemberDeclarationSyntax CreateNewMember(EnumMemberDeclarationSyntax enumMember)
            {
                if (!ConvertHelpers.CanConvertFromUInt64(value, numericType))
                    return enumMember;

                EnumMemberDeclarationSyntax newEnumMember = CreateNewEnumMember(enumMember, value, numericType);

                value++;

                return newEnumMember;
            }
        }

        private static EnumMemberDeclarationSyntax CreateNewEnumMember(
            EnumMemberDeclarationSyntax enumMember,
            ulong value,
            SpecialType numericType)
        {
            EqualsValueClauseSyntax equalsValue = EqualsValueClause(
                Token(TriviaList(ElasticSpace), SyntaxKind.EqualsToken, TriviaList(ElasticSpace)),
                NumericLiteralExpression(value, numericType));

            if (enumMember.EqualsValue != null)
                equalsValue = equalsValue.WithTriviaFrom(enumMember.EqualsValue);

            return enumMember.WithEqualsValue(equalsValue);
        }
    }
}