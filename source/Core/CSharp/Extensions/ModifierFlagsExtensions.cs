// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp
{
    internal static class ModifierFlagsExtensions
    {
        public static bool Any(this ModifierFlags flags, ModifierFlags value)
        {
            return (flags & value) != 0;
        }

        public static bool All(this ModifierFlags flags, ModifierFlags value)
        {
            return (flags & value) != value;
        }
    }
}
