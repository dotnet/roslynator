// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    public static class CallExtensionMethodAsInstanceMethodRefactoring
    {
        public const string Title = "Call extension method as instance method";

        private static CallExtensionMethodAsInstanceMethodAnalysis Fail { get; }

        public static CallExtensionMethodAsInstanceMethodAnalysis Analyze(
            InvocationExpressionSyntax invocationExpression,
            SemanticModel semanticModel,
            bool allowAnyExpression = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionSyntax expression = invocationExpression
                .ArgumentList?
                .Arguments
                .FirstOrDefault()?
                .Expression?
                .WalkDownParentheses();

            if (expression == null)
                return Fail;

            if (!allowAnyExpression)
            {
                switch (expression.Kind())
                {
                    case SyntaxKind.IdentifierName:
                    case SyntaxKind.GenericName:
                    case SyntaxKind.InvocationExpression:
                    case SyntaxKind.SimpleMemberAccessExpression:
                    case SyntaxKind.ElementAccessExpression:
                    case SyntaxKind.ConditionalAccessExpression:
                    case SyntaxKind.CastExpression:
                        break;
                    default:
                        return Fail;
                }
            }

            if (!semanticModel.TryGetMethodInfo(invocationExpression, out MethodInfo methodInfo, cancellationToken))
                return Fail;

            if (!methodInfo.Symbol.IsNonReducedExtensionMethod())
                return Fail;

            InvocationExpressionSyntax newInvocationExpression = GetNewInvocationForAnalysis(invocationExpression);

            if (newInvocationExpression == null)
                return Fail;

            if (semanticModel
                .GetSpeculativeMethodSymbol(invocationExpression.SpanStart, newInvocationExpression)?
                .ReducedFromOrSelf()
                .Equals(methodInfo.ConstructedFrom) != true)
            {
                return Fail;
            }

            return new CallExtensionMethodAsInstanceMethodAnalysis(invocationExpression, newInvocationExpression, methodInfo.Symbol);
        }

        public static SyntaxNodeOrToken GetNodeOrToken(ExpressionSyntax expression)
        {
            switch (expression.Kind())
            {
                case SyntaxKind.IdentifierName:
                    return (SimpleNameSyntax)expression;
                case SyntaxKind.GenericName:
                    return ((GenericNameSyntax)expression).Identifier;
                case SyntaxKind.SimpleMemberAccessExpression:
                    return ((MemberAccessExpressionSyntax)expression).Name;
                default:
                    return null;
            }
        }

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

        private static InvocationExpressionSyntax GetNewInvocationForAnalysis(InvocationExpressionSyntax invocation)
        {
            ExpressionSyntax expression = invocation.Expression;
            ArgumentListSyntax argumentList = invocation.ArgumentList;
            SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;
            ArgumentSyntax argument = arguments.First();

            MemberAccessExpressionSyntax newMemberAccess = CreateNewMemberAccessExpression();

            if (newMemberAccess == null)
                return null;

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
                            return SimpleMemberAccessExpression(
                                ParenthesizedExpression(argument.Expression),
                                (SimpleNameSyntax)expression);
                        }
                    case SyntaxKind.SimpleMemberAccessExpression:
                        {
                            var memberAccess = (MemberAccessExpressionSyntax)expression;

                            return memberAccess.WithExpression(ParenthesizedExpression(argument.Expression));
                        }
                    default:
                        {
                            Debug.Fail(expression.Kind().ToString());
                            return null;
                        }
                }
            }
        }
    }
}