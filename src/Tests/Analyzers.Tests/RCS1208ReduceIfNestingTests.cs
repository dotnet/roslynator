// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1208ReduceIfNestingTests : AbstractCSharpDiagnosticVerifier<ReduceIfNestingAnalyzer, IfStatementCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.ReduceIfNesting;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ReduceIfNesting)]
    public async Task Test()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(bool p)
    {
        [|if|] (p)
        {
            M2();
        }
    }

    void M2()
    {
    }
}
", @"
class C
{
    void M(bool p)
    {
        if (!p)
        {
            return;
        }

        M2();
    }

    void M2()
    {
    }
}
");
    }
    
    
    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ReduceIfNesting)]
    public async Task Test_InvertingCoalesceToFalse()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(bool? p)
    {
        [|if|] (p??false)
        {
            M2();
        }
    }

    void M2()
    {
    }
}
", @"
class C
{
    void M(bool? p)
    {
        if (p != true)
        {
            return;
        }

        M2();
    }

    void M2()
    {
    }
}
");
    }
    
    
    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ReduceIfNesting)]
    public async Task Test_InvertingCoalesceToTrue()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(bool? p)
    {
        [|if|] (p??true)
        {
            M2();
        }
    }

    void M2()
    {
    }
}
", @"
class C
{
    void M(bool? p)
    {
        if (p == false)
        {
            return;
        }

        M2();
    }

    void M2()
    {
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ReduceIfNesting)]
    public async Task Test_InvertingCoalesceToUnknown()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    bool b { get; set; }

    void M(bool? p)
    {
        [|if|] (p??b)
        {
            M2();
        }
    }

    void M2()
    {
    }
}
", @"
class C
{
    bool b { get; set; }

    void M(bool? p)
    {
        if (!(p ?? b))
        {
            return;
        }

        M2();
    }

    void M2()
    {
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ReduceIfNesting)]
    public async Task TestNoDiagnostic_OverlappingLocalVariables()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M(bool p, bool q)
    {
        if (p)
        {
            var x = 1;
            M2();
        }

        if(q)
        {
            var x = 2;
            M2();
        }

    }

    void M2()
    {
    }
}
");
    }
}
