// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1176UseVarInsteadOfExplicitTypeWhenTypeIsNotObviousTests : AbstractCSharpDiagnosticVerifier<UseVarInsteadOfExplicitTypeWhenTypeIsNotObviousAnalyzer, UseVarInsteadOfExplicitTypeCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.UseVarInsteadOfExplicitTypeWhenTypeIsNotObvious;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarInsteadOfExplicitTypeWhenTypeIsNotObvious)]
    public async Task Test()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    object M()
    {
        [|object|] x = M();

        return default;
    }
}
", @"
class C
{
    object M()
    {
        var x = M();

        return default;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarInsteadOfExplicitTypeWhenTypeIsNotObvious)]
    public async Task Test_TupleExpression()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    (object x, object y) M()
    {
        [|(object x, object y)|] = M();

        return default;
    }
}
", @"
class C
{
    (object x, object y) M()
    {
        var (x, y) = M();

        return default;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarInsteadOfExplicitTypeWhenTypeIsNotObvious)]
    public async Task Test_TupleExpression_Var()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    (object x, object y) M()
    {
        [|(var x, object y)|] = M();

        return default;
    }
}
", @"
class C
{
    (object x, object y) M()
    {
        var (x, y) = M();

        return default;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarInsteadOfExplicitTypeWhenTypeIsNotObvious)]
    public async Task Test_DiscardDesignation()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        if (int.TryParse("""", out [|int|] result))
        {
        }
    }
}
", @"
class C
{
    void M()
    {
        if (int.TryParse("""", out var result))
        {
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarInsteadOfExplicitTypeWhenTypeIsNotObvious)]
    public async Task Test_TryParse_GenericType()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
#nullable enable

class C
{
    void M()
    {
        bool TryParse<T>(string? s, out T t)
        {
            t = default!;
            return false;
        }

        TryParse<IntPtr>(""wasted"", out [|IntPtr|] i);
    }
}
", @"
using System;
#nullable enable

class C
{
    void M()
    {
        bool TryParse<T>(string? s, out T t)
        {
            t = default!;
            return false;
        }

        TryParse<IntPtr>(""wasted"", out var i);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarInsteadOfExplicitTypeWhenTypeIsNotObvious)]
    public async Task TestNoDiagnostic_ForEach_DeclarationExpression()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;

class C
{
    IEnumerable<(object x, string y)> M()
    {
        foreach (var (x, y) in M())
        {
        }

        return default;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarInsteadOfExplicitTypeWhenTypeIsNotObvious)]
    public async Task TestNoDiagnostic_ForEach_TupleExpression()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;

class C
{
    IEnumerable<(object x, string y)> M()
    {
        foreach ((object x, string y) in M())
        {
        }

        return default;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarInsteadOfExplicitTypeWhenTypeIsNotObvious)]
    public async Task TestNoDiagnostic_ParseMethod()
    {
        await VerifyNoDiagnosticAsync(@"
using System;

class C
{
    void M()
    {
        TimeSpan timeSpan = TimeSpan.Parse(null);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarInsteadOfExplicitTypeWhenTypeIsNotObvious)]
    public async Task TestNoDiagnostic_SpanStackAlloc()
    {
        await VerifyNoDiagnosticAsync(@"
using System;

class C
{
    void M()
    {
        Span<char> span = stackalloc char[1];
        ReadOnlySpan<char> readonlySpan = stackalloc char[1];
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarInsteadOfExplicitTypeWhenTypeIsNotObvious)]
    public async Task TestNoDiagnostic_DefaultLiteralWithSuppressNullableWarning()
    {
        await VerifyNoDiagnosticAsync(@"
using System;

class C
{
    void M<T>()
    {
        T result = default!;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarInsteadOfExplicitTypeWhenTypeIsNotObvious)]
    public async Task TestNoDiagnostic_ImplicitObjectCreationExpression()
    {
        await VerifyNoDiagnosticAsync(@"
using System;

class C
{
    void M()
    {
        C x = new();
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarInsteadOfExplicitTypeWhenTypeIsNotObvious)]
    public async Task TestNoDiagnostic_NullableReferenceType()
    {
        await VerifyNoDiagnosticAsync(@"
using System;

#nullable enable

class C
{
    void M()
    {
        var type = typeof(int);
        Type? nullableType = type;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarInsteadOfExplicitTypeWhenTypeIsNotObvious)]
    public async Task TestNoDiagnostic_InferredType_Invocation_IdentifierName()
    {
        await VerifyNoDiagnosticAsync(@"
using System;
#nullable enable

class C
{
    void M()
    {
        bool TryParse<T>(string? s, out T t)
        {
            t = default!;
            return false;
        }

        TryParse(""wasted"", out IntPtr i);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarInsteadOfExplicitTypeWhenTypeIsNotObvious)]
    public async Task TestNoDiagnostic_InferredType_Invocation_MemberAccessExpression()
    {
        await VerifyNoDiagnosticAsync(@"
using System;
#nullable enable

static class C
{
    static void M()
    {

        C.TryParse(""wasted"", out IntPtr i);
    }

    static bool TryParse<T>(string? s, out T t)
    {
        t = default!;
        return false;
    }
}
");
    }
}
