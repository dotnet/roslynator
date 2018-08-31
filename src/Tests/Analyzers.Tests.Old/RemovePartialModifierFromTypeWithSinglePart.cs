// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.ObjectModel;

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class RemovePartialModifierFromTypeWithSinglePart
    {
        private static class Foo
        {
            private partial class FooClass
            {
            }

            private partial interface FooInterface
            {
            }

            private partial struct FooStruct
            {
            }
        }

        private static class Foo2
        {
            private static partial class FooClass
            {
                private partial class FooClass2
                {
                }

                private partial class FooClass2
                {
                }
            }

            private partial struct FooStruct
            {
                private partial class FooClass2
                {
                }

                private partial class FooClass2
                {
                }
            }
        }

        private static class Foo3
        {
            private static partial class FooClass
            {
                private partial interface FooInterface
                {
                }

                private partial interface FooInterface
                {
                }
            }

            private partial struct FooStruct
            {
                private partial interface FooInterface
                {
                }

                private partial interface FooInterface
                {
                }
            }
        }

        private static class Foo4
        {
            private static partial class FooClass
            {
                private partial class FooClass2
                {
                }

                private partial class FooClass2
                {
                }
            }

            private partial struct FooStruct
            {
                private partial class FooClass2
                {
                }

                private partial class FooClass2
                {
                }
            }
        }

        private static class Foo5
        {
            private partial class FooClass
            {
                partial void FooMethod();

                partial void FooMethod()
                {
                }
            }

            private partial struct FooStruct
            {
                partial void FooMethod();

                partial void FooMethod()
                {
                }
            }
        }
    }
}