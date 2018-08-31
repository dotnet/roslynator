// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

#pragma warning disable RCS1049

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class SimplifyBooleanExpression
    {
        private static void Foo()
        {
            bool? x = null;
            bool? x2 = null;

            if (x.HasValue && x.Value) { }

            if ((x.HasValue) && (x.Value)) { }

            if (x.HasValue && !x.Value) { }

            if ((x.HasValue) && (!x.Value)) { }

            if ((x.HasValue) && (!(x.Value))) { }

            if (x.HasValue && x.Value == false) { }

            if ((x.HasValue) && (x.Value == false)) { }

            if ((x.HasValue) && ((x.Value) == false)) { }

            //n

            if (x2.HasValue && x.Value) { }
            if (x.HasValue && x2.Value) { }
            if (x.HasValue && x.HasValue) { }

            if (x2.HasValue && !x.Value) { }
            if (x.HasValue && !x2.Value) { }
            if (x.HasValue && !x.HasValue) { }

            if (x2.HasValue && x.Value == false) { }
            if (x.HasValue && x2.Value == false) { }
            if (x.HasValue && x.HasValue) { }
        }
    }
}
