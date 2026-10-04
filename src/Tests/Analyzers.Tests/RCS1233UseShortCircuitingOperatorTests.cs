// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1233UseShortCircuitingOperatorTests : AbstractCSharpDiagnosticVerifier<UseShortCircuitingOperatorAnalyzer, BinaryExpressionCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.UseShortCircuitingOperator;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseShortCircuitingOperator)]
    public async Task Test_Or()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool f = false;
        bool f2 = false;

        if (f [|||] f2)
        {
        }
    }
}
", @"
class C
{
    void M()
    {
        bool f = false;
        bool f2 = false;

        if (f || f2)
        {
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseShortCircuitingOperator)]
    public async Task Test_And()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool f = false;
        bool f2 = false;

        if (f [|&|] f2)
        {
        }
    }
}
", @"
class C
{
    void M()
    {
        bool f = false;
        bool f2 = false;

        if (f && f2)
        {
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseShortCircuitingOperator)]
    public async Task Test_OrInsideLogicalAnd()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Threading.Tasks;

class C
{
    bool M(Task task)
    {
        return task is not null && task.IsCompleted [|||] task.IsCanceled [|||] task.IsFaulted;
    }
}
", @"
using System.Threading.Tasks;

class C
{
    bool M(Task task)
    {
        return task is not null && (task.IsCompleted || task.IsCanceled || task.IsFaulted);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseShortCircuitingOperator)]
    public async Task Test_SingleOrInsideLogicalAnd()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool f = false;
        bool f2 = false;
        bool f3 = false;

        if (f && f2 [|||] f3)
        {
        }
    }
}
", @"
class C
{
    void M()
    {
        bool f = false;
        bool f2 = false;
        bool f3 = false;

        if (f && (f2 || f3))
        {
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseShortCircuitingOperator)]
    public async Task Test_ParenthesizedLogicalOrAsLeftOperand()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool f = false;
        bool f2 = false;
        bool f3 = false;

        if ((f || f2) [|||] f3)
        {
        }
    }
}
", @"
class C
{
    void M()
    {
        bool f = false;
        bool f2 = false;
        bool f3 = false;

        if (f || f2 || f3)
        {
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseShortCircuitingOperator)]
    public async Task Test_AndInsideOr()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool f = false;
        bool f2 = false;
        bool f3 = false;

        if (f [|&|] f2 [|||] f3)
        {
        }
    }
}
", @"
class C
{
    void M()
    {
        bool f = false;
        bool f2 = false;
        bool f3 = false;

        if ((f && f2) || f3)
        {
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseShortCircuitingOperator)]
    public async Task Test_OrInsideLogicalOr()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool f = false;
        bool f2 = false;
        bool f3 = false;

        if (f || f2 [|||] f3)
        {
        }
    }
}
", @"
class C
{
    void M()
    {
        bool f = false;
        bool f2 = false;
        bool f3 = false;

        if (f || f2 || f3)
        {
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseShortCircuitingOperator)]
    public async Task Test_AndInsideExclusiveOr()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool f = false;
        bool f2 = false;
        bool f3 = false;

        if (f [|&|] f2 ^ f3)
        {
        }
    }
}
", @"
class C
{
    void M()
    {
        bool f = false;
        bool f2 = false;
        bool f3 = false;

        if ((f && f2) ^ f3)
        {
        }
    }
}
");
    }
}
