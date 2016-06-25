// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis.CSharp.Removers;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    public static class RemoveAllCommentsRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            bool keepXmlComment = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            CompilationUnitSyntax newRoot = CommentRemover.RemoveFrom((CompilationUnitSyntax)oldRoot, keepXmlComment)
                .WithAdditionalAnnotations(Formatter.Annotation);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
