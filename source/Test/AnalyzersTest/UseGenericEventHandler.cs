// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator.CSharp.Analyzers.Test
{
    public static class UseGenericEventHandler
    {
        public class Foo
        {
            public event FooEventHandler EventName;

            public event FooEventHandler EventName2
            {
                add { }
                remove { }
            }

            public event EventHandler EventName3;
        }

        public interface FooInterface
        {
            event FooEventHandler Changed;
        }

        public class FooImplementation : FooInterface
        {
            public event FooEventHandler Changed;
        }

        public class FooEventArgs : EventArgs
        {
        }

        public delegate void FooEventHandler(object sender, FooEventArgs args);
    }
}
