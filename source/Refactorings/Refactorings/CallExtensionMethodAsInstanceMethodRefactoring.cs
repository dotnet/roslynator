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
    internal static class CallExtensionMethodAsInstanceMethodRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, InvocationExpressionSyntax invocation)
        {
            ExpressionSyntax expression = invocation.Expression;

            if (expression != null)
            {
                SimpleNameSyntax name = GetSimpleName(expression);

                if (name?.Span.Contains(context.Span) == true)
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    IMethodSymbol methodSymbol = semanticModel.GetMethodSymbol(invocation, context.CancellationToken);

                    if (methodSymbol?.IsExtensionMethod == true
                        && methodSymbol.MethodKind == MethodKind.Ordinary
                        && invocation.ArgumentList?.Arguments.Any() == true)
                    {
                        context.RegisterRefactoring(
                            "Call extension method as instance method",
                            cancellationToken =>
                            {
                                return RefactorAsync(
                                    context.Document,
                                    invocation,
                                    context.CancellationToken);
                            });
                    }
                }
            }
        }

        private static SimpleNameSyntax GetSimpleName(ExpressionSyntax expression)
        {
            switch (expression.Kind())
            {
                case SyntaxKind.IdentifierName:
                    return (SimpleNameSyntax)expression;
                case SyntaxKind.SimpleMemberAccessExpression:
                    return ((MemberAccessExpressionSyntax)expression).Name;
                default:
                    Debug.Assert(false, expression.Kind().ToString());
                    return null;
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax expression = invocation.Expression;
            ArgumentListSyntax argumentList = invocation.ArgumentList;
            SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;
            ArgumentSyntax argument = arguments.First();

            MemberAccessExpressionSyntax newMemberAccess = null;

            switch (expression.Kind())
            {
                case SyntaxKind.IdentifierName:
                    {
                        newMemberAccess = SimpleMemberAccessExpression(
                            argument.Expression.WithLeadingTrivia(expression.GetLeadingTrivia()),
                            (SimpleNameSyntax)expression.WithoutLeadingTrivia());

                        break;
                    }
                case SyntaxKind.SimpleMemberAccessExpression:
                    {
                        var memberAccess = (MemberAccessExpressionSyntax)expression;

                        newMemberAccess = memberAccess
                            .WithExpression(argument.Expression.WithTriviaFrom(memberAccess.Expression));

                        break;
                    }
                default:
                    {
                        Debug.Assert(false, expression.Kind().ToString());
                        return document;
                    }
            }

            InvocationExpressionSyntax newInvocation = invocation
                .WithExpression(newMemberAccess)
                .WithArgumentList(argumentList.WithArguments(arguments.Remove(argument)));

            return await document.ReplaceNodeAsync(invocation, newInvocation, cancellationToken).ConfigureAwait(false);
        }
    }
}