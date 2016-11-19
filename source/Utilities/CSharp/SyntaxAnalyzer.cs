// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    public static class SyntaxAnalyzer
    {
        public static bool IsEnumerableExtensionOrImmutableArrayExtensionMethod(
            InvocationExpressionSyntax invocation,
            string methodName,
            int parameterCount,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return IsEnumerableExtensionMethod(invocation, methodName, parameterCount, semanticModel, cancellationToken)
                || IsImmutableArrayExtensionMethod(invocation, methodName, parameterCount, semanticModel, cancellationToken);
        }

        public static bool IsEnumerableExtensionMethod(
            InvocationExpressionSyntax invocation,
            string methodName,
            int parameterCount,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (invocation == null)
                throw new ArgumentNullException(nameof(invocation));

            if (methodName == null)
                throw new ArgumentNullException(nameof(methodName));

            if (parameterCount < 1)
                throw new ArgumentOutOfRangeException(nameof(parameterCount), parameterCount, null);

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            IMethodSymbol methodSymbol = semanticModel.GetMethodSymbol(invocation, cancellationToken);

            methodSymbol = methodSymbol?.ReducedFrom;

            if (methodSymbol != null
                && NameEquals(methodSymbol.MetadataName, methodName))
            {
                ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                if (parameters.Length == parameterCount
                    && IsContainedInEnumerable(methodSymbol, semanticModel)
                    && IsGenericIEnumerable(parameters[0].Type))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsImmutableArrayExtensionMethod(
            InvocationExpressionSyntax invocation,
            string methodName,
            int parameterCount,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (invocation == null)
                throw new ArgumentNullException(nameof(invocation));

            if (methodName == null)
                throw new ArgumentNullException(nameof(methodName));

            if (parameterCount < 1)
                throw new ArgumentOutOfRangeException(nameof(parameterCount), parameterCount, null);

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            IMethodSymbol methodSymbol = semanticModel.GetMethodSymbol(invocation, cancellationToken);

            methodSymbol = methodSymbol?.ReducedFrom;

            if (methodSymbol != null
                && NameEquals(methodSymbol.MetadataName, methodName))
            {
                ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                if (parameters.Length == parameterCount
                    && IsContainedInImmutableArrayExtensions(methodSymbol, semanticModel)
                    && IsGenericImmutableArray(parameters[0].Type, semanticModel))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsEnumerableWhereOrImmutableArrayWhereMethod(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return IsEnumerableWhereMethod(invocation, semanticModel, cancellationToken)
                || IsImmutableArrayWhereMethod(invocation, semanticModel, cancellationToken);
        }

        public static bool IsEnumerableWhereMethod(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (invocation == null)
                throw new ArgumentNullException(nameof(invocation));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            IMethodSymbol methodSymbol = semanticModel.GetMethodSymbol(invocation, cancellationToken);

            methodSymbol = methodSymbol?.ReducedFrom;

            if (methodSymbol != null
                && NameEquals(methodSymbol.MetadataName, "Where"))
            {
                ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                if (parameters.Length == 2
                    && IsContainedInEnumerable(methodSymbol, semanticModel)
                    && IsGenericIEnumerable(parameters[0].Type)
                    && IsPredicate(parameters[1].Type, semanticModel))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsImmutableArrayWhereMethod(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (invocation == null)
                throw new ArgumentNullException(nameof(invocation));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            IMethodSymbol methodSymbol = semanticModel.GetMethodSymbol(invocation, cancellationToken);

            methodSymbol = methodSymbol?.ReducedFrom;

            if (methodSymbol != null
                && NameEquals(methodSymbol.MetadataName, "Where"))
            {
                ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                if (parameters.Length == 2
                    && IsContainedInImmutableArrayExtensions(methodSymbol, semanticModel)
                    && IsGenericImmutableArray(parameters[0].Type, semanticModel)
                    && IsPredicate(parameters[1].Type, semanticModel))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsEnumerableCastMethod(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (invocation == null)
                throw new ArgumentNullException(nameof(invocation));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            IMethodSymbol methodSymbol = semanticModel.GetMethodSymbol(invocation, cancellationToken);

            methodSymbol = methodSymbol?.ReducedFrom;

            if (methodSymbol != null
                && NameEquals(methodSymbol.MetadataName, "Cast"))
            {
                ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                if (parameters.Length == 1
                    && IsContainedInEnumerable(methodSymbol, semanticModel)
                    && IsIEnumerable(parameters[0].Type))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsIEnumerable(ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            return typeSymbol.IsNamedType()
                && ((INamedTypeSymbol)typeSymbol).ConstructedFrom.SpecialType == SpecialType.System_Collections_IEnumerable;
        }

        public static bool IsGenericIEnumerable(ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            return typeSymbol.IsNamedType()
                && ((INamedTypeSymbol)typeSymbol).ConstructedFrom.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T;
        }

        public static bool IsGenericImmutableArray(ISymbol symbol, SemanticModel semanticModel)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (symbol.IsNamedType())
            {
                INamedTypeSymbol immutableArraySymbol = semanticModel
                    .Compilation
                    .GetTypeByMetadataName("System.Collections.Immutable.ImmutableArray`1");

                return immutableArraySymbol != null
                    && ((INamedTypeSymbol)symbol).ConstructedFrom.Equals(immutableArraySymbol);
            }

            return false;
        }

        private static bool IsPredicate(ISymbol symbol, SemanticModel semanticModel)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (symbol.IsNamedType())
            {
                INamedTypeSymbol func2Symbol = semanticModel.Compilation.GetTypeByMetadataName("System.Func`2");

                return ((INamedTypeSymbol)symbol).ConstructedFrom.Equals(func2Symbol);
            }

            return false;
        }

        private static bool IsContainedInEnumerable(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            return methodSymbol.ContainingType?.Equals(semanticModel.Compilation.GetTypeByMetadataName("System.Linq.Enumerable")) == true;
        }

        private static bool IsContainedInImmutableArrayExtensions(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            return methodSymbol.ContainingType?.Equals(semanticModel.Compilation.GetTypeByMetadataName("System.Linq.ImmutableArrayExtensions")) == true;
        }

        private static bool NameEquals(string name1, string name2)
        {
            return string.Equals(name1, name2, StringComparison.Ordinal);
        }
    }
}
