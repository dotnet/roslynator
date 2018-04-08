// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class RemoveAllCommentsRefactoring
    {

        /// <summary>
        /// 
        /// </summary>
        public void SomeMethod()
        {
            // ...
            // ...
            // ...
        }


        /// <summary>
        /// 
        /// </summary>
        public void SomeMethod2()
        {
            // ...
            // ...
            // ...
        }

#pragma warning disable 1 // aaa
        //public string PropertyName { get; set; }
#pragma warning disable 1 // aaa
    }
}
