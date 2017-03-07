// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator.CSharp.Analyzers.Test
{
#pragma warning disable RCS1100, RCS1101
    public static class AddSummaryToDocumentationComment
    {
        /// <summary>
        /// Bla
        /// </summary>
        private static void Foo()
        {
        }

        /// <summary>Bla</summary>
        private static void Foo2()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        private static void Foo3()
        {
        }

        /// <summary>
        ///     
        /// </summary>
        private static void Foo4()
        {
        }

        /// <summary>
        /// </summary>
        private static void Foo5()
        {
        }

        /// <summary></summary>
        private static void Foo6()
        {
        }

        /// <summary> </summary>
        private static void Foo7()
        {
        }
    }
#pragma warning restore RCS1100, RCS1101
}
