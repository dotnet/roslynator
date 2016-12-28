// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    public static class CallConfigureAwaitRefactoring
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

            ExpressionSyntax expression = awaitExpression.Expression;

            if (expression?.IsKind(SyntaxKind.InvocationExpression) == true)
            {
                IMethodSymbol methodSymbol = semanticModel.GetMethodSymbol(expression, cancellationToken);

                return methodSymbol != null
                    && Symbol.IsTaskOrDerivedFromTask(methodSymbol.ReturnType, semanticModel)
                    && semanticModel.GetTypeByMetadataName(MetadataNames.System_Runtime_CompilerServices_ConfiguredTaskAwaitable_T) != null;
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            AwaitExpressionSyntax awaitExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (awaitExpression == null)
                throw new ArgumentNullException(nameof(awaitExpression));

            var invocation = (InvocationExpressionSyntax)awaitExpression.Expression;

            InvocationExpressionSyntax newInvocation = InvocationExpression(
                SimpleMemberAccessExpression(invocation.WithoutTrailingTrivia(), IdentifierName("ConfigureAwait")),
                ArgumentList(Argument(FalseLiteralExpression())));

            newInvocation = newInvocation.WithTrailingTrivia(invocation.GetTrailingTrivia());

            return await document.ReplaceNodeAsync(invocation, newInvocation, cancellationToken).ConfigureAwait(false);
        }
    }
}
