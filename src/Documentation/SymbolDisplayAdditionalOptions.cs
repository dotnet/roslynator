// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Documentation
{
    [Flags]
    internal enum SymbolDisplayAdditionalOptions
    {
        None = 0,
        OmitContainingNamespace = 1,
        IncludeAttributes = 2,
        IncludeParameterAttributes = 4,
        IncludeAccessorAttributes = 8,
        IncludeAttributeArguments = 16,
        FormatAttributes = 32,
        WrapBaseTypes = 64,
        WrapConstraints = 128,
        FormatParameters = 256,
        OmitIEnumerable = 512,
        IncludeTrailingSemicolon = 1024,
    }
}
