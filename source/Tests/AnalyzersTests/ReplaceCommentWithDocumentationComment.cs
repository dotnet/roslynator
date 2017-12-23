// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

#pragma warning disable RCS1007, RCS1016, RCS1100, RCS1106, RCS1138, RCS1163, RCS1164, RCS1176

// x
namespace Roslynator.CSharp.Analyzers.Tests
{
    // x
    public static class ReplaceCommentWithDocumentationComment
    {
        // x
        // xx
        public class Foo
        {
            // x
            public string FieldName;

            // x
            public const string ConstantName = null;

            // x
            public Foo(object parameter)
            {
            }

            // x
            ~Foo()
            {
            }

            // x
            public event EventHandler EventName;

            // x
            protected virtual void OnEventName(EventArgs e)
            {
                EventName?.Invoke(this, e);
            }

            // x
            public event EventHandler<EventArgs> EventName2;

            // x
            protected virtual void OnEventName2(EventArgs e)
            {
                EventHandler<EventArgs> handler = EventName2;
                if (handler != null)
                    handler(this, e);
            }

            // x
            public string PropertyName { get; set; }

            // x
            public string this[int index]
            {
                get { return null; }
                set { }
            }

            // x
            public void MethodNameVoid<T>(object parameter)
            {
            }

            // x
            public string MethodName<T>(object parameter)
            {
                return null;
            }

            // x
            public static explicit operator Foo(string value)
            {
                return new Foo(null);
            }

            // x
            public static explicit operator string(Foo value)
            {
                return null;
            }

            // x
            public static Foo operator !(Foo value)
            {
                return new Foo(null);
            }

            // x
            public enum EnumName
            {
                // x
                None
            }

            // x
            public interface InterfaceName<T>
            {
            }

            // x
            public struct StructName<T>
            {
            }

            // x
            public class ClassName<T>
            {
            }

            // x
            public delegate void DelegateName<T>(object parameter);

            // n

            /// <summary>
            /// x
            /// </summary>
            public class Foo2
            {
            }
        }
    }
}
