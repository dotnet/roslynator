// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp;

[Flags]
internal enum TriviaBetweenFlags
{
    None = 0,
    SingleLineComment = 1,
    DocumentationComment = 1 << 1,
    Comment = SingleLineComment | DocumentationComment,
}
