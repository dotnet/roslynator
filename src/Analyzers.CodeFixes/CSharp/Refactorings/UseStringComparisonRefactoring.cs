// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;
using static Roslynator.CSharp.CSharpTypeFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseStringComparisonRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            string comparisonName,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax left = binaryExpression.Left.WalkDownParentheses();
            ExpressionSyntax right = binaryExpression.Right.WalkDownParentheses();

            ExpressionSyntax newNode = SimpleMemberInvocationExpression(
                StringType(),
                IdentifierName("Equals"),
                ArgumentList(
                    CreateArgument(left),
                    CreateArgument(right),
                    Argument(CreateStringComparison(comparisonName))));

            if (binaryExpression.IsKind(SyntaxKind.NotEqualsExpression))
                newNode = LogicalNotExpression(newNode);

            newNode = newNode
                .WithTriviaFrom(binaryExpression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(binaryExpression, newNode, cancellationToken);
        }

        internal static Task<Document> RefactorAsync(
            Document document,
            SimpleMemberInvocationExpressionInfo invocationInfo,
            string comparisonName,
            CancellationToken cancellationToken)
        {
            SeparatedSyntaxList<ArgumentSyntax> arguments = invocationInfo.Arguments;

            InvocationExpressionSyntax invocation = invocationInfo.InvocationExpression;

            if (arguments.Count == 2)
            {
                ArgumentListSyntax newArgumentList = ArgumentList(
                    CreateArgument(arguments[0]),
                    CreateArgument(arguments[1]),
                    Argument(CreateStringComparison(comparisonName)));

                InvocationExpressionSyntax newNode = invocation.WithArgumentList(newArgumentList);

                return document.ReplaceNodeAsync(invocation, newNode, cancellationToken);
            }
            else
            {
                var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;
                var invocation2 = (InvocationExpressionSyntax)memberAccess.Expression;
                var memberAccess2 = (MemberAccessExpressionSyntax)invocation2.Expression;

                MemberAccessExpressionSyntax newMemberAccess = memberAccess.WithExpression(memberAccess2.Expression);

                bool isContains = memberAccess.Name.Identifier.ValueText == "Contains";

                if (isContains)
                    newMemberAccess = newMemberAccess.WithName(newMemberAccess.Name.WithIdentifier(Identifier("IndexOf").WithTriviaFrom(newMemberAccess.Name.Identifier)));

                ArgumentListSyntax newArgumentList = ArgumentList(
                    CreateArgument(arguments[0]),
                    Argument(CreateStringComparison(comparisonName)));

                ExpressionSyntax newNode = invocation
                    .WithExpression(newMemberAccess)
                    .WithArgumentList(newArgumentList);

                if (isContains)
                    newNode = GreaterThanOrEqualExpression(newNode.Parenthesize(), NumericLiteralExpression(0)).Parenthesize();

                return document.ReplaceNodeAsync(invocation, newNode, cancellationToken);
            }
        }

        private static NameSyntax CreateStringComparison(string comparisonName)
        {
            return ParseName($"System.StringComparison.{comparisonName}").WithSimplifierAnnotation();
        }

        private static ArgumentSyntax CreateArgument(ExpressionSyntax expression)
        {
            switch (expression.Kind())
            {
                case SyntaxKind.StringLiteralExpression:
                    {
                        return Argument(expression);
                    }
                case SyntaxKind.InvocationExpression:
                    {
                        var invocation = (InvocationExpressionSyntax)expression;

                        var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

                        return Argument(memberAccess.Expression).WithTriviaFrom(expression);
                    }
                default:
                    {
                        Debug.Fail(expression.Kind().ToString());
                        return Argument(expression);
                    }
            }
        }

        private static ArgumentSyntax CreateArgument(ArgumentSyntax argument)
        {
            ExpressionSyntax expression = argument.Expression.WalkDownParentheses();

            switch (expression.Kind())
            {
                case SyntaxKind.StringLiteralExpression:
                    {
                        return argument;
                    }
                case SyntaxKind.InvocationExpression:
                    {
                        var invocation = (InvocationExpressionSyntax)expression;

                        var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

                        return Argument(memberAccess.Expression).WithTriviaFrom(argument);
                    }
                default:
                    {
                        Debug.Fail(expression.Kind().ToString());
                        return argument;
                    }
            }
        }
    }
}
