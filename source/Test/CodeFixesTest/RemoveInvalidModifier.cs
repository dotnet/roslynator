// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.CodeFixes.Test
{
    internal static class RemoveInvalidModifier
    {
        private class Foo
        {
            public static Foo()
            {
            }

            public private void MethodName()
            {
                public static void LocalFunction()
                {
                }
            }

            public partial string PropertyName { get; set; }

            public static object this[int index]
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }
            public event EventHandler<EventArgs> EventName
            {
                private add { throw new NotImplementedException(); }
                protected internal remove { throw new NotImplementedException(); }
            }

            public class Foo2 : IFoo
            {
                public static virtual override abstract object IFoo.this[int index] { get => throw new NotImplementedException(); }

                public static string IFoo.PropertyName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

                public string PropertyName2 { get; set; }

                protected internal event EventHandler<EventArgs> IFoo.EventName
                {
                    add { throw new NotImplementedException(); }
                    remove { throw new NotImplementedException(); }
                }

                public void IFoo.MethodName()
                {
                }
            }

            public interface IFoo
            {
                public static virtual override abstract void MethodName();

                public static string PropertyName { get; private set; }

                public static string PropertyName2 { get; protected internal set; }

                public object this[int index] { get; }

                protected internal static event EventHandler<EventArgs> EventName;
            }

            private readonly volatile string _field;

            private static sealed class StaticAndSealed
            {
            }
        }
    }
}
