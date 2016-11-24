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
        public void Foo(object parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            if (parameter == null)
                throw new ArgumentException(nameof(parameter));

            if (parameter == null)
                throw new Exception<string>();
        }

        public class Exception<T> : Exception
        {

        }
    }
}
