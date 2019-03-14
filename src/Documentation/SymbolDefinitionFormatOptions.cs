// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Documentation
{
    [Flags]
    internal enum SymbolDefinitionFormatOptions
    {
        None = 0,
        Attributes = 1,
        Parameters = 2,
        BaseList = 4,
        Constraints = 8,
    }
}
