// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    internal static class UseElementAccessInsteadOfElementAtAnalysis
    {
        public static bool IsFixable(
            SimpleMemberInvocationExpressionInfo invocationInfo,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax argumentExpression = invocationInfo.Arguments[0].Expression;

            if (argumentExpression?.IsMissing != false)
                return false;

            if (invocationInfo.Expression?.IsMissing != false)
                return false;

            IMethodSymbol methodSymbol = semanticModel.GetReducedExtensionMethodInfo(invocationInfo.InvocationExpression, cancellationToken).Symbol;

            if (methodSymbol == null)
                return false;

            if (!SymbolUtility.IsLinqElementAt(methodSymbol, semanticModel, allowImmutableArrayExtension: true))
                return false;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(invocationInfo.Expression, cancellationToken);

            return SymbolUtility.HasAccessibleIndexer(typeSymbol, semanticModel, invocationInfo.InvocationExpression.SpanStart);
        }
    }
}