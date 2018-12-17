// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
        internal const string EquivalenceKey = RefactoringIdentifiers.GenerateEnumValues;

        internal static readonly string StartFromHighestExistingValueEquivalenceKey = Roslynator.EquivalenceKey.Join(EquivalenceKey, "StartFromHighestExistingValue");

        public static void ComputeRefactoring(
            RefactoringContext context,
            EnumDeclarationSyntax enumDeclaration,
            SemanticModel semanticModel)
        {
            INamedTypeSymbol enumSymbol = semanticModel.GetDeclaredSymbol(enumDeclaration, context.CancellationToken);

            if (enumSymbol?.HasAttribute(MetadataNames.System_FlagsAttribute) != true)
                return;

            SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = enumDeclaration.Members;

            if (!members.Any(f => f.EqualsValue == null))
                return;

            ImmutableArray<ulong> values = GetExplicitValues(enumDeclaration, semanticModel, context.CancellationToken);

            Optional<ulong> optional = FlagsUtility<ulong>.Instance.GetUniquePowerOfTwo(values);

            if (!optional.HasValue)
                return;

            if (!ConvertHelpers.CanConvert(optional.Value, enumSymbol.EnumUnderlyingType.SpecialType))
                return;

            Document document = context.Document;

            context.RegisterRefactoring(
                "Declare explicit values",
                ct => RefactorAsync(document, enumDeclaration, enumSymbol, values, startFromHighestExistingValue: false, cancellationToken: ct),
                EquivalenceKey);

            if (members.Any(f => f.EqualsValue != null))
            {
                Optional<ulong> optional2 = FlagsUtility<ulong>.Instance.GetUniquePowerOfTwo(values, startFromHighestExistingValue: true);

                if (optional2.HasValue
                    && optional.Value != optional2.Value)
                {
                    context.RegisterRefactoring(
                        $"Declare explicit values (starting from {optional2.Value})",
                        ct => RefactorAsync(document, enumDeclaration, enumSymbol, values, startFromHighestExistingValue: true, cancellationToken: ct),
                        StartFromHighestExistingValueEquivalenceKey);
                }
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            EnumDeclarationSyntax enumDeclaration,
            INamedTypeSymbol enumSymbol,
            ImmutableArray<ulong> values,
            bool startFromHighestExistingValue,
            CancellationToken cancellationToken)
        {
            SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = enumDeclaration.Members;

            List<ulong> valuesList = values.ToList();

            for (int i = 0; i < members.Count; i++)
            {
                if (members[i].EqualsValue == null)
                {
                    Optional<ulong> optional = FlagsUtility<ulong>.Instance.GetUniquePowerOfTwo(valuesList, startFromHighestExistingValue);

                    if (optional.HasValue
                        && ConvertHelpers.CanConvert(optional.Value, enumSymbol.EnumUnderlyingType.SpecialType))
                    {
                        valuesList.Add(optional.Value);

                        EqualsValueClauseSyntax equalsValue = EqualsValueClause(CSharpFactory.NumericLiteralExpression(optional.Value, enumSymbol.EnumUnderlyingType.SpecialType));

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

        private static ImmutableArray<ulong> GetExplicitValues(
            EnumDeclarationSyntax enumDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ImmutableArray<ulong>.Builder values = ImmutableArray.CreateBuilder<ulong>();

            foreach (EnumMemberDeclarationSyntax member in enumDeclaration.Members)
            {
                ExpressionSyntax value = member.EqualsValue?.Value;

                if (value != null)
                {
                    IFieldSymbol fieldSymbol = semanticModel.GetDeclaredSymbol(member, cancellationToken);

                    if (fieldSymbol?.HasConstantValue == true)
                        values.Add(SymbolUtility.GetEnumValueAsUInt64(fieldSymbol.ConstantValue, fieldSymbol.ContainingType));
                }
            }

            return values.ToImmutableArray();
        }
    }
}