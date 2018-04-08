// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Reflection;

namespace Roslynator
{
    public static class TypeInfoExtensions
    {
        public static bool IsStatic(this TypeInfo typeInfo)
        {
            return typeInfo.IsAbstract
                && typeInfo.IsSealed;
        }
    }
}
