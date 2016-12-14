// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class FormatAccessorBraceOnSingleLineRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            AccessorDeclarationSyntax accessor,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            AccessorDeclarationSyntax newAccessor = SyntaxRemover.RemoveWhitespaceOrEndOfLine(accessor)
                .WithTriviaFrom(accessor)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(accessor, newAccessor, cancellationToken).ConfigureAwait(false);
        }
    }
}
