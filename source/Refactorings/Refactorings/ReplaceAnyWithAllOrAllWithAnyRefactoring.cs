// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceAnyWithAllOrAllWithAnyRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, InvocationExpressionSyntax invocationExpression)
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            var methodSymbol = semanticModel.GetSymbolInfo(invocationExpression, context.CancellationToken).Symbol as IMethodSymbol;

            if (methodSymbol == null)
                return;

            INamedTypeSymbol enumerable = semanticModel.Compilation.GetTypeByMetadataName("System.Linq.Enumerable");

            if (enumerable == null)
                return;

            int argumentIndex = (methodSymbol.ReducedFrom != null) ? 0 : 1;

            methodSymbol = methodSymbol.ReducedFrom ?? methodSymbol.ConstructedFrom;

            if (methodSymbol.Equals(GetMethod(enumerable, "Any")))
            {
                ExpressionSyntax expression = GetExpression(invocationExpression, argumentIndex);

                if (expression != null)
                {
                    context.RegisterRefactoring(
                        "Replace 'Any' with 'All'",
                        cancellationToken =>
                        {
                            return RefactorAsync(
                                context.Document,
                                invocationExpression,
                                "All",
                                expression,
                                cancellationToken);
                        });
                }
            }
            else if (methodSymbol.Equals(GetMethod(enumerable, "All")))
            {
                ExpressionSyntax expression = GetExpression(invocationExpression, argumentIndex);

                if (expression != null)
                {
                    context.RegisterRefactoring(
                        "Replace 'All' with 'Any'",
                        cancellationToken =>
                        {
                            return RefactorAsync(
                                context.Document,
                                invocationExpression,
                                "Any",
                                expression,
                                cancellationToken);
                        });
                }
            }
        }

        private static ISymbol GetMethod(INamedTypeSymbol enumerable, string methodName)
        {
            return enumerable
                .GetMembers(methodName)
                .Where(f => f.IsMethod())
                .FirstOrDefault(f => ((IMethodSymbol)f).Parameters.Length == 2);
        }

        private static ExpressionSyntax GetExpression(InvocationExpressionSyntax invocationExpression, int argumentIndex)
        {
            ArgumentSyntax argument = invocationExpression
                .ArgumentList?
                .Arguments
                .ElementAtOrDefault(argumentIndex);

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

        public static async Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocationExpression,
            string memberName,
            ExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var memberAccessExpression = (MemberAccessExpressionSyntax)invocationExpression.Expression;

            MemberAccessExpressionSyntax newMemberAccessExpression = memberAccessExpression
                .WithName(SyntaxFactory.IdentifierName(memberName).WithTriviaFrom(memberAccessExpression.Name));

            InvocationExpressionSyntax newNode = invocationExpression
                .ReplaceNode(expression, expression.Negate())
                .WithExpression(newMemberAccessExpression);

            SyntaxNode newRoot = oldRoot.ReplaceNode(invocationExpression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
