// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator.CSharp.Analyzers.Test
{
#pragma warning disable RCS1100, RCS1101
    public static class AddSummaryElementToDocumentationComment
    {
        /// <summary>
        /// Bla
        /// </summary>
        /// <param name="parameter"></param>
        private static void Foo(object parameter)
        {
        }

        /// <param name="parameter"></param>
        /// <param name="parameter2"></param>
        private static void Foo2(object parameter, object parameter2)
        {
        }
    }
#pragma warning restore RCS1100, RCS1101
}
