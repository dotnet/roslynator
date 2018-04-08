// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal class UnderscoreSuffixNameGenerator : NameGenerator
    {
        public override string EnsureUniqueName(string baseName, IEnumerable<string> reservedNames, bool isCaseSensitive = true)
        {
            string suffix = "";

            string name = baseName;

            while (!IsUniqueName(name, reservedNames, isCaseSensitive))
            {
                suffix += "_";
                name = baseName + suffix;
            }

            return name;
        }

        public override string EnsureUniqueName(string baseName, ImmutableArray<ISymbol> symbols, bool isCaseSensitive = true)
        {
            string suffix = "";

            string name = baseName;

            while (!IsUniqueName(name, symbols, isCaseSensitive))
            {
                suffix += "_";
                name = baseName + suffix;
            }

            return name;
        }
    }
}
