// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel;

namespace Roslynator.CSharp.Analyzers.Test
{
#pragma warning disable RCS1100, RCS1016, RCS1023, CS0168
    public static class UnusedParameter
    {
        private static void ExtensionMethod(this Foo foo, object parameter)
        {
        }

        private interface IFoo
        {
            void Bar(object parameter);
        }

        private abstract class Foo : IFoo
        {
            public void Bar(object parameter) { }

            public virtual void Bar2(object parameter) { }

            public abstract void Bar3(object parameter);

            private void EventHandlerMethod(object sender, EventArgs args) { }

            private void EventHandlerMethod2(object sender, ConsoleCancelEventArgs args) { }

            public void Bar4(object parameter)
            {
                Bar4(parameter);
            }

            protected Foo(object parameter)
            {
                object LocalFunction(object value) => null;
            }

            public string this[int index] => "";

            public void Bar5(object parameter) { }

            public object Bar6(object parameter1, object parameter2) => parameter2;

            /// <summary>
            /// ...
            /// </summary>
            /// <param name="parameter1"></param>
            /// <param name="parameter2"></param>
            public void Bar7(object parameter1, object parameter2) { }

            public void Bar8(object parameter1, object parameter2) { Bar7(parameter1: null, parameter2: null); }
        }

        private partial class Foo2 : Foo
        {
            public Foo2(object parameter) : base(parameter) { }

            public override void Bar2(object parameter) { }

            public override void Bar3(object parameter) { }

            partial void BarPartial<T>();

            private void MethodGroup(object parameter)
            {
                Action<object> action = MethodGroup;
            }
        }

        private partial class Foo2 : Foo
        {
        }
    }
}
