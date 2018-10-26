// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS0165UseOfUnassignedLocalVariableTests : AbstractCSharpCompilerDiagnosticFixVerifier
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.UseOfUnassignedLocalVariable;

        public override CodeFixProvider FixProvider { get; } = new IdentifierNameCodeFixProvider();

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.UseOfUnassignedLocalVariable)]
        public async Task Test()
        {
            await VerifyFixAsync(@"
using System;

class C
{
    void M()
    {
        TimeSpan ts;

        if (ts == default)
        {
        }
    }
}
", @"
using System;

class C
{
    void M()
    {
        TimeSpan ts = default;

        if (ts == default)
        {
        }
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }
    }
}
