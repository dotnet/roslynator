// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.Analyzers.Test
{
    internal class MarkClassAsStatic
    {
        public class Foo
        {
            public const string FooConst = "";

            private static readonly string _fooField;

            public static string FooProperty { get; set; }

            public static event EventHandler FooEvent;

            public static void FooMethod()
            {
            }

            public class FooClass
            {
            }

            public struct FooStruct
            {
            }

            public interface FooInterface
            {
            }

            public delegate void FooDelegate();

            public enum FooEnum
            {
            }
        }

        class Foo2
        {
            public static void Foo()
            {
            }
        }

        public partial class Foo3
        {
            public static void Foo()
            {
            }
        }

        public partial class Foo3
        {
        }

        public sealed class Foo4
        {
            public static void Foo()
            {
            }
        }
    }
}
