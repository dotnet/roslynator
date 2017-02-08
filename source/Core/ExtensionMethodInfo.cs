// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Roslynator.Extensions;

namespace Roslynator
{
    public struct ExtensionMethodInfo
    {
        private ExtensionMethodInfo(IMethodSymbol originalSymbol, IMethodSymbol symbol, SemanticModel semanticModel)
            : this()
        {
            OriginalSymbol = originalSymbol;
            Symbol = symbol;
            SemanticModel = semanticModel;
        }

        public bool IsValid
        {
            get { return Symbol != null; }
        }

        public bool IsFromReduced
        {
            get { return IsValid && !object.ReferenceEquals(OriginalSymbol, Symbol); }
        }

        public bool IsFromOrdinary
        {
            get { return IsValid && object.ReferenceEquals(OriginalSymbol, Symbol); }
        }

        public string Name
        {
            get { return Symbol?.Name; }
        }

        private ImmutableArray<IParameterSymbol> Parameters
        {
            get { return (IsValid) ? Symbol.Parameters : ImmutableArray<IParameterSymbol>.Empty; }
        }

        public IMethodSymbol Symbol { get; }

        private IMethodSymbol OriginalSymbol { get; }

        private SemanticModel SemanticModel { get; }

        public static ExtensionMethodInfo FromOrdinary(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            return Create(methodSymbol, semanticModel, ExtensionMethodKind.Ordinary);
        }

        public static ExtensionMethodInfo FromReduced(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            return Create(methodSymbol, semanticModel, ExtensionMethodKind.Reduced);
        }

        public static ExtensionMethodInfo FromOrdinaryOrReduced(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            return Create(methodSymbol, semanticModel, ExtensionMethodKind.OrdinaryOrReduced);
        }

