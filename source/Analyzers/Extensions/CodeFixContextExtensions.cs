// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    internal static class CodeFixContextExtensions
    {
        public static async Task<TNode> FindNodeAsync<TNode>(
            this CodeFixContext context,
            bool getInnermostNodeForTie = true) where TNode : SyntaxNode
        {
            SyntaxNode root = await context.Document
                .GetSyntaxRootAsync(context.CancellationToken)
                .ConfigureAwait(false);

            return root
                .FindNode(context.Span, getInnermostNodeForTie)?
                .FirstAncestorOrSelf<TNode>();
        }
    }
}
