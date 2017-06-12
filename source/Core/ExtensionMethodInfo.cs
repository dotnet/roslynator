// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal struct ExtensionMethodInfo
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

        public static bool TryCreate(IMethodSymbol methodSymbol, SemanticModel semanticModel, out ExtensionMethodInfo extensionMethodInfo, ExtensionMethodKind kind = ExtensionMethodKind.None)
        {
            if (methodSymbol?.IsExtensionMethod == true)
            {
                IMethodSymbol reducedFrom = methodSymbol.ReducedFrom;

                if (reducedFrom != null)
                {
                    if (kind != ExtensionMethodKind.NonReduced)
                    {
                        extensionMethodInfo = new ExtensionMethodInfo(reducedFrom, methodSymbol, semanticModel);
                        return true;
                    }
                }
                else if (kind != ExtensionMethodKind.Reduced)
                {
                    extensionMethodInfo = new ExtensionMethodInfo(methodSymbol, methodSymbol, semanticModel);
                    return true;
                }
            }

            extensionMethodInfo = default(ExtensionMethodInfo);
            return false;
        }
    }
}
