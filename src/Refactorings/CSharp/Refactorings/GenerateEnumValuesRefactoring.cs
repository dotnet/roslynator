// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class GenerateEnumValuesRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, EnumDeclarationSyntax enumDeclaration)
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            INamedTypeSymbol enumSymbol = semanticModel.GetDeclaredSymbol(enumDeclaration, context.CancellationToken);

            if (!enumSymbol.IsEnumWithFlags(semanticModel))
                return;

            SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = enumDeclaration.Members;

            if (!members.Any(f => f.EqualsValue == null))
                return;

            SpecialType specialType = enumSymbol.EnumUnderlyingType.SpecialType;

            List<object> values = GetExplicitValues(enumDeclaration, semanticModel, context.CancellationToken);

            Optional<object> optional = FlagsUtility.GetUniquePowerOfTwo(specialType, values);

            if (!optional.HasValue)
                return;

            context.RegisterRefactoring(
                "Generate enum values",
                cancellationToken => RefactorAsync(context.Document, enumDeclaration, enumSymbol, startFromHighestExistingValue: false, cancellationToken: cancellationToken));

            if (!members.Any(f => f.EqualsValue != null))
                return;

            Optional<object> optional2 = FlagsUtility.GetUniquePowerOfTwo(specialType, values, startFromHighestExistingValue: true);

            if (!optional2.HasValue)
                return;

            if (optional.Value.Equals(optional2.Value))
                return;

            context.RegisterRefactoring(
                $"Generate enum values (starting from {optional2.Value})",
                cancellationToken => RefactorAsync(context.Document, enumDeclaration, enumSymbol, startFromHighestExistingValue: true, cancellationToken: cancellationToken));
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            EnumDeclarationSyntax enumDeclaration,
            INamedTypeSymbol enumSymbol,
            bool startFromHighestExistingValue,
            CancellationToken cancellationToken)
        {
            SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = enumDeclaration.Members;

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            SpecialType specialType = enumSymbol.EnumUnderlyingType.SpecialType;

            List<object> values = GetExplicitValues(enumDeclaration, semanticModel, cancellationToken);

            for (int i = 0; i < members.Count; i++)
            {
                if (members[i].EqualsValue == null)
                {
                    Optional<object> optional = FlagsUtility.GetUniquePowerOfTwo(specialType, values, startFromHighestExistingValue);

                    if (optional.HasValue)
                    {
                        values.Add(optional.Value);

                        EqualsValueClauseSyntax equalsValue = EqualsValueClause(CSharpFactory.LiteralExpression(optional.Value));

                        EnumMemberDeclarationSyntax newMember = members[i]
                            .WithEqualsValue(equalsValue)
                            .WithFormatterAnnotation();

                        members = members.ReplaceAt(i, newMember);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            EnumDeclarationSyntax newNode = enumDeclaration.WithMembers(members);

            return await document.ReplaceNodeAsync(enumDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }

        private static List<object> GetExplicitValues(
            EnumDeclarationSyntax enumDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            var values = new List<object>();

            foreach (EnumMemberDeclarationSyntax member in enumDeclaration.Members)
            {
                EqualsValueClauseSyntax equalsValue = member.EqualsValue;

                if (equalsValue != null)
                {
                    ExpressionSyntax value = equalsValue.Value;

                    if (value != null)
                    {
                        IFieldSymbol fieldSymbol = semanticModel.GetDeclaredSymbol(member, cancellationToken);

                        if (fieldSymbol?.HasConstantValue == true)
                            values.Add(fieldSymbol.ConstantValue);
                    }
                }
            }

            return values;
        }
    }
}