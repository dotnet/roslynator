// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Comparers;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseConstantInsteadOfFieldRefactoring
    {
        public static async Task<bool> CanRefactorAsync(
            RefactoringContext context,
            FieldDeclarationSyntax declaration)
        {
            TypeSyntax type = declaration.Declaration?.Type;

            if (type != null
                && declaration.Declaration.Variables.All(f => f.Initializer != null))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, context.CancellationToken);

                return typeSymbol?.SupportsConstantValue() == true;
            }

            return false;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            FieldDeclarationSyntax node,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            FieldDeclarationSyntax newNode = node
                .RemoveModifier(SyntaxKind.StaticKeyword)
                .RemoveModifier(SyntaxKind.ReadOnlyKeyword)
                .InsertModifier(SyntaxKind.ConstKeyword, ModifierComparer.Instance)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(node, newNode, cancellationToken);
        }
    }
}
