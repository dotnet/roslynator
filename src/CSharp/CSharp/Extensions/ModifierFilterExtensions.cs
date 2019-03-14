// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp
{
    internal static class ModifierFilterExtensions
    {
        public static bool Any(this ModifierFilter modifierFilter, ModifierFilter value)
        {
            return (modifierFilter & value) != 0;
        }
    }
}
