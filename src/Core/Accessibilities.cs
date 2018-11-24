// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator
{
    [Flags]
    internal enum Accessibilities
    {
        None = 0,
        Private = 1,
        ProtectedAndInternal = 2,
        Protected = 4,
        Internal = 8,
        ProtectedOrInternal = 16,
        Public = 32,
    }
}
