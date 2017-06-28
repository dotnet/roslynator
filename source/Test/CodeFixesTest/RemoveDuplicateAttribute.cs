// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Roslynator.CSharp.CodeFixes.Test;

[assembly: AssemblyAttribute, AssemblyAttribute]
[assembly: AssemblyAttribute]

namespace Roslynator.CSharp.CodeFixes.Test
{
    internal static class RemoveDuplicateAttribute
    {
        [Obsolete, Obsolete]
        private static class Foo
        {
        }

        [Obsolete]
        [Obsolete]
        private static class Foo2
        {
        }
    }

    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyAttribute : Attribute
    {
    }
}
