// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Documentation
{
    [Flags]
    public enum RootDocumentationParts
    {
        None = 0,
        Content = 1,
        Namespaces = 2,
        ClassHierarchy = 4,
        Types = 8,
        Other = 16,
        All = int.MaxValue
    }
}