        internal static ExtensionMethodInfo Create(IMethodSymbol methodSymbol, SemanticModel semanticModel, ExtensionMethodKind allowedKinds)
        {
            if (methodSymbol == null)
                throw new ArgumentNullException(nameof(methodSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (methodSymbol.IsExtensionMethod)
            {
                IMethodSymbol reducedFrom = methodSymbol.ReducedFrom;

                if (reducedFrom != null)
                {
                    if ((allowedKinds & ExtensionMethodKind.Reduced) != 0)
                        return new ExtensionMethodInfo(methodSymbol, reducedFrom, semanticModel);
                }
                else if ((allowedKinds & ExtensionMethodKind.Ordinary) != 0)
                {
                    return new ExtensionMethodInfo(methodSymbol, methodSymbol, semanticModel);
                }
            }

            return default(ExtensionMethodInfo);
        }

        public bool IsLinqExtensionOfIEnumerableOfTWithoutParameters(
            string name,
            bool allowImmutableArrayExtension = false)
        {
            return IsLinqExtensionOfIEnumerableOfT(name, parameterCount: 1, allowImmutableArrayExtension: allowImmutableArrayExtension);
        }

        public bool IsLinqElementAt(
            bool allowImmutableArrayExtension = false)
        {
            return IsLinqExtensionOfIEnumerableOfT("ElementAt", parameterCount: 2, allowImmutableArrayExtension: allowImmutableArrayExtension)
                && Parameters[1].Type.IsInt32();
        }

        public bool IsLinqWhere(
            bool allowImmutableArrayExtension = false)
        {
            return IsLinqExtensionOfIEnumerableOfTWithPredicate("Where", parameterCount: 2, allowImmutableArrayExtension: allowImmutableArrayExtension);
        }

        public bool IsLinqWhereWithIndex()
        {
            return IsLinqExtensionOfIEnumerableOfT("Where", parameterCount: 2, allowImmutableArrayExtension: false)
                && Roslynator.Symbol.IsPredicateFunc(Parameters[1].Type, Symbol.TypeArguments[0], SemanticModel.Compilation.GetSpecialType(SpecialType.System_Int32), SemanticModel);
        }

        public bool IsLinqSelect(bool allowImmutableArrayExtension = false)
        {
            if (IsValid
                && Symbol.IsPublic()
                && Symbol.ReturnType.IsConstructedFromIEnumerableOfT()
                && HasName("Select")
                && Symbol.Arity == 2)
            {
                INamedTypeSymbol containingType = Symbol.ContainingType;

                if (containingType != null)
                {
                    if (containingType.Equals(SemanticModel.GetTypeByMetadataName(MetadataNames.System_Linq_Enumerable)))
                    {
                        ImmutableArray<IParameterSymbol> parameters = Parameters;

                        return parameters.Length == 2
                            && parameters[0].Type.IsConstructedFromIEnumerableOfT()
                            && Roslynator.Symbol.IsFunc(parameters[1].Type, Symbol.TypeArguments[0], Symbol.TypeArguments[1], SemanticModel);
                    }
                    else if (allowImmutableArrayExtension
                        && containingType.Equals(SemanticModel.GetTypeByMetadataName(MetadataNames.System_Linq_ImmutableArrayExtensions)))
                    {
                        ImmutableArray<IParameterSymbol> parameters = Parameters;

                        return parameters.Length == 2
                            && parameters[0].Type.IsConstructedFromImmutableArrayOfT(SemanticModel)
                            && Roslynator.Symbol.IsFunc(parameters[1].Type, Symbol.TypeArguments[0], Symbol.TypeArguments[1], SemanticModel);
                    }
                }
            }

            return false;
        }

        public bool IsLinqCast()
        {
            return IsValid
                && Symbol.IsPublic()
                && Symbol.ReturnType.IsConstructedFromIEnumerableOfT()
                && HasName("Cast")
                && Symbol.Arity == 1
                && Symbol.SingleParameterOrDefault()?.Type.IsIEnumerable() == true
                && Symbol
                    .ContainingType?
                    .Equals(SemanticModel.GetTypeByMetadataName(MetadataNames.System_Linq_Enumerable)) == true;
        }

        public bool IsLinqExtensionOfIEnumerableOfT(
            string name,
            int parameterCount,
            bool allowImmutableArrayExtension = false)
        {
            if (IsValid
                && Symbol.IsPublic()
                && HasName(name))
            {
                INamedTypeSymbol containingType = Symbol.ContainingType;

                if (containingType != null)
                {
                    if (containingType.Equals(SemanticModel.GetTypeByMetadataName(MetadataNames.System_Linq_Enumerable)))
                    {
                        ImmutableArray<IParameterSymbol> parameters = Parameters;

                        return parameters.Length == parameterCount
                            && parameters[0].Type.IsConstructedFromIEnumerableOfT();
                    }
                    else if (allowImmutableArrayExtension
                        && containingType.Equals(SemanticModel.GetTypeByMetadataName(MetadataNames.System_Linq_ImmutableArrayExtensions)))
                    {
                        ImmutableArray<IParameterSymbol> parameters = Parameters;

                        return parameters.Length == parameterCount
                            && parameters[0].Type.IsConstructedFromImmutableArrayOfT(SemanticModel);
                    }
                }
            }

            return false;
        }

        public bool IsLinqExtensionOfIEnumerableOfTWithPredicate(
            string name,
            bool allowImmutableArrayExtension = false)
        {
            return IsLinqExtensionOfIEnumerableOfTWithPredicate(name, parameterCount: 2, allowImmutableArrayExtension: allowImmutableArrayExtension);
        }

        private bool IsLinqExtensionOfIEnumerableOfTWithPredicate(
            string name,
            int parameterCount,
            bool allowImmutableArrayExtension = false)
        {
            if (IsValid
                && Symbol.IsPublic()
                && HasName(name))
            {
                INamedTypeSymbol containingType = Symbol.ContainingType;

                if (containingType != null)
                {
                    if (containingType.Equals(SemanticModel.GetTypeByMetadataName(MetadataNames.System_Linq_Enumerable)))
                    {
                        ImmutableArray<IParameterSymbol> parameters = Parameters;

                        return parameters.Length == parameterCount
                            && parameters[0].Type.IsConstructedFromIEnumerableOfT()
                            && Roslynator.Symbol.IsPredicateFunc(parameters[1].Type, Symbol.TypeArguments[0], SemanticModel);
                    }
                    else if (allowImmutableArrayExtension
                        && containingType.Equals(SemanticModel.GetTypeByMetadataName(MetadataNames.System_Linq_ImmutableArrayExtensions)))
                    {
                        ImmutableArray<IParameterSymbol> parameters = Parameters;

                        return parameters.Length == parameterCount
                            && parameters[0].Type.IsConstructedFromImmutableArrayOfT(SemanticModel)
                            && Roslynator.Symbol.IsPredicateFunc(parameters[1].Type, Symbol.TypeArguments[0], SemanticModel);
                    }
                }
            }

            return false;
        }

        private bool HasName(string name)
        {
            return string.Equals(Name, name, StringComparison.Ordinal);
        }
    }
}
