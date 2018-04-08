// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Analysis
{
    public static class CallExtensionMethodAsInstanceMethodAnalysis
    {
        public const string Title = "Call extension method as instance method";

        private static CallExtensionMethodAsInstanceMethodAnalysisResult Fail { get; }

        public static CallExtensionMethodAsInstanceMethodAnalysisResult Analyze(
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

            IMethodSymbol methodSymbol = semanticModel.GetMethodSymbol(invocationExpression, cancellationToken);

            if (methodSymbol == null)
                return Fail;

            if (!methodSymbol.IsOrdinaryExtensionMethod())
                return Fail;

            InvocationExpressionSyntax newInvocationExpression = GetNewInvocation(invocationExpression);

            if (newInvocationExpression == null)
                return Fail;

            if (semanticModel
                .GetSpeculativeMethodSymbol(invocationExpression.SpanStart, newInvocationExpression)?
                .ReducedFromOrSelf()
                .Equals(methodSymbol.ConstructedFrom) != true)
            {
                return Fail;
            }

            return new CallExtensionMethodAsInstanceMethodAnalysisResult(invocationExpression, newInvocationExpression, methodSymbol);
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

        private static InvocationExpressionSyntax GetNewInvocation(InvocationExpressionSyntax invocation)
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