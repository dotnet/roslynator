// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analyzers.Test
{
    public static class FormatEmptyBlock
    {
        public static void Foo()
        { }

        public static string FooProperty
        {
            get { }
            set { }
        }
    }
}
