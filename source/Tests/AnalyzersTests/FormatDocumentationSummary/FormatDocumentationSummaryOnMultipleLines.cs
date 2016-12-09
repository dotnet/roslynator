// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class FormatSummaryElementOnMultipleLines
    {
        /// <summary>a<code>b</code>c</summary>
        public static void Foo()
        {
        }

        /// <summary></summary>
        public static void Foo2()
        {
        }

        /// <summary>
        /// </summary>
        public static void Foo3()
        {
        }

        /// <summary>
        /// comment
        /// </summary>
        public static void Foo4()
        {
        }
    }
}
