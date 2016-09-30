// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings.Tests
{
    internal class MergeLocalDeclarationsRefactoring
    {
        public void MethodName()
        {
#if DEBUG
            bool f = false;
            bool f2 = false;
            bool f3 = false;
#endif
            string s = null;

            switch (0)
            {
                case 0:
                    bool ff = false;
                    bool ff2 = false;
                    bool ff3 = false;
                    string ss = null;
                    break;
            }
        }
    }
}
