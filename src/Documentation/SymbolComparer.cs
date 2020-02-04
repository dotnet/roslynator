// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation
{
    internal sealed class SymbolComparer : IComparer<ISymbol>
    {
        private SymbolComparer(SymbolDisplayFormat format, bool systemNamespaceFirst, bool includeNamespaces, SymbolDisplayAdditionalMemberOptions additionalOptions)
        {
            Format = format;
            SystemNamespaceFirst = systemNamespaceFirst;
            IncludeNamespaces = includeNamespaces;
            AdditionalOptions = additionalOptions;
        }

        internal static SymbolComparer TypeWithoutNamespace { get; } = new SymbolComparer(TypeSymbolDisplayFormats.Name_ContainingTypes_TypeParameters, systemNamespaceFirst: false, includeNamespaces: false, SymbolDisplayAdditionalMemberOptions.None);

        public static SymbolComparer Create(
            bool systemNamespaceFirst = true,
            bool includeNamespaces = true,
            SymbolDisplayAdditionalMemberOptions additionalOptions = SymbolDisplayAdditionalMemberOptions.None)
        {
            return new SymbolComparer(
                TypeSymbolDisplayFormats.GetFormat(includeNamespaces: false),
                systemNamespaceFirst: systemNamespaceFirst,
                includeNamespaces: includeNamespaces,
                additionalOptions: additionalOptions);
        }

        public SymbolDisplayFormat Format { get; }

        public bool SystemNamespaceFirst { get; }

        public bool IncludeNamespaces { get; }

        public SymbolDisplayAdditionalMemberOptions AdditionalOptions { get; }

        public int Compare(ISymbol x, ISymbol y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            int diff;

            if (IncludeNamespaces)
            {
                if (SystemNamespaceFirst)
                {
                    diff = SymbolDefinitionComparer.SystemFirst.CompareContainingNamespace(x, y);
                }
                else
                {
                    diff = SymbolDefinitionComparer.Default.CompareContainingNamespace(x, y);
                }

                if (diff != 0)
                    return diff;
            }

            return string.Compare(x.ToDisplayString(Format, AdditionalOptions), y.ToDisplayString(Format, AdditionalOptions), StringComparison.InvariantCulture);
        }
    }
}
