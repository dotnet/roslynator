// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma warning disable RCS1170

namespace Roslynator.CSharp.Refactorings.Tests
{
    public partial class ChangeAccessibilityRefactoring
    {
        //method
        public void Method() { }

        public string Property { get; private set; }

        protected internal string Property2 { get; protected set; }

        public string Property3 { get; protected set; }

        public string Property4 { get; internal set; }

        //public
        public partial class Foo { }
        public partial class Foo { }
        partial class Foo { }

        //protected internal
        protected internal partial class Foo2 { }
        protected internal partial class Foo2 { }
        partial class Foo2 { }

        protected internal partial class Foo3 { }
        protected internal partial class Foo3 { }
    }
}
