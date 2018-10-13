// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Documentation
{
    //XTODO: Security
    [Flags]
    public enum MemberDocumentationParts
    {
        None = 0,
        Overloads = 1,
        ContainingType = 2,
        ContainingAssembly = 4,
        ObsoleteMessage = 8,
        Summary = 16,
        Declaration = 32,
        TypeParameters = 64,
        Parameters = 128,
        ReturnValue = 256,
        Implements = 512,
        Attributes = 1024,
        Exceptions = 2048,
        Examples = 4096,
        Remarks = 8192,
        SeeAlso = 16384,
        All = Overloads | ContainingType | ContainingAssembly | ObsoleteMessage | Summary | Declaration | TypeParameters | Parameters | ReturnValue | Implements | Attributes | Exceptions | Examples | Remarks | SeeAlso
    }
}
