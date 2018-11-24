// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator
{
    // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/recommended-tags-for-documentation-comments
    // https://docs.microsoft.com/en-us/dotnet/visual-basic/language-reference/xmldoc
    internal enum XmlElementKind
    {
        None,
        C,
        Code,
        Content,
        Example,
        Exception,
        Exclude,
        Include,
        InheritDoc,
        List,
        Para,
        Param,
        ParamRef,
        Permission,
        Remarks,
        Returns,
        See,
        SeeAlso,
        Summary,
        TypeParam,
        TypeParamRef,
        Value,
    }
}
