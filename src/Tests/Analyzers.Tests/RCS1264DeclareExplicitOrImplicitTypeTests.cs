// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1264DeclareExplicitOrImplicitTypeTests : AbstractCSharpDiagnosticVerifier<DeclareExplicitOrImplicitTypeAnalyzer, DeclareExplicitOrImplicitTypeCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.DeclareExplicitOrImplicitType;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_Implicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_Implicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_Implicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_Implicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_Implicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_Implicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_Implicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_Implicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_Implicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_Implicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_Implicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_Implicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_Implicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
    public async Task TestNoDiagnostic_FixedStatement()
    {
        await VerifyNoDiagnosticAsync(@"
using System;
using System.Runtime.InteropServices;

public class Foo
{
    public unsafe Foo(string p)
    {
        var span = p.AsSpan();

        fixed (char* ptr = &MemoryMarshal.GetReference(span))
        {
        }
    }
}
", options: Options.WithAllowUnsafe(true));
    }
}
