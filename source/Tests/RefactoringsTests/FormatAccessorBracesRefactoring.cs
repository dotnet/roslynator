// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class FormatAccessorBraces
    {
        private class FormatAccessorBracesOnMultipleLinesRefactoring
        {
            private string _value;

            public string Value
            {
                get { return _value; }
            }
        }

        internal class FormatAccessorBracesOnSingleLineRefactoring
        {
            private string _value;

            public string Value
            {
                get
                {
                    return _value;
                }
            }
        }
    }
}
