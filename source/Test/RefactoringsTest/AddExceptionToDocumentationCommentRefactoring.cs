// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.Refactorings.Test
{
    internal class AddExceptionToDocumentationCommentRefactoring
    {
        /// <summary>
        /// 
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

            Parameter3 = parameter3 ?? throw new InvalidOperationException(nameof(parameter3));
        }

        /// <summary>
        /// 
        /// </summary>
        public string FooProperty
        {
            get
            {
                string s = null;

                if (s == null)
                    throw new ArgumentNullException(nameof(s));

                return s;
            }
        }

        public object Parameter3 { get; private set; }

        public class Exception<T> : Exception
        {

        }
    }
}
