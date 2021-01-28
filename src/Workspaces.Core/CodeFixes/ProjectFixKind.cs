// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CodeFixes
{
    internal enum ProjectFixKind
    {
        Success = 0,

        [Obsolete]
        NoAnalyzers = 1,

        NoFixers = 2,

        [Obsolete]
        NoFixableAnalyzers = 3,

        CompilerError = 4,
        Skipped = 5,
        InfiniteLoop = 6
    }
}
