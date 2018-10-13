// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Documentation
{
    [Flags]
    public enum SymbolDisplayTypeDeclarationOptions
    {
        None = 0,
        IncludeModifiers = 1,
        IncludeAccessibility = 2,
    }
}
