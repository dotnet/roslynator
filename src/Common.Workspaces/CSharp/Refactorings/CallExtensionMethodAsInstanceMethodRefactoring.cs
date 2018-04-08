// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    public static class CallExtensionMethodAsInstanceMethodRefactoring
    {
        public const string Title = "Call extension method as instance method";

        public static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken)
        {
            InvocationExpressionSyntax newInvocation = GetNewInvocation(invocation);

            return document.ReplaceNodeAsync(invocation, newInvocation, cancellationToken);
        }

        private static InvocationExpressionSyntax GetNewInvocation(InvocationExpressionSyntax invocation)
        {
            ExpressionSyntax expression = invocation.Expression;
            ArgumentListSyntax argumentList = invocation.ArgumentList;
            SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;
            ArgumentSyntax argument = arguments.First();

            MemberAccessExpressionSyntax newMemberAccess = CreateNewMemberAccessExpression();

            return invocation
                .WithExpression(newMemberAccess)
                .WithArgumentList(argumentList.WithArguments(arguments.Remove(argument)));

            MemberAccessExpressionSyntax CreateNewMemberAccessExpression()
            {
                switch (expression.Kind())
                {
                    case SyntaxKind.IdentifierName:
                    case SyntaxKind.GenericName:
                        {
                            ExpressionSyntax newExpression = argument.Expression
                                .WithLeadingTrivia(expression.GetLeadingTrivia())
                                .Parenthesize();

                            return SimpleMemberAccessExpression(
                                newExpression,
                                (SimpleNameSyntax)expression.WithoutLeadingTrivia());
                        }
                    case SyntaxKind.SimpleMemberAccessExpression:
                        {
                            var memberAccess = (MemberAccessExpressionSyntax)expression;

                            ExpressionSyntax newExpression = argument.Expression
                                .WithTriviaFrom(memberAccess.Expression)
                                .Parenthesize();

                            return memberAccess.WithExpression(newExpression);
                        }
                }

                throw new InvalidOperationException();
            }
        }
    }
}