// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Markdown
{
    [Flags]
    public enum TableFormatting
    {
        None = 0,
        Header = 1,
        Content = 2,
        All = Header | Content
    }
}
