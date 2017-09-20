// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

#pragma warning disable RCS1100, RCS1016, RCS1079, CS0168

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class UnusedTypeParameter
    {
        private interface IFoo
        {
            void Bar<T>();
        }

        private abstract class Foo : IFoo
        {
            public void Bar<T>()
            {
            }

            public virtual void Bar2<T>()
            {
            }

            public abstract void Bar3<T>();

            public void Bar4<T>(T parameter)
            {
                Bar4<T>(parameter);
            }

            public void Bar5<T>()
            {
                void LocalFunction<T2>()
                {
                }
            }

            public T2 Bar6<T1, T2>(T2 value)
            {
                return value;
            }

            /// <summary>
            /// ...
            /// </summary>
            /// <typeparam name="T1"></typeparam>
            /// <typeparam name="T2"></typeparam>
            /// <param name="value"></param>
            /// <returns></returns>
            public void Bar7<T1, T2>()
            {
            }
        }

        private abstract class Foo2
        {
            public void Bar<T>()
            {
                throw new NotImplementedException();
            }

            public void Bar4<T>(T parameter)
            {
                throw new NotImplementedException();
            }

            public T2 Bar6<T1, T2>(T2 value)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// ...
            /// </summary>
            /// <typeparam name="T1"></typeparam>
            /// <typeparam name="T2"></typeparam>
            /// <param name="value"></param>
            /// <returns></returns>
            public void Bar7<T1, T2>()
            {
                throw new NotImplementedException();
            }
        }

        private partial class Foo3 : Foo
        {
            public override void Bar2<T>()
            {
            }

            public override void Bar3<T>()
            {
            }

            partial void BarPartial<T>();
        }

        private partial class Foo3 : Foo
        {
        }
    }
}
