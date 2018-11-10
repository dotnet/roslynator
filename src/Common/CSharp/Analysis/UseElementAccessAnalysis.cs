// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;
using static Roslynator.SymbolUtility;

namespace Roslynator.CSharp.Analysis
{
    internal static class UseElementAccessAnalysis
    {
        public static bool IsFixableElementAt(
            in SimpleMemberInvocationExpressionInfo invocationInfo,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            InvocationExpressionSyntax invocationExpression = invocationInfo.InvocationExpression;

            if (invocationExpression.IsParentKind(SyntaxKind.ExpressionStatement))
                return false;

            IMethodSymbol methodSymbol = semanticModel.GetReducedExtensionMethodInfo(invocationExpression, cancellationToken).Symbol;

            if (methodSymbol == null)
                return false;

            if (!IsLinqElementAt(methodSymbol, allowImmutableArrayExtension: true))
                return false;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(invocationInfo.Expression, cancellationToken);

            if (!HasAccessibleIndexer(typeSymbol, semanticModel, invocationExpression.SpanStart))
                return false;

            ElementAccessExpressionSyntax elementAccess = SyntaxFactory.ElementAccessExpression(
                invocationInfo.Expression,
                CSharpFactory.BracketedArgumentList(invocationInfo.Arguments[0]));

            SymbolInfo symbolInfo = semanticModel.GetSpeculativeSymbolInfo(invocationExpression.SpanStart, elementAccess, SpeculativeBindingOption.BindAsExpression);

            return !(symbolInfo.Symbol is IPropertySymbol propertySymbol)
                || !propertySymbol.IsIndexer
                || CheckInfiniteRecursion(propertySymbol, invocationExpression.SpanStart, semanticModel, cancellationToken);
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

        private static bool CheckInfiniteRecursion(
        IPropertySymbol indexerSymbol,
            int position,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ISymbol symbol = semanticModel.GetEnclosingSymbol(position, cancellationToken);

            if (symbol != null)
            {
                IPropertySymbol propertySymbol = null;

                if (symbol.Kind == SymbolKind.Property)
                {
                    propertySymbol = (IPropertySymbol)symbol;
                }
                else if (symbol.Kind == SymbolKind.Method)
                {
                    var methodSymbol = (IMethodSymbol)symbol;

                    if (methodSymbol.MethodKind.Is(MethodKind.PropertyGet, MethodKind.PropertySet))
                        propertySymbol = methodSymbol.AssociatedSymbol as IPropertySymbol;
                }

                if (propertySymbol == indexerSymbol)
                    return false;
            }

            return true;
        }
    }
}