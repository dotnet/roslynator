// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Formatting;

internal class CodeFormatterOptions
{
    public static CodeFormatterOptions Default { get; } = new();

    public CodeFormatterOptions(
        FileSystemFilter fileSystemFilter = null,
        bool includeGeneratedCode = false)
    {
        FileSystemFilter = fileSystemFilter;
        IncludeGeneratedCode = includeGeneratedCode;
    }

    public FileSystemFilter FileSystemFilter { get; }

    public bool IncludeGeneratedCode { get; }
}
