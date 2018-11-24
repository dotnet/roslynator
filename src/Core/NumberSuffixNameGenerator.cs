// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal class NumberSuffixNameGenerator : NameGenerator
    {
        public override string EnsureUniqueName(string baseName, IEnumerable<string> reservedNames, bool isCaseSensitive = true)
        {
            int suffix = 1;

            string name = baseName;

            while (!IsUniqueName(name, reservedNames, isCaseSensitive))
            {
                suffix++;
                name = baseName + suffix.ToString();
            }

            return name;
        }

        public override string EnsureUniqueName(string baseName, ImmutableArray<ISymbol> symbols, bool isCaseSensitive = true)
        {
            int suffix = 1;

            string name = baseName;

            while (!IsUniqueName(name, symbols, isCaseSensitive))
            {
                suffix++;
                name = baseName + suffix.ToString();
            }

            return name;
        }
    }
}
