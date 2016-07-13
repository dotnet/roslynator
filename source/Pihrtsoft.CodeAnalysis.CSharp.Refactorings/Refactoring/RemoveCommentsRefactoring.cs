// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Removers;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class RemoveCommentsRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            CommentRemoveOptions removeOptions,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            CompilationUnitSyntax newRoot = CommentRemover.RemoveFrom((CompilationUnitSyntax)oldRoot, removeOptions)
                .WithFormatterAnnotation();

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
