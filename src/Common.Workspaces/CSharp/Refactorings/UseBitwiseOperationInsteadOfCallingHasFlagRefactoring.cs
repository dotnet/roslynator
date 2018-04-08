// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseBitwiseOperationInsteadOfCallingHasFlagRefactoring
    {
        public const string Title = "Use bitwise operation instead of calling 'HasFlag'";

        public static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ParenthesizedExpressionSyntax parenthesizedExpression = ParenthesizedExpression(
                BitwiseAndExpression(
                    ((MemberAccessExpressionSyntax)invocation.Expression).Expression.Parenthesize(),
                    invocation.ArgumentList.Arguments[0].Expression).Parenthesize());

            var binaryExpressionKind = SyntaxKind.NotEqualsExpression;
            SyntaxNode nodeToReplace = invocation;

            SyntaxNode parent = invocation.Parent;

            if (!parent.SpanContainsDirectives())
            {
                SyntaxKind parentKind = parent.Kind();

                if (parentKind == SyntaxKind.LogicalNotExpression)
                {
                    binaryExpressionKind = SyntaxKind.EqualsExpression;
                    nodeToReplace = parent;
                }
                else if (parentKind == SyntaxKind.EqualsExpression)
                {
                    ExpressionSyntax right = ((BinaryExpressionSyntax)parent).Right;

                    if (right != null)
                    {
                        SyntaxKind rightKind = right.Kind();

                        if (rightKind == SyntaxKind.TrueLiteralExpression)
                        {
                            binaryExpressionKind = SyntaxKind.NotEqualsExpression;
                            nodeToReplace = parent;
                        }
                        else if (rightKind == SyntaxKind.FalseLiteralExpression)
                        {
                            binaryExpressionKind = SyntaxKind.EqualsExpression;
                            nodeToReplace = parent;
                        }
                    }
                }
            }

            ParenthesizedExpressionSyntax newNode = BinaryExpression(binaryExpressionKind, parenthesizedExpression, NumericLiteralExpression(0))
                .WithTriviaFrom(nodeToReplace)
                .Parenthesize()
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(nodeToReplace, newNode, cancellationToken);
        }
    }
}
