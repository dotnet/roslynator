// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1004RemoveBracesFromIfElseTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.RemoveBracesFromIfElse;

        public override DiagnosticAnalyzer Analyzer { get; } = new RemoveBracesFromIfElseAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new RemoveBracesFromIfElseCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveBracesFromIfElse)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool f = false;

        [|if (f)
        {
            M();
        }
        else if (f)
        {
            M();
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
            M();
        else if (f)
            M();
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveBracesFromIfElse)]
        public async Task Test2()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool f = false;

        [|if (f)
        {
            if (f) M(); else M();
        }
        else if (f)
        {
            M();
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
            if (f) M(); else M();
        else if (f)
            M();
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveBracesFromIfElse)]
        public async Task TestNoDiagnostic_SimpleIfInsideIfWithElse()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool f = false;

        if (f)
        {
            if (f) M();
        }
        else if (f)
        {
            M();
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveBracesFromIfElse)]
        public async Task TestNoDiagnostic_SimpleIfInsideIfWithElse2()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool f = false;

        if (f)
        {
            if (f) M(); else if (f) M();
        }
        else if (f)
        {
            M();
        }
    }
}
");
        }
    }
}
