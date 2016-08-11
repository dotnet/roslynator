// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis
{
    public static class INamespaceSymbolExtensions
    {
        public static IEnumerable<INamespaceSymbol> ContainingNamespacesAndSelf(this INamespaceSymbol @namespace)
        {
            if (@namespace == null)
                throw new ArgumentNullException(nameof(@namespace));

            do
            {
                yield return @namespace;

                @namespace = @namespace.ContainingNamespace;
            } while (@namespace != null);
        }

        public static IEnumerable<INamespaceSymbol> ContainingNamespaces(this INamespaceSymbol @namespace)
        {
            if (@namespace == null)
                throw new ArgumentNullException(nameof(@namespace));

            while (@namespace.ContainingNamespace != null)
            {
                yield return @namespace.ContainingNamespace;

                @namespace = @namespace.ContainingNamespace;
            }
        }
    }
}
