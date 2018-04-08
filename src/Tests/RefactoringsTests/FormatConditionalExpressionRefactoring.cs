// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class FormatConditionalExpression
    {
        private class FormatConditionalExpressionOnMultipleLinesRefactoring
        {
            public void SomeMethod()
            {
                bool condition = false;

                var s = (condition) ? true : false;
            }
        }

        private class FormatConditionalExpressionToSingleLineRefactoring
        {
            public void SomeMethod()
            {
                bool condition = false;

                var s = (condition)
                    ? true
                    : false;
            }
        }
    }
}
