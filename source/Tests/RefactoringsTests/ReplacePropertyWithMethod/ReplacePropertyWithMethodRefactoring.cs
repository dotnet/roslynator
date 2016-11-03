// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class ReplacePropertyWithMethodRefactoring
    {
        public ReplacePropertyWithMethodRefactoring()
        {
            var x = Value;
            x = this.Value;
        }

#if DEBUG
        public ReplacePropertyWithMethodRefactoring Value /**/
        {
            get
            {
                var x = /*a*/ Value /*b*/
                    /*c*/ .Value /*d*/
                    /*e*/ .Value /*f*/;

                return null;
            }
        }
#endif

        private string _value2;

        public string Value2
        {
            get { return _value2; }
            set { _value2 = value; }
        }

#if DEBUG
        public string Value3 { get; } = new string(' ', 1); /**/
#endif

        public string IsValue
        {
            get { return string.Empty; }
        }
    }
}
