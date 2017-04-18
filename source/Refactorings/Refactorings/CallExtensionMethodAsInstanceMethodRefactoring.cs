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
                SyntaxNodeOrToken nodeOrToken = GetNodeOrToken(expression);

                if (nodeOrToken.Span.Contains(context.Span))
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    ExtensionMethodInfo info = semanticModel.GetExtensionMethodInfo(invocation, ExtensionMethodKind.Ordinary, context.CancellationToken);

                    if (info.IsValid
                        && invocation.ArgumentList?.Arguments.Any() == true)
                    {
                        InvocationExpressionSyntax newInvocation = GetNewInvocation(invocation);

                        if (semanticModel
                            .GetSpeculativeMethodSymbol(invocation.SpanStart, newInvocation)?
                            .ReducedFromOrSelf()
                            .Equals(info.Symbol.ConstructedFrom) == true)
                        {
                            context.RegisterRefactoring(
                                "Call extension method as instance method",
                                cancellationToken =>
                                {
                                    return RefactorAsync(
                                        context.Document,
                                        invocation,
                                        newInvocation,
                                        context.CancellationToken);
                                });
                        }
                    }
                }
            }
        }

        private static SyntaxNodeOrToken GetNodeOrToken(ExpressionSyntax expression)
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
                default:
                    {
                        Debug.Assert(false, expression.Kind().ToString());
                        return null;
                    }
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            InvocationExpressionSyntax newInvocation,
            CancellationToken cancellationToken)
        {
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
                        return invocation;
                    }
            }

            return invocation
                .WithExpression(newMemberAccess)
                .WithArgumentList(argumentList.WithArguments(arguments.Remove(argument)));
        }
    }
}