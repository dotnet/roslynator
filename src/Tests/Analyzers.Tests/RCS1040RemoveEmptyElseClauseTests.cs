// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1040RemoveEmptyElseClauseTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.RemoveEmptyElseClause;

        public override DiagnosticAnalyzer Analyzer { get; } = new RemoveEmptyElseClauseAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new ElseClauseCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveEmptyElseClause)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool f = false;

        if (f)
        {
        }
        [|else
        {
        }|]
    }
}
", @"
class C
{
    void M()
    {
        bool f = false;

        if (f)
        {
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveEmptyElseClause)]
        public async Task TestNoDiagnostic_ElseIf()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool f = false;

        if (f)
        {
        }
        else if (f)
        {
        }
}
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveEmptyElseClause)]
        public async Task TestNoDiagnostic_NonEmptyElse()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool f = false;

        if (f)
        {
        }
        else
        {
            M();
        }
}
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveEmptyElseClause)]
        public async Task TestNoDiagnostic_IfElseEmbededInIfWithElse()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool f = false;

        if (f)
            if (f) M(); else { }
        else
        {
            M();
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveEmptyElseClause)]
        public async Task TestNoDiagnostic_IfElseEmbededInIfWithElse2()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool f = false;

        if (f)
            if (f) M(); else if (f) M(); else { }
        else
        {
            M();
        }
    }
}
");
        }
    }
}
