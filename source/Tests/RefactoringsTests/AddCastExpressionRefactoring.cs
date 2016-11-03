// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class AddCastExpressionRefactoring
    {
        private class Foo
        {
            public void SomeMethod()
            {
                object value = GetObject();

                ProcessString(value);
                ProcessValue(value);
            }

            private object GetObject()
            {
                return string.Empty;
            }

            private void ProcessString(string value)
            {
            }

            private void ProcessValue(string value)
            {
            }

            private void ProcessValue(int index)
            {
            }
        }

        private class AddCastExpressionToAssignmentExpressionRefactoring
        {
            public string SomeMethod()
            {
                string s = null;

                object value = GetObject();

                s = value;

                return s;
            }

            private object GetObject()
            {
                return string.Empty;
            }
        }

        public class AddCastExpressionToReturnStatementRefactoring
        {
            public string GetString()
            {
                object value = GetObject();

                return value;
            }

            private object GetObject()
            {
                return string.Empty;
            }

            private async Task<int> Method4Async()
            {
                bool f = false;
                if (f)
                {
                    return await Task.FromResult(new object());
                }
                else
                {
                    return new object();
                }
            }
        }

        private class AddCastExpressionToVariableDeclarationRefactoring
        {
            public void GetString()
            {
                string value = GetObject();
            }

            private object GetObject()
            {
                string s = "";

                return s;
            }
        }
    }
}
