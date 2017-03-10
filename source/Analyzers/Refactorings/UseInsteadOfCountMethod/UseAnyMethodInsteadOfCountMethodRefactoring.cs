// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.UseInsteadOfCountMethod
{
    internal static class UseAnyMethodInsteadOfCountMethodRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            InvocationExpressionSyntax invocation = GetInvocationExpression(binaryExpression);

            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            memberAccess = memberAccess
                .WithName(
                    IdentifierName("Any")
                        .WithTriviaFrom(memberAccess.Name));

            ExpressionSyntax newNode = invocation.WithExpression(memberAccess);

            if (binaryExpression.IsKind(SyntaxKind.EqualsExpression))
                newNode = LogicalNotExpression(newNode);

            newNode = newNode.WithTriviaFrom(binaryExpression);

            SyntaxNode newRoot = oldRoot.ReplaceNode(binaryExpression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static InvocationExpressionSyntax GetInvocationExpression(BinaryExpressionSyntax binaryExpression)
        {
            if (binaryExpression.Left.IsKind(SyntaxKind.InvocationExpression))
            {
                return (InvocationExpressionSyntax)binaryExpression.Left;
            }
            else
            {
                return (InvocationExpressionSyntax)binaryExpression.Right;
            }
        }
    }
}
