// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CodeFixes
{
    public enum ProjectFixKind
    {
        Success = 0,
        NoAnalyzers = 1,
        NoFixers = 2,
        CompilerError = 3,
        Skipped = 4,
        InfiniteLoop = 5,
    }
}
