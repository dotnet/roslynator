// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

#pragma warning disable RCS1090

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1124InlineLocalVariableTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.InlineLocalVariable;

        public override DiagnosticAnalyzer Analyzer { get; } = new InlineLocalVariableAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new LocalDeclarationStatementCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.InlineLocalVariable)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;

class C
{
    IEnumerable<string> M()
    {
        [|var s = """";|]
        yield return s;
    }
}
", @"
using System.Collections.Generic;

class C
{
    IEnumerable<string> M()
    {
        yield return """";
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.InlineLocalVariable)]
        public async Task TestNoDiagnostic_YieldReturnIsNotLastStatement()
        {
            await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;

class C
{
    IEnumerable<string> M()
    {
        var s = """";
        yield return s;
        s = null;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.InlineLocalVariable)]
        public async Task TestNoDiagnostic_ForEachWithAwait()
        {
            await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;
using System.Threading.Tasks;

class C
{
    async Task<IEnumerable<object>> GetAsync()
    {
        var items = await GetAsync();

        foreach (var item in items)
        {
        }

        return null;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.InlineLocalVariable)]
        public async Task TestNoDiagnostic_SwitchWithAwait()
        {
            await VerifyNoDiagnosticAsync(@"
using System.Threading.Tasks;

class C
{
    async Task<string> GetAsync()
    {
        var x = await GetAsync();

        switch (x)
        {
            case """":
                break;
        }

        return null;
    }
}
");
        }
    }
}
