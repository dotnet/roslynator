// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1249UnnecessaryNullForgivingOperatorTests : AbstractCSharpDiagnosticVerifier<UnnecessaryNullForgivingOperatorAnalyzer, TokenCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.UnnecessaryNullForgivingOperator;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullForgivingOperator)]
    public async Task Test_NonNullExpressions()
    {
        await VerifyDiagnosticAndFixAsync("""
#nullable enable

class C
{
    private readonly object _value = new object();

    private object P => new object();

    private object GetValue() => new object();

    object M()
    {
        object value = "value"[|!|];
        value = new()[|!|];
        value = _value[|!|];
        value = P[|!|];
        value = GetValue()[|!|];
        return value;
    }
}
""", """
#nullable enable

class C
{
    private readonly object _value = new object();

    private object P => new object();

    private object GetValue() => new object();

    object M()
    {
        object value = "value";
        value = new();
        value = _value;
        value = P;
        value = GetValue();
        return value;
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullForgivingOperator)]
    public async Task Test_FlowStateNotNull()
    {
        await VerifyDiagnosticAndFixAsync(@"
#nullable enable

using System.Collections.Generic;

class C
{
    object M(Stack<object> stack)
    {
        object? value;

        while (!stack.TryPop(out value))
        {
        }

        return value[|!|];
    }
}
", @"
#nullable enable

using System.Collections.Generic;

class C
{
    object M(Stack<object> stack)
    {
        object? value;

        while (!stack.TryPop(out value))
        {
        }

        return value;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullForgivingOperator)]
    public async Task Test_Property()
    {
        await VerifyDiagnosticAndFixAsync(@"
#nullable enable

class C
{
    string? P { get; set; } [|= null!|]; //x
}
", @"
#nullable enable

class C
{
    string? P { get; set; } //x
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullForgivingOperator)]
    public async Task Test_Property_DefaultLiteral()
    {
        await VerifyDiagnosticAndFixAsync(@"
#nullable enable

class C
{
    string? P { get; set; } [|= default!|];
}
", @"
#nullable enable

class C
{
    string? P { get; set; }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullForgivingOperator)]
    public async Task Test_Property_DefaultExpression()
    {
        await VerifyDiagnosticAndFixAsync(@"
#nullable enable

class C
{
    string? P { get; set; } [|= default(string)!|];
}
", @"
#nullable enable

class C
{
    string? P { get; set; }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullForgivingOperator)]
    public async Task Test_Field()
    {
        await VerifyDiagnosticAndFixAsync(@"
#nullable enable

class C
{
    private string? F [|= null!|]; //x
}
", @"
#nullable enable

class C
{
    private string? F; //x
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullForgivingOperator)]
    public async Task Test_LocalVariable()
    {
        await VerifyDiagnosticAndFixAsync(@"
#nullable enable

class C
{
    void M()
    {
        string? s = null[|!|]; //x
    }
}
", @"
#nullable enable

class C
{
    void M()
    {
        string? s = null; //x
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullForgivingOperator)]
    public async Task Test_Argument()
    {
        await VerifyDiagnosticAndFixAsync(@"
#nullable enable

class C
{
    void M(string? p)
    {
        string? s = null;

        M(s[|!|]);
    }
}
", @"
#nullable enable

class C
{
    void M(string? p)
    {
        string? s = null;

        M(s);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullForgivingOperator)]
    public async Task TestNoDiagnostic_Argument()
    {
        await VerifyNoDiagnosticAsync(@"
#nullable enable

class C
{
    void M(string p)
    {
        string? s = null;

        M(s!);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullForgivingOperator)]
    public async Task TestNoDiagnostic_MaybeNullExpression()
    {
        await VerifyNoDiagnosticAsync(@"
#nullable enable

class C
{
    object M(object? value)
    {
        return value!;
    }

    object M2()
    {
        return default(object)!;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnnecessaryNullForgivingOperator)]
    public async Task TestNoDiagnostic_MaybeNullWhenAttribute()
    {
        await VerifyNoDiagnosticAsync(@"
#nullable enable

using System.Diagnostics.CodeAnalysis;

class C
{
    void M()
    {
        var x = new C();

        M2(x!);
    }

    public bool M2([MaybeNullWhen(true)] C? p)
    {
        return false;
    }
}
");
    }
}
