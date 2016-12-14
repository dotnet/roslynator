// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    public static class SemanticAnalyzer
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
                && NameEquals(methodSymbol.Name, methodName))
            {
                ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                return parameters.Length == parameterCount
                    && IsContainedInEnumerable(methodSymbol, semanticModel)
                    && parameters[0].Type.IsConstructedFromIEnumerableOfT();
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
                && NameEquals(methodSymbol.Name, methodName))
            {
                ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                return parameters.Length == parameterCount
                    && IsContainedInImmutableArrayExtensions(methodSymbol, semanticModel)
                    && parameters[0].Type.IsConstructedFromImmutableArrayOfT(semanticModel);
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

            if (methodSymbol != null)
            {
                IMethodSymbol reducedFrom = methodSymbol.ReducedFrom;

                if (reducedFrom != null
                    && NameEquals(reducedFrom.Name, "Where"))
                {
                    ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                    return parameters.Length == 1
                        && IsContainedInEnumerable(reducedFrom, semanticModel)
                        && reducedFrom.Parameters.First().Type.IsConstructedFromIEnumerableOfT()
                        && IsPredicateFunc(parameters[0].Type, methodSymbol.TypeArguments[0], semanticModel);
                }
            }

            return false;
        }

        public static bool IsEnumerableWhereMethodWithIndex(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (invocation == null)
                throw new ArgumentNullException(nameof(invocation));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            IMethodSymbol methodSymbol = semanticModel.GetMethodSymbol(invocation, cancellationToken);

            if (methodSymbol != null)
            {
                IMethodSymbol reducedFrom = methodSymbol.ReducedFrom;

                if (reducedFrom != null
                    && NameEquals(reducedFrom.Name, "Where"))
                {
                    ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                    return parameters.Length == 1
                        && IsContainedInEnumerable(reducedFrom, semanticModel)
                        && reducedFrom.Parameters.First().Type.IsConstructedFromIEnumerableOfT()
                        && IsPredicateFunc(
                            parameters[0].Type,
                            methodSymbol.TypeArguments[0],
                            semanticModel.Compilation.GetSpecialType(SpecialType.System_Int32),
                            semanticModel);
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

            if (methodSymbol != null)
            {
                IMethodSymbol reducedFrom = methodSymbol.ReducedFrom;

                if (reducedFrom != null
                    && NameEquals(reducedFrom.Name, "Where"))
                {
                    ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                    return parameters.Length == 1
                        && IsContainedInImmutableArrayExtensions(reducedFrom, semanticModel)
                        && reducedFrom.Parameters.First().Type.IsConstructedFromImmutableArrayOfT(semanticModel)
                        && IsPredicateFunc(parameters[0].Type, methodSymbol.TypeArguments[0], semanticModel);
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

            if (methodSymbol != null)
            {
                IMethodSymbol reducedFrom = methodSymbol.ReducedFrom;

                if (reducedFrom != null
                    && NameEquals(reducedFrom.Name, "Cast"))
                {
                    ImmutableArray<IParameterSymbol> parameters = reducedFrom.Parameters;

                    return parameters.Length == 1
                        && IsContainedInEnumerable(methodSymbol, semanticModel)
                        && parameters[0].Type.IsIEnumerable();
                }
            }

            return false;
        }

        public static bool IsEnumerableElementAtMethod(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (invocation == null)
                throw new ArgumentNullException(nameof(invocation));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            IMethodSymbol methodSymbol = semanticModel.GetMethodSymbol(invocation, cancellationToken);

            if (methodSymbol != null)
            {
                IMethodSymbol reducedFrom = methodSymbol.ReducedFrom;

                if (reducedFrom != null
                    && NameEquals(reducedFrom.Name, "ElementAt"))
                {
                    ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                    return parameters.Length == 1
                        && IsContainedInEnumerable(reducedFrom, semanticModel)
                        && reducedFrom.Parameters.First().Type.IsConstructedFromIEnumerableOfT()
                        && parameters[0].Type.IsInt32();
                }
            }

            return false;
        }

        public static bool IsImmutableArrayElementAtMethod(
            InvocationExpressionSyntax invocation,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (invocation == null)
                throw new ArgumentNullException(nameof(invocation));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            IMethodSymbol methodSymbol = semanticModel.GetMethodSymbol(invocation, cancellationToken);

            if (methodSymbol != null)
            {
                IMethodSymbol reducedFrom = methodSymbol.ReducedFrom;

                if (reducedFrom != null
                    && NameEquals(reducedFrom.Name, "ElementAt"))
                {
                    ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                    return parameters.Length == 1
                        && IsContainedInImmutableArrayExtensions(reducedFrom, semanticModel)
                        && reducedFrom.Parameters.First().Type.IsConstructedFromImmutableArrayOfT(semanticModel)
                        && parameters[0].Type.IsInt32();
                }
            }

            return false;
        }

        public static bool IsFunc(ISymbol symbol, ITypeSymbol parameter1, ITypeSymbol parameter2, SemanticModel semanticModel)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            if (parameter1 == null)
                throw new ArgumentNullException(nameof(parameter1));

            if (parameter2 == null)
                throw new ArgumentNullException(nameof(parameter2));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (symbol.IsNamedType())
            {
                INamedTypeSymbol funcSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_Func_T2);

                var namedTypeSymbol = (INamedTypeSymbol)symbol;

                if (namedTypeSymbol.ConstructedFrom.Equals(funcSymbol))
                {
                    ImmutableArray<ITypeSymbol> typeArguments = namedTypeSymbol.TypeArguments;

                    return typeArguments.Length == 2
                        && typeArguments[0].Equals(parameter1)
                        && typeArguments[1].Equals(parameter2);
                }
            }

            return false;
        }

        public static bool IsFunc(ISymbol symbol, ITypeSymbol parameter1, ITypeSymbol parameter2, ITypeSymbol parameter3, SemanticModel semanticModel)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            if (parameter1 == null)
                throw new ArgumentNullException(nameof(parameter1));

            if (parameter2 == null)
                throw new ArgumentNullException(nameof(parameter2));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (symbol.IsNamedType())
            {
                INamedTypeSymbol funcSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_Func_T3);

                var namedTypeSymbol = (INamedTypeSymbol)symbol;

                if (namedTypeSymbol.ConstructedFrom.Equals(funcSymbol))
                {
                    ImmutableArray<ITypeSymbol> typeArguments = namedTypeSymbol.TypeArguments;

                    return typeArguments.Length == 3
                        && typeArguments[0].Equals(parameter1)
                        && typeArguments[1].Equals(parameter2)
                        && typeArguments[2].Equals(parameter3);
                }
            }

            return false;
        }

        public static bool IsPredicateFunc(ISymbol symbol, ITypeSymbol parameter, SemanticModel semanticModel)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (symbol.IsNamedType())
            {
                INamedTypeSymbol funcSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_Func_T2);

                var namedTypeSymbol = (INamedTypeSymbol)symbol;

                if (namedTypeSymbol.ConstructedFrom.Equals(funcSymbol))
                {
                    ImmutableArray<ITypeSymbol> typeArguments = namedTypeSymbol.TypeArguments;

                    return typeArguments.Length == 2
                        && typeArguments[0].Equals(parameter)
                        && typeArguments[1].IsBoolean();
                }
            }

            return false;
        }

        public static bool IsPredicateFunc(ISymbol symbol, ITypeSymbol parameter1, ITypeSymbol parameter2, SemanticModel semanticModel)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            if (parameter1 == null)
                throw new ArgumentNullException(nameof(parameter1));

            if (parameter2 == null)
                throw new ArgumentNullException(nameof(parameter2));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (symbol.IsNamedType())
            {
                INamedTypeSymbol funcSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_Func_T3);

                var namedTypeSymbol = (INamedTypeSymbol)symbol;

                if (namedTypeSymbol.ConstructedFrom.Equals(funcSymbol))
                {
                    ImmutableArray<ITypeSymbol> typeArguments = namedTypeSymbol.TypeArguments;

                    return typeArguments.Length == 3
                        && typeArguments[0].Equals(parameter1)
                        && typeArguments[1].Equals(parameter2)
                        && typeArguments[2].IsBoolean();
                }
            }

            return false;
        }

        public static bool IsContainedInEnumerable(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            if (methodSymbol == null)
                throw new ArgumentNullException(nameof(methodSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return methodSymbol.ContainingType?.Equals(semanticModel.GetTypeByMetadataName(MetadataNames.System_Linq_Enumerable)) == true;
        }

        public static bool IsContainedInImmutableArrayExtensions(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            if (methodSymbol == null)
                throw new ArgumentNullException(nameof(methodSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return methodSymbol.ContainingType?.Equals(semanticModel.GetTypeByMetadataName(MetadataNames.System_Linq_ImmutableArrayExtensions)) == true;
        }

        private static bool NameEquals(string name1, string name2)
        {
            return string.Equals(name1, name2, StringComparison.Ordinal);
        }
    }
}
