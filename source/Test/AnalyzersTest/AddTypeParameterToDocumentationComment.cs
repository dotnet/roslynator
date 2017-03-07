// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.Analyzers.Test
{
#pragma warning disable RCS1100, RCS1131
    internal class AddTypeParameterToDocumentationComment
    {
        /// <summary>
        /// x
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        private class FooClass<T1, T2>
        {
        }

        /// <summary>
        /// x
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        private interface FooInterface<T1, T2, T3>
        {
        }

        /// <typeparam name="T2"></typeparam>
        private interface FooInterface2<T1, T2, T3>
        {
        }

        /// <typeparam name="T2"></typeparam>
        private struct FooStruct<T1, T2>
        {
        }

        /// <typeparam name="T1"></typeparam>
        /// <param name="parameter"></param>
        private delegate void FooDelegate<T1, T2>(object parameter);

        /// <summary>
        /// x
        /// </summary>
        /// <param name="parameter"></param>
        public void Foo<T>(object parameter)
        {
        }
    }
#pragma warning restore RCS1100, RCS1131
}
