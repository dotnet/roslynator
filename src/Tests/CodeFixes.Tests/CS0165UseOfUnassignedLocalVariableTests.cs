// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS0165UseOfUnassignedLocalVariableTests : AbstractCSharpCompilerDiagnosticFixVerifier<IdentifierNameCodeFixProvider>
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.CS0165_UseOfUnassignedLocalVariable;

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS0165_UseOfUnassignedLocalVariable)]
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
