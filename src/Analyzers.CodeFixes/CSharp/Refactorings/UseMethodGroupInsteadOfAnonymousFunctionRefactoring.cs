// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseMethodGroupInsteadOfAnonymousFunctionRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            AnonymousFunctionExpressionSyntax anonymousFunction,
            CancellationToken cancellationToken)
        {
            InvocationExpressionSyntax invocationExpression = UseMethodGroupInsteadOfAnonymousFunctionAnalyzer.GetInvocationExpression(anonymousFunction.Body);

            ExpressionSyntax newNode = invocationExpression.Expression;

            SemanticModel semanticModel = await document.GetSemanticModelAsync().ConfigureAwait(false);

            var methodSymbol = (IMethodSymbol)semanticModel.GetSymbol(invocationExpression, cancellationToken);

            if (methodSymbol.IsReducedExtensionMethod())
                newNode = ((MemberAccessExpressionSyntax)newNode).Name;

            newNode = newNode.WithTriviaFrom(anonymousFunction);

            return await document.ReplaceNodeAsync(anonymousFunction, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
