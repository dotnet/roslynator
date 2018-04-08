// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseCountOrLengthPropertyInsteadOfAnyMethodRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            string propertyName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            memberAccess = memberAccess
                .WithName(IdentifierName(propertyName).WithTriviaFrom(memberAccess.Name))
                .AppendToTrailingTrivia(invocation.ArgumentList.DescendantTrivia().Where(f => !f.IsWhitespaceOrEndOfLineTrivia()));

            if (invocation.IsParentKind(SyntaxKind.LogicalNotExpression))
            {
                var logicalNot = (PrefixUnaryExpressionSyntax)invocation.Parent;

                IEnumerable<SyntaxTrivia> leadingTrivia = logicalNot.GetLeadingTrivia()
                    .Concat(logicalNot.OperatorToken.TrailingTrivia.Where(f => !f.IsWhitespaceOrEndOfLineTrivia()))
                    .Concat(logicalNot.Operand.GetLeadingTrivia().Where(f => !f.IsWhitespaceOrEndOfLineTrivia()));

                BinaryExpressionSyntax newNode = EqualsExpression(memberAccess, NumericLiteralExpression(0))
                    .WithLeadingTrivia(leadingTrivia)
                    .WithTrailingTrivia(logicalNot.GetTrailingTrivia());

                return document.ReplaceNodeAsync(invocation.Parent, newNode, cancellationToken);
            }
            else
            {
                BinaryExpressionSyntax newNode = GreaterThanExpression(memberAccess, NumericLiteralExpression(0))
                    .WithTriviaFrom(invocation);

                return document.ReplaceNodeAsync(invocation, newNode, cancellationToken);
            }
        }
    }
}
