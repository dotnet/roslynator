// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CodeFixes
{
    internal enum DiagnosticFixKind
    {
        Success = 0,
        PartiallyFixed = 1,
        NotFixed = 2,
        MultipleFixers = 3,
        CompilerError = 4
    }
}
