// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.Syntax;
using static Roslynator.SymbolUtility;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp.Analysis
{
    internal static class UseElementAccessAnalysis
    {
        public static bool IsFixableElementAt(
            in SimpleMemberInvocationExpressionInfo invocationInfo,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (invocationInfo.InvocationExpression.IsParentKind(SyntaxKind.ExpressionStatement))
                return false;

            IMethodSymbol methodSymbol = semanticModel.GetReducedExtensionMethodInfo(invocationInfo.InvocationExpression, cancellationToken).Symbol;

            if (methodSymbol == null)
                return false;

            if (!IsLinqElementAt(methodSymbol, allowImmutableArrayExtension: true))
                return false;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(invocationInfo.Expression, cancellationToken);

            return HasAccessibleIndexer(typeSymbol, semanticModel, invocationInfo.InvocationExpression.SpanStart);
        }

        public static bool IsFixableFirst(
            in SimpleMemberInvocationExpressionInfo invocationInfo,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (invocationInfo.InvocationExpression.IsParentKind(SyntaxKind.ExpressionStatement))
                return false;

            IMethodSymbol methodSymbol = semanticModel.GetReducedExtensionMethodInfo(invocationInfo.InvocationExpression, cancellationToken).Symbol;

            if (methodSymbol == null)
                return false;

            if (!IsLinqExtensionOfIEnumerableOfTWithoutParameters(methodSymbol, "First", allowImmutableArrayExtension: true))
                return false;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(invocationInfo.Expression, cancellationToken);

            return HasAccessibleIndexer(typeSymbol, semanticModel, invocationInfo.InvocationExpression.SpanStart);
        }

        public static bool IsFixableLast(
            in SimpleMemberInvocationExpressionInfo invocationInfo,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (invocationInfo.InvocationExpression.IsParentKind(SyntaxKind.ExpressionStatement))
                return false;

            IMethodSymbol methodSymbol = semanticModel.GetReducedExtensionMethodInfo(invocationInfo.InvocationExpression, cancellationToken).Symbol;

            if (methodSymbol == null)
                return false;

            if (!IsLinqExtensionOfIEnumerableOfTWithoutParameters(methodSymbol, "Last", allowImmutableArrayExtension: true))
                return false;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(invocationInfo.Expression, cancellationToken);

            return HasAccessibleIndexer(typeSymbol, semanticModel, invocationInfo.InvocationExpression.SpanStart);
        }
    }
}