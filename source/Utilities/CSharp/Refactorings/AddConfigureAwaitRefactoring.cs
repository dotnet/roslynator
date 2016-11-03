// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    public static class AddConfigureAwaitRefactoring
    {
        public static bool CanRefactor(
            AwaitExpressionSyntax awaitExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (awaitExpression == null)
                throw new ArgumentNullException(nameof(awaitExpression));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (awaitExpression.Expression?.IsKind(SyntaxKind.InvocationExpression) == true
                && ReturnsTask((InvocationExpressionSyntax)awaitExpression.Expression, semanticModel, cancellationToken))
            {
                ISymbol symbol = semanticModel
                    .GetSymbolInfo(awaitExpression.Expression, cancellationToken)
                    .Symbol;

                if (symbol?.IsErrorType() == false)
                {
                    INamedTypeSymbol configuredTaskSymbol = semanticModel
                        .Compilation
                        .GetTypeByMetadataName("System.Runtime.CompilerServices.ConfiguredTaskAwaitable`1");

                    if (configuredTaskSymbol != null
                        && !symbol.Equals(configuredTaskSymbol))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            AwaitExpressionSyntax awaitExpressionSyntax,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (awaitExpressionSyntax == null)
                throw new ArgumentNullException(nameof(awaitExpressionSyntax));

            SyntaxNode root = await document
                .GetSyntaxRootAsync(cancellationToken)
                .ConfigureAwait(false);

            var invocation = (InvocationExpressionSyntax)awaitExpressionSyntax.Expression;

            InvocationExpressionSyntax newInvocation = InvocationExpression(
                SimpleMemberAccessExpression(invocation.WithoutTrailingTrivia(), IdentifierName("ConfigureAwait")),
                ArgumentList(Argument(FalseLiteralExpression())));

            newInvocation = newInvocation.WithTrailingTrivia(invocation.GetTrailingTrivia());

            root = root.ReplaceNode(invocation, newInvocation);

            return document.WithSyntaxRoot(root);
        }

        private static bool ReturnsTask(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            INamedTypeSymbol taskSymbol = semanticModel
                .Compilation
                .GetTypeByMetadataName("System.Threading.Tasks.Task");

            if (taskSymbol != null)
            {
                ISymbol symbol = semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol;

                if (symbol?.IsMethod() == true)
                {
                    var methodSymbol = (IMethodSymbol)symbol;

                    if (methodSymbol.ReturnType.Equals(taskSymbol))
                        return true;

                    if (methodSymbol.ReturnType.IsNamedType()
                        && ((INamedTypeSymbol)methodSymbol.ReturnType)
                            .ConstructedFrom
                            .BaseTypes()
                            .Any(baseType => baseType.Equals(taskSymbol)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
