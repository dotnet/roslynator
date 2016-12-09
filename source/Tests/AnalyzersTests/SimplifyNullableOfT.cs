// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class SimplifyNullableOfT
    {
        public static void Foo()
        {
            Nullable<int> x = null;

            Nullable<System.Int32> y = null;

            System.Nullable<System.Int32> y = null;
        }
    }
}
