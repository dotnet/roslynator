// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Roslynator.Tests;
using Xunit;

#pragma warning disable RCS1090

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CSTests : AbstractCSharpCompilerCodeFixVerifier
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.OperatorCannotBeAppliedToOperands;

        public override CodeFixProvider FixProvider { get; }

        public override CodeVerificationOptions Options { get; } = CodeVerificationOptions.Default;

        //[Fact]
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
", EquivalenceKey.Create(DiagnosticId));
        }

        //[Theory]
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
", fromData, toData, EquivalenceKey.Create(DiagnosticId));
        }

        //[Fact]
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
", EquivalenceKey.Create(DiagnosticId));
        }
    }
}
