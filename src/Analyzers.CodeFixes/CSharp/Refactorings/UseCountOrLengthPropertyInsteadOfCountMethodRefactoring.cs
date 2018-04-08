// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System.Collections.Generic;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseCountOrLengthPropertyInsteadOfCountMethodRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            string propertyName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            IEnumerable<SyntaxTrivia> trailingTrivia = memberAccess.Name.GetTrailingTrivia().Where(f => !f.IsWhitespaceOrEndOfLineTrivia())
                .Concat(invocation.ArgumentList.DescendantTrivia().Where(f => !f.IsWhitespaceOrEndOfLineTrivia()))
                .Concat(invocation.GetTrailingTrivia());

            IdentifierNameSyntax newName = IdentifierName(propertyName)
                .WithLeadingTrivia(memberAccess.Name.GetLeadingTrivia())
                .WithTrailingTrivia(trailingTrivia);

            MemberAccessExpressionSyntax newNode = memberAccess.WithName(newName);

            return document.ReplaceNodeAsync(invocation, newNode, cancellationToken);
        }
    }
}
