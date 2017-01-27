// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.Analyzers.Tests
{
#pragma warning disable RCS1100
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

        public class Exception<T> : Exception
        {
        }
    }
#pragma warning restore RCS1100
}
