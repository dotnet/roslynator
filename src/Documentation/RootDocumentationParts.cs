// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Documentation
{
    [Flags]
    public enum RootDocumentationParts
    {
        None = 0,
        Content = 1,
        Namespaces = 2,
        Classes = 4,
        StaticClasses = 8,
        Structs = 16,
        Interfaces = 32,
        Enums = 64,
        Delegates = 128,
        Types = Classes | StaticClasses | Structs | Interfaces | Enums | Delegates,
        Other = 256,
        All = Content | Namespaces | Types | Other
    }
}
