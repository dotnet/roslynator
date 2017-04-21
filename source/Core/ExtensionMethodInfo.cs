// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    public struct ExtensionMethodInfo
    {
        private ExtensionMethodInfo(IMethodSymbol methodSymbol, IMethodSymbol reducedSymbol, SemanticModel semanticModel)
            : this()
        {
            MethodInfo = new MethodInfo(methodSymbol, semanticModel);
            ReducedSymbol = reducedSymbol;
        }

        public MethodInfo MethodInfo { get; }

        public IMethodSymbol ReducedSymbol { get; }

        public IMethodSymbol Symbol
        {
            get { return MethodInfo.Symbol; }
        }

        public bool IsValid
        {
            get { return MethodInfo.IsValid; }
        }

        public bool IsFromReduced
        {
            get { return IsValid && !object.ReferenceEquals(ReducedSymbol, MethodInfo.Symbol); }
        }

        public bool IsFromOrdinary
        {
            get { return IsValid && object.ReferenceEquals(ReducedSymbol, MethodInfo.Symbol); }
        }

        internal static ExtensionMethodInfo FromOrdinary(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            return Create(methodSymbol, semanticModel, ExtensionMethodKind.Ordinary);
        }

        internal static ExtensionMethodInfo FromReduced(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            return Create(methodSymbol, semanticModel, ExtensionMethodKind.Reduced);
        }

        internal static ExtensionMethodInfo FromOrdinaryOrReduced(IMethodSymbol methodSymbol, SemanticModel semanticModel)
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
                        return new ExtensionMethodInfo(reducedFrom, methodSymbol, semanticModel);
                }
                else if ((allowedKinds & ExtensionMethodKind.Ordinary) != 0)
                {
                    return new ExtensionMethodInfo(methodSymbol, methodSymbol, semanticModel);
                }
            }

            return default(ExtensionMethodInfo);
        }
    }
}
