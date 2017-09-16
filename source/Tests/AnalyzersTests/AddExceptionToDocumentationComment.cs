// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

#pragma warning disable RCS1100, RCS1141, RCS1163, RCS1176

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal class AddExceptionToDocumentationComment
    {
        /// <summary>
        /// x
        /// </summary>
        /// <param name="parameter"></param>
        public void Foo(object parameter, object parameter2, object parameter3)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            if (parameter2 == null)
                throw new ArgumentNullException(nameof(parameter2));

            if (parameter == null)
                throw new ArgumentException(nameof(parameter));

            if (parameter == null)
                throw new Exception<string>();

            object x = parameter3 ?? throw new InvalidOperationException(nameof(parameter3));
        }

        /// <summary>
        /// x
        /// </summary>
        public string FooProperty
        {
            get
            {
                Func<object, object> func = f =>
                {
                    throw new ArgumentException();
                };

                const string s = null;

                if (s == null)
                    throw new ArgumentNullException(nameof(s));
            }
        }

        private class InheritDoc
        {
            /// <inheritdoc />
            /// <summary>
            /// x
            /// </summary>
            /// <param name="parameter"></param>
            public void Foo(object parameter, object parameter2, object parameter3)
            {
                if (parameter == null)
                    throw new ArgumentNullException(nameof(parameter));
            }
        }

        private class Include
        {
            /// <include file='' path='[@name=""]' />
            public void Foo(object parameter, object parameter2, object parameter3)
            {
                if (parameter == null)
                    throw new ArgumentNullException(nameof(parameter));
            }
        }

        private class Exclude
        {
            /// <exclude />
            public void Foo(object parameter, object parameter2, object parameter3)
            {
                if (parameter == null)
                    throw new ArgumentNullException(nameof(parameter));
            }
        }

        public class Exception<T> : Exception
        {
        }
    }
}
