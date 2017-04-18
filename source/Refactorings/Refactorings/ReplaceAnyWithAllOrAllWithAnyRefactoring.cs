// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceAnyWithAllOrAllWithAnyRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, InvocationExpressionSyntax invocation)
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            if (!ComputeRefactoring(context, invocation, semanticModel, "Any", "All"))
                ComputeRefactoring(context, invocation, semanticModel, "All", "Any");
        }

        private static bool ComputeRefactoring(
            RefactoringContext context,
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel,
            string fromMethodName,
            string toMethodName)
        {
            if (semanticModel
                .GetExtensionMethodInfo(invocation, context.CancellationToken)
                .MethodInfo
                .IsLinqExtensionOfIEnumerableOfTWithPredicate(fromMethodName))
            {
                ExpressionSyntax expression = GetExpression(invocation);

                if (expression != null)
                {
                    context.RegisterRefactoring(
                        $"Replace '{fromMethodName}' with '{toMethodName}'",
                        cancellationToken =>
                        {
                            return RefactorAsync(
                                context.Document,
                                invocation,
                                toMethodName,
                                expression,
                                cancellationToken);
                        });

                    return true;
                }
            }

            return false;
        }

        private static ExpressionSyntax GetExpression(InvocationExpressionSyntax invocation)
        {
            ArgumentSyntax argument = invocation
                .ArgumentList?
                .Arguments
                .Last();

            switch (argument?.Expression?.Kind())
            {
                case SyntaxKind.SimpleLambdaExpression:
                    {
                        return ((SimpleLambdaExpressionSyntax)argument.Expression).Body as ExpressionSyntax;
                    }
                case SyntaxKind.ParenthesizedLambdaExpression:
                    {
                        return ((ParenthesizedLambdaExpressionSyntax)argument.Expression).Body as ExpressionSyntax;
                    }
            }

            return null;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocationExpression,
            string memberName,
            ExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var memberAccessExpression = (MemberAccessExpressionSyntax)invocationExpression.Expression;

            MemberAccessExpressionSyntax newMemberAccessExpression = memberAccessExpression
                .WithName(SyntaxFactory.IdentifierName(memberName).WithTriviaFrom(memberAccessExpression.Name));

            InvocationExpressionSyntax newNode = invocationExpression
                .ReplaceNode(expression, Negator.LogicallyNegate(expression))
                .WithExpression(newMemberAccessExpression);

            return document.ReplaceNodeAsync(invocationExpression, newNode, cancellationToken);
        }
    }
}
