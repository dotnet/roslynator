// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
            INamedTypeSymbol enumSymbol = semanticModel.GetDeclaredSymbol(enumDeclaration, context.CancellationToken);

            if (enumSymbol?.HasAttribute(MetadataNames.System_FlagsAttribute) != true)
                return;

            ImmutableArray<ulong> constantValues = selectedMembers
                .Select(f => semanticModel.GetDeclaredSymbol(f, context.CancellationToken))
                .Where(f => f.HasConstantValue)
                .Select(f => SymbolUtility.GetEnumValueAsUInt64(f.ConstantValue, enumSymbol))
                .ToImmutableArray();

            Optional<ulong> optionalValue = FlagsUtility<ulong>.Instance.TryCompose(constantValues);

            if (!optionalValue.HasValue)
                return;

            if (IsValueDefined(enumSymbol, optionalValue))
                return;

            string name = NameGenerator.Default.EnsureUniqueEnumMemberName(
                string.Concat(selectedMembers.Select(f => f.Identifier.ValueText)),
                enumSymbol);

            context.RegisterRefactoring(
                $"Generate enum member '{name}'",
                ct => RefactorAsync(context.Document, enumDeclaration, selectedMembers, name, ct),
                RefactoringIdentifiers.GenerateCombinedEnumMember);
        }

        private static bool IsValueDefined(INamedTypeSymbol enumSymbol, object value)
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

        private static Task<Document> RefactorAsync(
            Document document,
            EnumDeclarationSyntax enumDeclaration,
            SeparatedSyntaxListSelection<EnumMemberDeclarationSyntax> selectedMembers,
            string name,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax expression = IdentifierName(selectedMembers.Last().Identifier.WithoutTrivia());

            for (int i = selectedMembers.LastIndex - 1; i >= selectedMembers.FirstIndex; i--)
            {
                expression = BitwiseOrExpression(
                    IdentifierName(selectedMembers.UnderlyingList[i].Identifier.WithoutTrivia()),
                    expression);
            }

            EnumMemberDeclarationSyntax newEnumMember = EnumMemberDeclaration(
                Identifier(name).WithRenameAnnotation(),
                EqualsValueClause(expression));

            EnumDeclarationSyntax newEnumDeclaration = enumDeclaration
                .WithMembers(enumDeclaration.Members.Insert(selectedMembers.LastIndex + 1, newEnumMember));

            return document.ReplaceNodeAsync(enumDeclaration, newEnumDeclaration, cancellationToken);
        }
    }
}