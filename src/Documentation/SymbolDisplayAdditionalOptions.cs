// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
        FormatBaseList = 64,
        FormatConstraints = 128,
        FormatParameters = 256,
        OmitIEnumerable = 512,
        PreferDefaultLiteral = 1024,
        IncludeTrailingSemicolon = 2048,
    }
}
