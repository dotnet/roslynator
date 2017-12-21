// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Markdown
{
    [Flags]
    public enum EmphasisOptions
    {
        None = 0,
        Bold = 1,
        Italic = 2,
        BoldItalic = Bold | Italic,
        Strikethrough = 4,
        Code = 8
    }
}
