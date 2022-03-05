// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Documentation
{
    [Flags]
    internal enum SymbolDisplayAdditionalOptions
    {
        None = 0,
        OmitContainingNamespace = 1,
        IncludeAttributes = 1 << 1,
        IncludeParameterAttributes = 1 << 2,
        IncludeAccessorAttributes = 1 << 3,
        IncludeAttributeArguments = 1 << 4,
        FormatAttributes = 1 << 5,
        WrapBaseTypes = 1 << 6,
        WrapConstraints = 1 << 7,
        FormatParameters = 1 << 8,
        OmitIEnumerable = 1 << 9,
        IncludeTrailingSemicolon = 1 << 10,
    }
}
