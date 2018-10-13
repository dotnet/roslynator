// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Roslynator.Tests;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CSTests : AbstractCSharpCompilerDiagnosticFixVerifier
    {
        public CSTests()
        {
            Options = base.Options;
        }

        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.OperatorCannotBeAppliedToOperands;

        public override CodeFixProvider FixProvider { get; }

        public override CodeVerificationOptions Options { get; }

        //[Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.OperatorCannotBeAppliedToOperands)]
        public async Task Test()
        {
            await VerifyFixAsync(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class C
{
    void M()
    {
    }
}
", @"
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }

        //[Theory, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.OperatorCannotBeAppliedToOperands)]
        //[InlineData("", "")]
        public async Task Test(string fromData, string toData)
        {
            await VerifyFixAsync(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class C
{
    void M()
    {
    }
}
", fromData, toData, equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }

        //[Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.OperatorCannotBeAppliedToOperands)]
        public async Task TestNoFix()
        {
            await VerifyNoFixAsync(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class C
{
    void M()
    {
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }
    }
}
