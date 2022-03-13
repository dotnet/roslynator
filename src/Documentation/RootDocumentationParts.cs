// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Documentation
{
    [Flags]
    public enum RootDocumentationParts
    {
        None = 0,
        Content = 1,
        Namespaces = 1 << 1,
        ClassHierarchy = 1 << 2,
        Types = 1 << 3,
        Other = 1 << 4,
        All = int.MaxValue
    }
}
