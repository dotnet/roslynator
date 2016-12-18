// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    public static class SymbolAnalyzer
    {
        public static bool IsEnumerableMethod(
            IMethodSymbol methodSymbol,
            string methodName,
            SemanticModel semanticModel,
            Func<IMethodSymbol, bool> validate = null)
        {
            if (methodSymbol == null)
                throw new ArgumentNullException(nameof(methodSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return methodSymbol.IsExtensionMethod
                && NameEquals(methodSymbol.Name, methodName)
                && IsEnumerableMethod(methodSymbol, semanticModel)
                && validate?.Invoke(methodSymbol) != false;
        }

        public static bool IsImmutableArrayExtensionMethod(
            IMethodSymbol methodSymbol,
            string methodName,
            SemanticModel semanticModel,
            Func<IMethodSymbol, bool> validate = null)
        {
            if (methodSymbol == null)
                throw new ArgumentNullException(nameof(methodSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return methodSymbol.IsExtensionMethod
                && NameEquals(methodSymbol.Name, methodName)
                && IsImmutableArrayExtensionMethod(methodSymbol, semanticModel)
                && validate?.Invoke(methodSymbol) != false;
        }

        public static bool IsEnumerableOrImmutableArrayExtensionMethod(
            IMethodSymbol methodSymbol,
            string methodName,
            SemanticModel semanticModel,
            Func<IMethodSymbol, bool> validate = null)
        {
            if (methodSymbol == null)
                throw new ArgumentNullException(nameof(methodSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return methodSymbol.IsExtensionMethod
                && NameEquals(methodSymbol.Name, methodName)
                && (IsEnumerableMethod(methodSymbol, semanticModel) || IsImmutableArrayExtensionMethod(methodSymbol, semanticModel))
                && validate?.Invoke(methodSymbol) != false;
        }

        private static bool IsEnumerableMethod(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            if (IsContainedInEnumerable(methodSymbol, semanticModel))
            {
                IMethodSymbol reducedFrom = methodSymbol.ReducedFrom;

                IParameterSymbol parameter = (reducedFrom != null)
                    ? reducedFrom.Parameters.First()
                    : methodSymbol.Parameters.First();

                return parameter.Type.IsConstructedFromIEnumerableOfT();
            }
            else
            {
                return false;
            }
        }

        private static bool IsImmutableArrayExtensionMethod(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            if (IsContainedInImmutableArrayExtensions(methodSymbol, semanticModel))
            {
                IMethodSymbol reducedFrom = methodSymbol.ReducedFrom;

                IParameterSymbol parameter = (reducedFrom != null)
                    ? reducedFrom.Parameters.First()
                    : methodSymbol.Parameters.First();

                return parameter.Type.IsConstructedFromImmutableArrayOfT(semanticModel);
            }
            else
            {
                return false;
            }
        }

        public static bool IsEnumerableMethodWithoutParameters(IMethodSymbol methodSymbol, string methodName, SemanticModel semanticModel)
        {
            return IsEnumerableMethod(
                methodSymbol,
                methodName,
                semanticModel,
                f => !f.Parameters.Any());
        }

        public static bool IsEnumerableOrImmutableArrayExtensionMethodWithoutParameters(IMethodSymbol methodSymbol, string methodName, SemanticModel semanticModel)
        {
            return IsEnumerableOrImmutableArrayExtensionMethod(
                methodSymbol,
                methodName,
                semanticModel,
                f => !f.Parameters.Any());
        }

        public static bool IsEnumerableMethodWithPredicate(IMethodSymbol methodSymbol, string methodName, SemanticModel semanticModel)
        {
            return IsEnumerableMethod(
                methodSymbol,
                methodName,
                semanticModel,
                f => f.Parameters.Length == 1 && IsPredicateFunc(f.Parameters[0].Type, f.TypeArguments[0], semanticModel));
        }

        public static bool IsEnumerableMethodWithPredicateWithIndex(IMethodSymbol methodSymbol, string methodName, SemanticModel semanticModel)
        {
            return IsEnumerableMethod(
                methodSymbol,
                methodName,
                semanticModel,
                f => f.Parameters.Length == 1 && IsPredicateFunc(f.Parameters[0].Type, f.TypeArguments[0], semanticModel.Compilation.GetSpecialType(SpecialType.System_Int32), semanticModel));
        }

        public static bool IsEnumerableCastMethod(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            if (methodSymbol == null)
                throw new ArgumentNullException(nameof(methodSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (methodSymbol.IsExtensionMethod
                && NameEquals(methodSymbol.Name, "Cast")
                && IsContainedInEnumerable(methodSymbol, semanticModel))
            {
                IMethodSymbol reducedFrom = methodSymbol.ReducedFrom;

                if (reducedFrom != null)
                    methodSymbol = reducedFrom;

                return methodSymbol.Parameters.Length == 1
                    && methodSymbol.Parameters[0].Type.IsIEnumerable();
            }

            return false;
        }

        public static bool IsEnumerableWhereMethod(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            return IsEnumerableMethodWithPredicate(methodSymbol, "Where", semanticModel);
        }

        public static bool IsEnumerableOrImmutableArrayExtensionWhereMethod(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            return IsEnumerableOrImmutableArrayExtensionMethod(
                methodSymbol,
                "Where",
                semanticModel,
                f => f.Parameters.Length == 1 && IsPredicateFunc(f.Parameters[0].Type, f.TypeArguments[0], semanticModel));
        }

        public static bool IsEnumerableOrImmutableArrayExtensionSelectMethod(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            return IsEnumerableOrImmutableArrayExtensionMethod(
                methodSymbol,
                "Select",
                semanticModel,
                f => f.Parameters.Length == 1 && IsFunc(f.Parameters[0].Type, f.TypeArguments[0], f.TypeArguments[1], semanticModel));
        }

        public static bool IsEnumerableOrImmutableArrayExtensionElementAtMethod(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            return IsEnumerableOrImmutableArrayExtensionMethod(
                methodSymbol,
                "ElementAt",
                semanticModel,
                f => f.Parameters.Length == 1 && f.Parameters[0].Type.IsInt32());
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
            return IsContainedIn(methodSymbol, MetadataNames.System_Linq_Enumerable, semanticModel);
        }

        public static bool IsContainedInImmutableArrayExtensions(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            return IsContainedIn(methodSymbol, MetadataNames.System_Linq_ImmutableArrayExtensions, semanticModel);
        }

        private static bool IsContainedIn(IMethodSymbol methodSymbol, string fullyQualifiedMetadataName, SemanticModel semanticModel)
        {
            if (methodSymbol == null)
                throw new ArgumentNullException(nameof(methodSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return methodSymbol
                .ContainingType?
                .Equals(semanticModel.GetTypeByMetadataName(fullyQualifiedMetadataName)) == true;
        }

        private static bool NameEquals(string name1, string name2)
        {
            return string.Equals(name1, name2, StringComparison.Ordinal);
        }
    }
}
