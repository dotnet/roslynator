// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Roslynator.CSharp
{
    internal static class CodeFixContextExtensions
    {
        public static async Task<TNode> FindNodeAsync<TNode>(
            this CodeFixContext context,
            bool findInsideTrivia = false,
            bool getInnermostNodeForTie = false) where TNode : SyntaxNode
        {
            SyntaxNode root = await context.Document
                .GetSyntaxRootAsync(context.CancellationToken)
                .ConfigureAwait(false);

            SyntaxNode node = root.FindNode(
                context.Span,
                findInsideTrivia: findInsideTrivia,
                getInnermostNodeForTie: getInnermostNodeForTie);

            return node?.FirstAncestorOrSelf<TNode>();
        }
    }
}
