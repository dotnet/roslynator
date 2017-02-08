// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings.UseInsteadOfCountMethod
{
    internal static class UseCountOrLengthPropertyInsteadOfCountMethodRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            string propertyName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            memberAccess = memberAccess
                .WithName(
                    IdentifierName(propertyName)
                        .WithTriviaFrom(memberAccess.Name))
                .WithTriviaFrom(invocation);

            SyntaxNode newRoot = root.ReplaceNode(invocation, memberAccess);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
