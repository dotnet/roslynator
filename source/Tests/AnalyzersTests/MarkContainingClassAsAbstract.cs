// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator.CSharp.Analyzers.Tests
{
#pragma warning disable RCS1016
    public static class MarkContainingClassAsAbstract
    {
        public class Foo
        {
            public abstract string MethodName();

            public abstract string PropertyName { get; set; }

            public abstract string this[int index] { get; set; }

            public abstract event EventHandler EventName;
        }
    }
}
