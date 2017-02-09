// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings.EnumWithFlagsAttribute
{
    internal static class GenerateEnumValuesRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, EnumDeclarationSyntax enumDeclaration)
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            var enumSymbol = semanticModel.GetDeclaredSymbol(enumDeclaration, context.CancellationToken) as INamedTypeSymbol;

            if (Symbol.IsEnumWithFlagsAttribute(enumSymbol, semanticModel))
            {
                SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = enumDeclaration.Members;

                if (members.Any(f => f.EqualsValue == null))
                {
                    SpecialType specialType = enumSymbol.EnumUnderlyingType.SpecialType;

                    List<object> values = GenerateEnumHelper.GetExplicitValues(enumDeclaration, semanticModel, context.CancellationToken);

                    Optional<object> optional = EnumHelper.GetUniquePowerOfTwo(specialType, values);

                    if (optional.HasValue)
                    {
                        context.RegisterRefactoring(
                            "Generate enum values",
                            cancellationToken => RefactorAsync(context.Document, enumDeclaration, enumSymbol, startFromHighestExistingValue: false, cancellationToken: cancellationToken));

                        if (members.Any(f => f.EqualsValue != null))
                        {
                            Optional<object> optional2 = EnumHelper.GetUniquePowerOfTwo(specialType, values, startFromHighestExistingValue: true);

                            if (optional2.HasValue
                                && !optional.Value.Equals(optional2.Value))
                            {
                                context.RegisterRefactoring(
                                    $"Generate enum values (starting from {optional2.Value})",
                                    cancellationToken => RefactorAsync(context.Document, enumDeclaration, enumSymbol, startFromHighestExistingValue: true, cancellationToken: cancellationToken));
                            }
                        }
                    }
                }
            }
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

            List<object> values = GenerateEnumHelper.GetExplicitValues(enumDeclaration, semanticModel, cancellationToken);

            for (int i = 0; i < members.Count; i++)
            {
                if (members[i].EqualsValue == null)
                {
                    Optional<object> optional = EnumHelper.GetUniquePowerOfTwo(specialType, values, startFromHighestExistingValue);

                    if (optional.HasValue)
                    {
                        values.Add(optional.Value);

                        EqualsValueClauseSyntax equalsValue = EqualsValueClause(CSharpFactory.ConstantExpression(optional.Value));

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
    }
}