// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CommandLine
{
    [Flags]
    internal enum OmitMemberParts
    {
        None = 0,
        ConstantValue = 1,
        Implements = 2,
        InheritedFrom = 4,
        Overrides = 8
    }
}
