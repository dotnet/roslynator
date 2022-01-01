// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CodeGeneration.CSharp
{
    [Flags]
    internal enum WrapRewriterOptions
    {
        None = 0,
        WrapArguments = 1,
        IndentFieldInitializer = 1 << 1,
    }
}
