// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal abstract class UniqueNameGenerator
    {
        public abstract string GetNewName(string name);

        public virtual string GetInitialName(string name)
        {
            return name;
        }

        public string EnsureUniqueName(string baseName, IEnumerable<string> reservedNames)
        {
            string name = GetInitialName(baseName);

            while (reservedNames.Any(f => NameEquals(f, name)))
                name = GetNewName(baseName);

            return name;
        }

        protected virtual bool NameEquals(string name1, string name2)
        {
            return string.Equals(name1, name2, StringComparison);
        }

        public virtual StringComparison StringComparison
        {
            get { return StringComparison.Ordinal; }
        }
    }
}
