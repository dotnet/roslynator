// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class FormatAccessorBraceOnMultipleLinesRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            AccessorDeclarationSyntax accessor,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxToken closeBrace = accessor.Body.CloseBraceToken;

            AccessorDeclarationSyntax newAccessor = accessor
                .WithBody(
                    accessor.Body.WithCloseBraceToken(
                        closeBrace.WithLeadingTrivia(
                            closeBrace.LeadingTrivia.Add(CSharpFactory.NewLineTrivia()))))
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(accessor, newAccessor, cancellationToken).ConfigureAwait(false);
        }
    }
}
