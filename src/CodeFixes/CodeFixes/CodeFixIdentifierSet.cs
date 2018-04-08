// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator.CodeFixes
{
    public class CodeFixIdentifierSet : HashSet<string>
    {
        public CodeFixIdentifierSet()
            : base(StringComparer.Ordinal)
        {
        }

        public bool ContainsAny(string identifier, string identifier2)
        {
            return Contains(identifier)
                || Contains(identifier2);
        }

        public bool ContainsAny(string identifier, string identifier2, string identifier3)
        {
            return Contains(identifier)
                || Contains(identifier2)
                || Contains(identifier3);
        }

        public bool ContainsAny(string identifier, string identifier2, string identifier3, string identifier4)
        {
            return Contains(identifier)
                || Contains(identifier2)
                || Contains(identifier3)
                || Contains(identifier4);
        }
    }
}
