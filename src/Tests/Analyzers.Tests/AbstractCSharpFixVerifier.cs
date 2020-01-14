// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Roslynator.CSharp.Testing;
using Roslynator.Testing;
using Roslynator.Testing.Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public abstract class AbstractCSharpFixVerifier : CSharpFixVerifier
    {
        protected override IAssert Assert => XunitAssert.Instance;
    }
}
