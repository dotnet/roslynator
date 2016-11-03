// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class RemoveBracesRefactoring
    {
        private string _value;

        public string Value
        {
            get
            {
                if (_value == null)
                {
                    _value = Initialize();
                }

                return _value;
            }
        }

        private string Initialize()
        {
            return null;
        }
    }
}
