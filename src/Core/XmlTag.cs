// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator
{
    // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/recommended-tags-for-documentation-comments
    // https://docs.microsoft.com/en-us/dotnet/visual-basic/language-reference/xmldoc
    internal enum XmlTag
    {
        None = 0,
        C = 1,
        Code = 2,
        Content = 3,
        Example = 4,
        Exception = 5,
        Exclude = 6,
        Include = 7,
        InheritDoc = 8,
        List = 9,
        Para = 10,
        Param = 11,
        ParamRef = 12,
        Permission = 13,
        Remarks = 14,
        Returns = 15,
        See = 16,
        SeeAlso = 17,
        Summary = 18,
        TypeParam = 19,
        TypeParamRef = 20,
        Value = 21
    }
}
