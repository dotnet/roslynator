// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    public partial class ChangeAccessibilityRefactoring
    {
        public partial class Foo { }
        public partial class Foo { }
        partial class Foo { }

        protected internal partial class Foo2 { }
        protected internal partial class Foo2 { }
        partial class Foo2 { }
    }
}
