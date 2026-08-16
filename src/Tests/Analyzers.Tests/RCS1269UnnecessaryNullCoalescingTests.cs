// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1269UnnecessaryNullCoalescingTests : AbstractCSharpDiagnosticVerifier<UnnecessaryNullCoalescingAnalyzer, UnnecessaryNullCoalescingCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.UnnecessaryNullCoalescing;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullCoalescing)]
    public async Task Test_NonNullableParameter()
    {
        await VerifyDiagnosticAndFixAsync(@"
#nullable enable

class C
{
    C(string[] errors)
    {
        Errors = errors [|?? []|];
    }

    string[] Errors { get; }
}
", @"
#nullable enable

class C
{
    C(string[] errors)
    {
        Errors = errors;
    }

    string[] Errors { get; }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullCoalescing)]
    public async Task Test_NonNullableField_CoalesceAssignment()
    {
        await VerifyDiagnosticAndFixAsync("""
#nullable enable

class C
{
    string _context = "";

    void M(string completionContext)
    {
        _context [|??= completionContext|];
    }
}
""", """
#nullable enable

class C
{
    string _context = "";

    void M(string completionContext)
    {
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullCoalescing)]
    public async Task Test_NonNullableProperty_CoalesceAssignment()
    {
        await VerifyDiagnosticAndFixAsync("""
#nullable enable

class C
{
    string Handler { get; set; } = "";

    void M(C other)
    {
        Handler [|??= other.Handler|];
    }
}
""", """
#nullable enable

class C
{
    string Handler { get; set; } = "";

    void M(C other)
    {
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullCoalescing)]
    public async Task Test_NonNullableLocal()
    {
        await VerifyDiagnosticAndFixAsync("""
#nullable enable

class C
{
    void M(string x)
    {
        string y = x [|?? ""|];
    }
}
""", """
#nullable enable

class C
{
    void M(string x)
    {
        string y = x;
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullCoalescing)]
    public async Task Test_CoalesceAssignment_Expression()
    {
        await VerifyDiagnosticAndFixAsync(@"
#nullable enable

class C
{
    string M(string x, string y)
    {
        return x [|??= y|];
    }
}
", @"
#nullable enable

class C
{
    string M(string x, string y)
    {
        return x;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullCoalescing)]
    public async Task Test_NullableLocal_AfterNullCheck()
    {
        await VerifyDiagnosticAndFixAsync("""
#nullable enable

class C
{
    void M(string? s)
    {
        if (s is not null)
        {
            string t = s [|?? ""|];
        }
    }
}
""", """
#nullable enable

class C
{
    void M(string? s)
    {
        if (s is not null)
        {
            string t = s;
        }
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullCoalescing)]
    public async Task Test_NestedCoalesce_OuterOnly()
    {
        await VerifyDiagnosticAndFixAsync(@"
#nullable enable

class C
{
    void M(string? maybe, string notNull, string extra)
    {
        string x = (maybe ?? notNull) [|?? extra|];
    }
}
", @"
#nullable enable

class C
{
    void M(string? maybe, string notNull, string extra)
    {
        string x = (maybe ?? notNull);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullCoalescing)]
    public async Task Test_NestedCoalesce_EntireChain()
    {
        await VerifyDiagnosticAndFixAsync(@"
#nullable enable

class C
{
    void M(string a, string b, string c)
    {
        string x = a [|?? b ?? c|];
    }
}
", @"
#nullable enable

class C
{
    void M(string a, string b, string c)
    {
        string x = a;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullCoalescing)]
    public async Task Test_CoalesceAssignment_EmbeddedIf()
    {
        await VerifyDiagnosticAndFixAsync("""
#nullable enable

class C
{
    string _context = "";

    void M(bool flag, string completionContext)
    {
        if (flag)
            _context [|??= completionContext|];
    }
}
""", """
#nullable enable

class C
{
    string _context = "";

    void M(bool flag, string completionContext)
    {
        if (flag)
        {
        }
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullCoalescing)]
    public async Task Test_CoalesceAssignment_WithCoalesceRight()
    {
        await VerifyDiagnosticAndFixAsync(@"
#nullable enable

class C
{
    void M(string x, string? a, string b)
    {
        x [|??= a ?? b|];
    }
}
", @"
#nullable enable

class C
{
    void M(string x, string? a, string b)
    {
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullCoalescing)]
    public async Task Test_ClassConstraint()
    {
        await VerifyDiagnosticAndFixAsync(@"
#nullable enable

class C
{
    void M<T>(T x, T y) where T : class
    {
        T z = x [|?? y|];
    }
}
", @"
#nullable enable

class C
{
    void M<T>(T x, T y) where T : class
    {
        T z = x;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullCoalescing)]
    public async Task TestNoDiagnostic_NullableParameter()
    {
        await VerifyNoDiagnosticAsync("""
#nullable enable

class C
{
    void M(string? x)
    {
        string y = x ?? "";
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullCoalescing)]
    public async Task TestNoDiagnostic_NullableContextOff()
    {
        await VerifyNoDiagnosticAsync("""
class C
{
    void M(string x)
    {
        string y = x ?? "";
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullCoalescing)]
    public async Task TestNoDiagnostic_StringLiteral()
    {
        await VerifyNoDiagnosticAsync("""
#nullable enable

class C
{
    void M()
    {
        string y = "" ?? "a";
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullCoalescing)]
    public async Task TestNoDiagnostic_This()
    {
        await VerifyNoDiagnosticAsync(@"
#nullable enable

class C
{
    void M()
    {
        C y = this ?? this;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullCoalescing)]
    public async Task TestNoDiagnostic_ObjectCreation()
    {
        await VerifyNoDiagnosticAsync(@"
#nullable enable

class C
{
    void M()
    {
        C y = new C() ?? this;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullCoalescing)]
    public async Task TestNoDiagnostic_NullableValueType()
    {
        await VerifyNoDiagnosticAsync(@"
#nullable enable

class C
{
    void M(int? x)
    {
        if (x is not null)
        {
            int y = x ?? 0;
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullCoalescing)]
    public async Task TestNoDiagnostic_NullableClassConstraint()
    {
        await VerifyNoDiagnosticAsync(@"
#nullable enable

class C
{
    void M<T>(T x, T y) where T : class?
    {
        T z = x ?? y;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullCoalescing)]
    public async Task TestNoDiagnostic_Directives()
    {
        await VerifyNoDiagnosticAsync(@"
#nullable enable

class C
{
    void M(string x, string y, string z)
    {
        string a = x ??
#if DEBUG
            y
#else
            z
#endif
            ;
    }
}
");
    }
}
