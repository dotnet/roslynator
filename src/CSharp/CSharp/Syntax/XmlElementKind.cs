// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Syntax
{
    internal enum XmlElementKind
    {
        None = 0,
        Include = 1,
        Exclude = 2,
        InheritDoc = 3,
        Summary = 4,
        Exception = 5,
    }
}
