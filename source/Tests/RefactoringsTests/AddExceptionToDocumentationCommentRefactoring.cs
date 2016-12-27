// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class AddExceptionToDocumentationCommentRefactoring
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        public void Foo(object parameter, object parameter2)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            if (parameter2 == null)
                throw new ArgumentNullException(nameof(parameter2));

            if (parameter == null)
                throw new ArgumentException(nameof(parameter));

            if (parameter == null)
                throw new Exception<string>();
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

        public class Exception<T> : Exception
        {

        }
    }
}
