// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS0192ReadOnlyFieldCannotBePassedAsRefOrOutValueTests : AbstractCSharpCompilerDiagnosticFixVerifier
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.ReadOnlyFieldCannotBePassedAsRefOrOutValue;

        public override CodeFixProvider FixProvider { get; } = new ArgumentCodeFixProvider();

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.ReadOnlyFieldCannotBePassedAsRefOrOutValue)]
        public async Task Test_MakeFieldWritable()
        {
            await VerifyFixAsync(@"
class C
{
    private readonly string _f;

    void M(out string p)
    {
        M(out _f)
    }
}
", @"
class C
{
    private string _f;

    void M(out string p)
    {
        M(out _f)
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId, CodeFixIdentifiers.MakeFieldWritable));
        }

        //[Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.ReadOnlyFieldCannotBePassedAsRefOrOutValue)]
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
