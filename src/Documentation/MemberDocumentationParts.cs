// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Documentation
{
    //XTODO: Security
    [Flags]
    public enum MemberDocumentationParts
    {
        None = 0,
        Overloads = 1,
        ContainingType = 1 << 1,
        ContainingAssembly = 1 << 2,
        ObsoleteMessage = 1 << 3,
        Summary = 1 << 4,
        Declaration = 1 << 5,
        TypeParameters = 1 << 6,
        Parameters = 1 << 7,
        ReturnValue = 1 << 8,
        Implements = 1 << 9,
        Attributes = 1 << 10,
        Exceptions = 1 << 11,
        Examples = 1 << 12,
        Remarks = 1 << 13,
        SeeAlso = 1 << 14,
        AppliesTo = 1 << 15,
        Content = 1 << 16,
        All = int.MaxValue
    }
}
