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
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class GenerateCombinedEnumMemberRefactoring
    {
        public static void ComputeRefactoring(
            RefactoringContext context,
            EnumDeclarationSyntax enumDeclaration,
            SeparatedSyntaxListSelection<EnumMemberDeclarationSyntax> selectedMembers,
            SemanticModel semanticModel)
        {
            if (!enumDeclaration.AttributeLists.Any())
                return;

            INamedTypeSymbol enumSymbol = semanticModel.GetDeclaredSymbol(enumDeclaration, context.CancellationToken);

            if (enumSymbol?.HasAttribute(MetadataNames.System_FlagsAttribute) != true)
                return;

            IEnumerable<(IFieldSymbol symbol, ulong value)> symbolsValues = selectedMembers
                .Select(f => semanticModel.GetDeclaredSymbol(f, context.CancellationToken))
                .Where(f => f.HasConstantValue)
                .Select(f => (symbol: f, value: SymbolUtility.GetEnumValueAsUInt64(f.ConstantValue, enumSymbol)))
                .Where(f => !FlagsUtility<ulong>.Instance.IsComposite(f.value));

            ImmutableArray<ulong> constantValues = symbolsValues.Select(f => f.value).ToImmutableArray();

            if (constantValues.Length <= 1)
                return;

            Optional<ulong> optionalValue = FlagsUtility<ulong>.Instance.Combine(constantValues);

            if (!optionalValue.HasValue)
                return;

            if (IsValueDefined(optionalValue))
                return;

            string name = NameGenerator.Default.EnsureUniqueEnumMemberName(
                string.Concat(symbolsValues.Select(f => f.symbol.Name)),
                enumSymbol);

            context.RegisterRefactoring(
                $"Generate member '{name}'",
                ct => RefactorAsync(context.Document, enumDeclaration, selectedMembers, enumSymbol, name, semanticModel, ct),
                RefactoringIdentifiers.GenerateCombinedEnumMember);

            bool IsValueDefined(object value)
            {
                foreach (ISymbol member in enumSymbol.GetMembers())
                {
                    if (member.Kind == SymbolKind.Field)
                    {
                        var fieldSymbol = (IFieldSymbol)member;

                        if (fieldSymbol.HasConstantValue
                            && object.Equals(fieldSymbol.ConstantValue, value))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            EnumDeclarationSyntax enumDeclaration,
            SeparatedSyntaxListSelection<EnumMemberDeclarationSyntax> selectedMembers,
            INamedTypeSymbol enumSymbol,
            string name,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax expression = null;
            EnumMemberDeclarationSyntax lastMember = null;

            using (IEnumerator<EnumMemberDeclarationSyntax> en = GetMembersToCombine().GetEnumerator())
            {
                if (en.MoveNext())
                {
                    lastMember = en.Current;
                    expression = IdentifierName(en.Current.Identifier.WithoutTrivia());

                    while (en.MoveNext())
                    {
                        expression = BitwiseOrExpression(
                            IdentifierName(en.Current.Identifier.WithoutTrivia()),
                            expression);
                    }
                }
            }

            EnumMemberDeclarationSyntax newEnumMember = EnumMemberDeclaration(
                Identifier(name).WithRenameAnnotation(),
                EqualsValueClause(expression));

            EnumDeclarationSyntax newEnumDeclaration = enumDeclaration
                .WithMembers(enumDeclaration.Members.Insert(selectedMembers.UnderlyingList.IndexOf(lastMember) + 1, newEnumMember));

            return document.ReplaceNodeAsync(enumDeclaration, newEnumDeclaration, cancellationToken);

            IEnumerable<EnumMemberDeclarationSyntax> GetMembersToCombine()
            {
                for (int i = selectedMembers.Count - 1; i >= 0; i--)
                {
                    IFieldSymbol symbol = semanticModel.GetDeclaredSymbol(selectedMembers[i], cancellationToken);

                    if (symbol.HasConstantValue)
                    {
                        ulong value = SymbolUtility.GetEnumValueAsUInt64(symbol.ConstantValue, enumSymbol);

                        if (!FlagsUtility<ulong>.Instance.IsComposite(value))
                        {
                            yield return selectedMembers[i];
                        }
                    }
                }
            }
        }
    }
}