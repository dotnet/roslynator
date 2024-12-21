// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1264UseVarOrExplicitTypeTests4 : AbstractCSharpDiagnosticVerifier<UseVarOrExplicitTypeAnalyzer, UseVarOrExplicitTypeCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.UseVarOrExplicitType;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarOrExplicitType)]
    public async Task Test_LocalVariable()
    {
        await VerifyDiagnosticAndFixAsync("""
class C
{
    void M()
    {
        var a = "a";
        [|var|] s = a;
    }
}
""", """
class C
{
    void M()
    {
        var a = "a";
        string s = a;
    }
}
""", options: Options.AddConfigOption(ConfigOptionKeys.UseVar, ConfigOptionValues.UseVar_WhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarOrExplicitType)]
    public async Task Test_DeclarationExpression()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    void M()
    {
        string value = null;
        if (DateTime.TryParse(value, out [|var|] result)) { }
    }
}
", @"
using System;

class C
{
    void M()
    {
        string value = null;
        if (DateTime.TryParse(value, out DateTime result)) { }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.UseVar, ConfigOptionValues.UseVar_WhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarOrExplicitType)]
    public async Task Test_Tuple()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Collections.Generic;

class C
{
    (IEnumerable<DateTime> e1, string e2) M()
    {
        [|var|] x = M();

        return default((IEnumerable<DateTime>, string));
    }
}
", @"
using System;
using System.Collections.Generic;

class C
{
    (IEnumerable<DateTime> e1, string e2) M()
    {
        (IEnumerable<DateTime> e1, string e2) x = M();

        return default((IEnumerable<DateTime>, string));
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.UseVar, ConfigOptionValues.UseVar_WhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarOrExplicitType)]
    public async Task Test_Parameter_NullableReferenceType()
    {
        await VerifyDiagnosticAndFixAsync(@"
 #nullable enable
class C
{
    void M(string? p)
    {
        [|var|] s = p;
    }
}
", @"
 #nullable enable
class C
{
    void M(string? p)
    {
        string? s = p;
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.UseVar, ConfigOptionValues.UseVar_WhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarOrExplicitType)]
    public async Task Test_Parameter_NullableReferenceType_Disable()
    {
        await VerifyDiagnosticAndFixAsync(@"
#nullable disable

class C
{
    void M(string? p)
    {
        [|var|] s = p;
    }
}
", @"
#nullable disable

class C
{
    void M(string? p)
    {
        string s = p;
    }
}
",
options: WellKnownCSharpTestOptions.Default_NullableReferenceTypes
            .AddConfigOption(ConfigOptionKeys.UseVar, ConfigOptionValues.UseVar_WhenTypeIsObvious)
            .AddAllowedCompilerDiagnosticId("CS8632"));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarOrExplicitType)]
    public async Task Test_Tuple_DeclarationExpression()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    (object x, System.DateTime y) M()
    {
        [|var|] (x, y) = M();

        return default;
    }
}
", @"
class C
{
    (object x, System.DateTime y) M()
    {
        (object x, System.DateTime y) = M();

        return default;
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.UseVar, ConfigOptionValues.UseVar_WhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarOrExplicitType)]
    public async Task Test_TupleExpression()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    (object x, System.DateTime y) M()
    {
        (object x, [|var|] y) = M();

        return default;
    }
}
", @"
class C
{
    (object x, System.DateTime y) M()
    {
        (object x, System.DateTime y) = M();

        return default;
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.UseVar, ConfigOptionValues.UseVar_WhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarOrExplicitType)]
    public async Task Test_TupleExpression_AllVar()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    (object x, System.DateTime y) M()
    {
        ([|var|] x, [|var|] y) = M();

        return default;
    }
}
", @"
class C
{
    (object x, System.DateTime y) M()
    {
        (object x, System.DateTime y) = M();

        return default;
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.UseVar, ConfigOptionValues.UseVar_WhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarOrExplicitType)]
    public async Task Test_DiscardDesignation()
    {
        await VerifyDiagnosticAndFixAsync("""
class C
{
    void M()
    {
        if (int.TryParse("", out [|var|] result))
        {
        }
    }
}
""", """
class C
{
    void M()
    {
        if (int.TryParse("", out int result))
        {
        }
    }
}
""", options: Options.AddConfigOption(ConfigOptionKeys.UseVar, ConfigOptionValues.UseVar_WhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarOrExplicitType)]
    public async Task Test_Func_Lambda()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        [|var|] x = () =>
        {
            return default(object);
        };
    }
}
", @"
class C
{
    void M()
    {
        System.Func<object> x = () =>
        {
            return default(object);
        };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.UseVar, ConfigOptionValues.UseVar_WhenTypeIsObvious)
            .AddAllowedCompilerDiagnosticId("CS8603"));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarOrExplicitType)]
    public async Task Test_Func_Lambda_Nullable()
    {
        await VerifyDiagnosticAndFixAsync(@"
#nullable enable

class C
{
    void M()
    {
        [|var|] x = () =>
        {
            return default(object);
        };
    }
}
", @"
#nullable enable

class C
{
    void M()
    {
        System.Func<object> x = () =>
        {
            return default(object);
        };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.UseVar, ConfigOptionValues.UseVar_WhenTypeIsObvious)
            .AddAllowedCompilerDiagnosticId("CS8603"));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarOrExplicitType)]
    public async Task TestNoDiagnostic()
    {
        await VerifyNoDiagnosticAsync(@"
using System;

class C
{
    void M()
    {
        string s = null;

        string value = null;
        if (DateTime.TryParse(s, out DateTime result))
        {
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.UseVar, ConfigOptionValues.UseVar_WhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarOrExplicitType)]
    public async Task TestNoDiagnostic_ObjectCreation()
    {
        await VerifyNoDiagnosticAsync(@"
#nullable enable

class C
{
    void M()
    {
        var s = new string(' ', 1);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.UseVar, ConfigOptionValues.UseVar_WhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarOrExplicitType)]
    public async Task TestNoDiagnostic_ForEach()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;

class C
{
    IEnumerable<(object x, System.DateTime y)> M()
    {
        foreach (var (x, y) in M())
        {
        }

        return default;
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.UseVar, ConfigOptionValues.UseVar_Always));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarOrExplicitType)]
    public async Task TestNoDiagnostic_ParseMethod()
    {
        await VerifyNoDiagnosticAsync(@"
using System;

class C
{
    void M()
    {
        var timeSpan = TimeSpan.Parse(null);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.UseVar, ConfigOptionValues.UseVar_WhenTypeIsObvious));
    }
}
