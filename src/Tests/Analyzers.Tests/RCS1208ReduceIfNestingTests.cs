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
    public async Task Test_WhenParentIsConstructor()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    C(bool p)
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
    C(bool p)
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
    public async Task Test_IsWhenIsNotIsInvalid()
    {
        await VerifyDiagnosticAndFixAsync(@"
class X
{
}

class C
{
    public X X { get; set; }

    C(object o)
    {
        [|if|] (o is X)
        {
            M2();
        }
    }

    void M2()
    {
    }
}
", @"
class X
{
}

class C
{
    public X X { get; set; }

    C(object o)
    {
        if (o is not global::X)
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
    public async Task Test_WhenParentIsConversionOperator()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    static bool b=false;
    public static implicit operator bool(C c)
    {
        [|if|] (b)
        {
            return M2();
        }
        return false;
    }

    static bool M2()
    {
        return true;
    }
}
", @"
class C
{
    static bool b=false;
    public static implicit operator bool(C c)
    {
        if (!b)
        {
            return false;
        }

        return M2();
    }

    static bool M2()
    {
        return true;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ReduceIfNesting)]
    public async Task Test_WhenParentIsGetAccessor()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    static bool b=false;
    public static bool s {
        get {
            [|if|] (b) {
                return M2();
            }
            return false;
        }  
    }

    static bool M2()
    {
        return true;
    }
}
", @"
class C
{
    static bool b=false;
    public static bool s {
        get
        {
            if (!b)
            {
                return false;
            }

            return M2();
        }
    }

    static bool M2()
    {
        return true;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ReduceIfNesting)]
    public async Task Test_WhenParentIsLambda()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(bool p)
    {
        var f = () => 
            {
                [|if|] (p)
                {
                    M2();
                }
            };
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
        var f = () =>
            {
                if (!p)
                {
                    return;
                }

                M2();
            };
    }

    void M2()
    {
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ReduceIfNesting)]
    public async Task Test_WhenParentIsLocalFunction()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(bool p)
    {
        void M3()
        {
            [|if|] (p)
            {
                M2();
            }
        }
        M3();
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
        void M3()
        {
            if (!p)
            {
                return;
            }

            M2();
        }
        M3();
    }

    void M2()
    {
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ReduceIfNesting)]
    public async Task Test_WhenParentIsMethod()
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
    public async Task TestNoDiagnostic_OverlappingLocalVariables_WhenParentIsConstructor()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    C(bool p)
    {
        if (p)
        {
            var s = 1;
        }

        if (p)
        {
            var s = 2;
            M2();
        }
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
    public async Task TestNoDiagnostic_OverlappingLocalVariables_WhenParentIsConversionOperator()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    static bool b=false;
    public static implicit operator bool(C c)
    {
        if (b)
        {
            var s = 1;
        }
        if (b)
        {
            var s = 2;
            return M2();
        }
        return false;
    }

    static bool M2()
    {
        return true;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ReduceIfNesting)]
    public async Task TestNoDiagnostic_OverlappingLocalVariables_WhenParentIsGetAccessor()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    static bool b=false;
    public static bool s {
        get {
            if (b)
            {
                var s = 1;
            }

            if (b)
            {
                var s = 2;
                return M2();
            }
            return false;
        }  
    }

    static bool M2()
    {
        return true;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ReduceIfNesting)]
    public async Task TestNoDiagnostic_OverlappingLocalVariables_WhenParentIsLambda()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M(bool p)
    {
        var f = () => 
        {
            if (p)
            {
                var s = 1;
            }

            if (p)
            {
                var s = 2;
                M2();
            }
        };
    }

    void M2()
    {
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ReduceIfNesting)]
    public async Task TestNoDiagnostic_OverlappingLocalVariables_WhenParentIsLocalFunction()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M(bool p)
    {
        void M3()
        {

            if (p)
            {
                var s = 1;
            }

            if (p)
            {
                var s = 2;
                M2();
            }

        }
        M3();
    }

    void M2()
    {
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ReduceIfNesting)]
    public async Task TestNoDiagnostic_OverlappingLocalVariables_WhenParentIsMethod()
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

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ReduceIfNesting)]
    public async Task TestDiagnostic_DoesNot_IncorrectlyRecurse()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(bool p, bool q, bool r)
    {
        [|if|] (r)
        {
            if (p)
            {
                var x = 1;
                M2();
            }

            if (q)
            {
                var x = 2;
                M2();
            }
        }
    }

    void M2()
    {
    }
}
", @"
class C
{
    void M(bool p, bool q, bool r)
    {
        if (!r)
        {
            return;
        }

        if (p)
        {
            var x = 1;
            M2();
        }

        if (q)
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

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ReduceIfNesting)]
    public async Task Test_WhenIsExpressionCsharp8()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(object o)
    {
        [|if|] (o is string)
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
    void M(object o)
    {
        if (!(o is string))
        {
            return;
        }

        M2();
    }

    void M2()
    {
    }
}
", options: WellKnownCSharpTestOptions.Default_CSharp8);
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ReduceIfNesting)]
    public async Task Test_WhenIsExpression()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(object o)
    {
        [|if|] (o is string)
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
    void M(object o)
    {
        if (o is not string)
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
}

