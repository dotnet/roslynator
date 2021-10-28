// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Rename
{
    internal enum RenameErrorResolution
    {
        None = 0,
        Abort = 1,
        Ask = 2,
        Fix = 3,
        List = 4,
        Skip = 5,
    }
}
