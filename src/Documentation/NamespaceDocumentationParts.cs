// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Documentation
{
    [Flags]
    public enum NamespaceDocumentationParts
    {
        None = 0,
        Content = 1,
        ContainingNamespace = 2,
        Summary = 4,
        Examples = 8,
        Remarks = 16,
        Classes = 32,
        Structs = 64,
        Interfaces = 128,
        Enums = 256,
        Delegates = 512,
        SeeAlso = 1024,
        All = Content | ContainingNamespace | Summary | Examples | Remarks | Classes | Structs | Interfaces | Enums | Delegates | SeeAlso
    }
}
