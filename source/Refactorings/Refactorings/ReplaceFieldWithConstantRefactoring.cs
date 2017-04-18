// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceFieldWithConstantRefactoring
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
            SyntaxTokenList modifiers = node.Modifiers;

            int index = modifiers.IndexOf(SyntaxKind.StaticKeyword);

            if (index != -1)
                modifiers = modifiers.RemoveAt(index);

            index = modifiers.IndexOf(SyntaxKind.ReadOnlyKeyword);

            modifiers = modifiers
                .RemoveAt(index)
                .Insert(index, ConstKeyword().WithTriviaFrom(modifiers[index]));

            FieldDeclarationSyntax newNode = node
                .WithModifiers(modifiers)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(node, newNode, cancellationToken);
        }
    }
}
