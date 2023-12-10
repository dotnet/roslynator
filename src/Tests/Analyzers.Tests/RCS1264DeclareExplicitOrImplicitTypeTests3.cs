// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1264DeclareExplicitOrImplicitTypeTests3 : AbstractCSharpDiagnosticVerifier<DeclareExplicitOrImplicitTypeAnalyzer, UseImplicitTypeCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.DeclareExplicitOrImplicitType;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
    public async Task Test_ObjectCreation()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;

class C
{
    void M()
    {
        [|List<object>|] x = new List<object>();
    }
}
", @"
using System.Collections.Generic;

class C
{
    void M()
    {
        var x = new List<object>();
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
    public async Task Test_DefaultExpression_TupleExpression()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        [|(object x, string y)|] = default((object, string));
    }
}
", @"
class C
{
    void M()
    {
        var (x, y) = default((object, string));
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
    public async Task Test_DefaultExpression_TupleExpression_Var()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        [|(object x, var y)|] = default((object, string));
    }
}
", @"
class C
{
    void M()
    {
        var (x, y) = default((object, string));
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
    public async Task Test_ImplicitArrayCreation()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        [|string[]|] x = new[] { ""a"", ""b"" };
    }
}
", @"
class C
{
    void M()
    {
        var x = new[] { ""a"", ""b"" };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
    public async Task Test_ParseMethod()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    void M()
    {
        [|TimeSpan|] timeSpan = TimeSpan.Parse(null);
    }
}
", @"
using System;

class C
{
    void M()
    {
        var timeSpan = TimeSpan.Parse(null);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
    public async Task TestNoDiagnostic()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;
using System.Text.RegularExpressions;

class C
{
    void M()
    {
        const string c = """";
        string value1, value2;
        dynamic x = new object();
        dynamic x2 = c;

        foreach (Match match in Regex.Matches(""input"", ""pattern""))
        {
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_ImplicitWhenTypeIsObvious));
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
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
    public async Task TestNoDiagnostic_DiscardDesignation()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        if (int.TryParse("""", out int result))
        {
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
    public async Task TestNoDiagnostic_ParseMethod()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        string x = C.Parse("""");
    }

    static string Parse(string value) => null;
}
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
    public async Task TestNoDiagnostic_ParseMethod2()
    {
        await VerifyNoDiagnosticAsync(@"
using I = System.Int32;

class C
{
    void M()
    {
        int x = I.Parse("""");
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_ImplicitWhenTypeIsObvious));
    }
}
