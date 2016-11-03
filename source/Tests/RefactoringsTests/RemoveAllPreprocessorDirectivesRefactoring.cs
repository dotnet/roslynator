// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class RemoveAllPreprocessorDirectivesRefactoring
    {
        #region Methods
        public void Foo()
        {
#if DEBUG
            string s = "DEBUG";
#endif
        }
        #endregion

#pragma warning disable 1
#pragma warning restore 1

        #region Properties
        public string PropertyName { get; set; }
        #endregion
    }
}
