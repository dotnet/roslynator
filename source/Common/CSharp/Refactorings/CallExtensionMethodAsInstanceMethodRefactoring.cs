// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
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

        private static CallExtensionMethodAsInstanceMethodAnalysis Fail { get; }

        public static CallExtensionMethodAsInstanceMethodAnalysis Analyze(InvocationExpressionSyntax invocationExpression, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            ArgumentListSyntax argumentList = invocationExpression.ArgumentList;

            if (argumentList == null)
                return Fail;

            SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

            if (!arguments.Any())
                return Fail;

            if (arguments[0].Expression?.IsKind(
                SyntaxKind.IdentifierName,
                SyntaxKind.GenericName,
                SyntaxKind.InvocationExpression,
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxKind.ElementAccessExpression,
                SyntaxKind.ConditionalAccessExpression) != true)
            {
                return Fail;
            }

            if (!semanticModel.TryGetMethodInfo(invocationExpression, out MethodInfo methodInfo, cancellationToken))
                return Fail;

            if (!methodInfo.Symbol.IsNonReducedExtensionMethod())
                return Fail;

            InvocationExpressionSyntax newInvocationExpression = GetNewInvocation(invocationExpression);

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
                case SyntaxKind.MemberBindingExpression:
                    return null;
            }

            Debug.Fail(expression.Kind().ToString());
            return null;
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

            MemberAccessExpressionSyntax newMemberAccess = null;

            switch (expression.Kind())
            {
                case SyntaxKind.IdentifierName:
                case SyntaxKind.GenericName:
                    {
                        ExpressionSyntax newExpression = argument.Expression
                            .WithLeadingTrivia(expression.GetLeadingTrivia())
                            .Parenthesize();

                        newMemberAccess = SimpleMemberAccessExpression(
                            newExpression,
                            (SimpleNameSyntax)expression.WithoutLeadingTrivia());

                        break;
                    }
                case SyntaxKind.SimpleMemberAccessExpression:
                    {
                        var memberAccess = (MemberAccessExpressionSyntax)expression;

                        ExpressionSyntax newExpression = argument.Expression
                            .WithTriviaFrom(memberAccess.Expression)
                            .Parenthesize();

                        newMemberAccess = memberAccess.WithExpression(newExpression);

                        break;
                    }
                default:
                    {
                        Debug.Fail(expression.Kind().ToString());
                        return invocation;
                    }
            }

            return invocation
                .WithExpression(newMemberAccess)
                .WithArgumentList(argumentList.WithArguments(arguments.Remove(argument)));
        }
    }
}