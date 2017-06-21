// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel;

#pragma warning disable RCS1100, RCS1016, RCS1023, RCS1079, RCS1140, RCS1185, CS0168

namespace Roslynator.CSharp.Analyzers.Test
{
    public static partial class UnusedParameter
    {
        private static void ExtensionMethod(this Foo foo, object parameter)
        {
        }

        private abstract class Foo : IFoo
        {
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

            // n

            public void Bar(object parameter) { }

            public virtual void Bar2(object parameter) { }

            public abstract void Bar3(object parameter);

            private void EventHandlerMethod(object sender, EventArgs args) { }

            private void EventHandlerMethod2(object sender, ConsoleCancelEventArgs args) { }

            public void Bar4(object parameter)
            {
                Bar4(parameter);
            }
        }

        private static void ExtensionMethod2(this Foo foo, object parameter)
        {
            throw new NotImplementedException();
        }

        private abstract class Foo2
        {
            protected Foo2(object parameter)
            {
                throw new NotImplementedException();
            }

            protected Foo2(object parameter, object parameter2) => throw new NotImplementedException();

            public string this[int index] => throw new NotImplementedException();

            public string this[string index]
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public string this[string index, string index2]
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            public void Bar(object parameter) { throw new NotImplementedException(); }

            public object Bar(object parameter1, object parameter2) => throw new NotImplementedException();

            /// <summary>
            /// ...
            /// </summary>
            /// <param name="parameter1"></param>
            /// <param name="parameter2"></param>
            public void Bar2(object parameter1, object parameter2) { throw new NotImplementedException(); }
        }

        private partial class FooPartial : Foo
        {
            public FooPartial(object parameter) : base(parameter) { }

            public override void Bar2(object parameter) { }

            public override void Bar3(object parameter) { }

            partial void BarPartial<T>(object parameter);

            private void Method(object parameter)
            {
                Action<object> action = Method;
            }

            private void Method2(object parameter)
            {
            }
        }

        private interface IFoo
        {
            void Bar(object parameter);
        }
    }
}
