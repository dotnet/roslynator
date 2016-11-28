// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CallExtensionMethodAsInstanceMethodRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, InvocationExpressionSyntax invocation)
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            IMethodSymbol methodSymbol = semanticModel.GetMethodSymbol(invocation, context.CancellationToken);

            if (methodSymbol?.IsExtensionMethod == true
                && methodSymbol.MethodKind == MethodKind.Ordinary
                && invocation.Expression != null
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

        private static async Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            ArgumentListSyntax argumentList = invocation.ArgumentList;

            SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

            ArgumentSyntax argument = arguments.First();

            MemberAccessExpressionSyntax newMemberAccess = memberAccess
                .WithExpression(argument.Expression.WithTriviaFrom(memberAccess.Expression));

            InvocationExpressionSyntax newInvocation = invocation
                .WithExpression(newMemberAccess)
                .WithArgumentList(argumentList.WithArguments(arguments.Remove(argument)));

            SyntaxNode newRoot = root.ReplaceNode(invocation, newInvocation);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}