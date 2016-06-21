// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis.CSharp.Removers;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class FormatAccessorBraceOnSingleLineRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            AccessorDeclarationSyntax accessor,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            AccessorDeclarationSyntax newAccessor = WhitespaceOrEndOfLineRemover.RemoveFrom(accessor)
                .WithTriviaFrom(accessor)
                .WithAdditionalAnnotations(Formatter.Annotation);

            root = root.ReplaceNode(accessor, newAccessor);

            return document.WithSyntaxRoot(root);
        }
    }
}
