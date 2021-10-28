// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Documentation
{
    [Flags]
    internal enum WrapListOptions
    {
        None = 0,
        Attributes = 1,
        Parameters = 2,
        BaseTypes = 4,
        Constraints = 8,
    }
}
