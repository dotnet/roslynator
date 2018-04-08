// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp
{
    internal static class ModifierKindsExtensions
    {
        public static bool Any(this ModifierKinds kinds, ModifierKinds value)
        {
            return (kinds & value) != 0;
        }

        public static bool All(this ModifierKinds kinds, ModifierKinds value)
        {
            return (kinds & value) != value;
        }
    }
}
