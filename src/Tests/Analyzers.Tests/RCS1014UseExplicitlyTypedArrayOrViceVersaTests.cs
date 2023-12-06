// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1014UseExplicitlyTypedArrayOrViceVersaTests : AbstractCSharpDiagnosticVerifier<UseExplicitlyOrImplicitlyTypedArrayAnalyzer, UseExplicitlyOrImplicitlyTypedArrayCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyOrImplicitlyTypedArray)]
    public async Task Test()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        var x = [|new[]|] { """" };
    }
}
", @"
class C
{
    void M()
    {
        var x = new string[] { """" };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyOrImplicitlyTypedArray)]
    public async Task Test_TypeIsNotObvious()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        var x = [|new[]|] { M2() };
    }

    string M2() => null;
}
", @"
class C
{
    void M()
    {
        var x = new string[] { M2() };
    }

    string M2() => null;
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyOrImplicitlyTypedArray)]
    public async Task Test_NestedArray()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string[][] _f = [|new[]|]
    {
        /**/[|new[]|] { """" },
    };
}
", @"
class C
{
    string[][] _f = new string[][]
    {
        /**/new string[] { """" },
    };
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyOrImplicitlyTypedArray)]
    public async Task Test_UnnecessaryCast()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C : A
{
    void M()
    {
        var x = [|new[]|] { default(B), (A)default(C) };
    }
}

class A
{
}

class B : A
{
}
", @"
class C : A
{
    void M()
    {
        var x = new A[] { default(B), default(C) };
    }
}

class A
{
}

class B : A
{
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyOrImplicitlyTypedArray)]
    public async Task TestNoDiagnostic_AnonymousType()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        var x = new[] { new { Value = """" } };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyOrImplicitlyTypedArray)]
    public async Task TestNoDiagnostic_TypeIsObvious()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        var x = new[] { """" };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyOrImplicitlyTypedArray)]
    public async Task TestNoDiagnostic_TypeIsObvious_Parse()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        var x = new[] { int.Parse("""") };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyOrImplicitlyTypedArray)]
    public async Task TestNoDiagnostic_TypeIsNotObvious_Parse()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        var x = new int[] { int.Parse(""""), C.Parse() };
    }

    private static int Parse()
    {
        return 0;
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyOrImplicitlyTypedArray)]
    public async Task TestNoDiagnostic_NoInitializer()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        var items = new string[0];
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_Implicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyOrImplicitlyTypedArray)]
    public async Task TestNoDiagnostic_AssignmentToObject()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        object o = new[] { "", "" };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_Implicit)
            .AddConfigOption(ConfigOptionKeys.UseCollectionExpression, true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyOrImplicitlyTypedArray)]
    public async Task TestNoDiagnostic_ForEachExpression()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        foreach (string item in new[] { "", "" })
        {
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_Implicit)
            .AddConfigOption(ConfigOptionKeys.UseCollectionExpression, true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyOrImplicitlyTypedArray)]
    public async Task Test_ExplicitToCollectionExpression_ImplicitStyle()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(string[] arr)
    {
        arr = new [|string|][] { """" };
    }
}
", @"
class C
{
    void M(string[] arr)
    {
        arr = [""""];
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_Implicit)
            .AddConfigOption(ConfigOptionKeys.UseCollectionExpression, true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyOrImplicitlyTypedArray)]
    public async Task Test_ExplicitToCollectionExpression_ImplicitWhenObviousStyle()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string[] P { get; } = new [|string|][] { """" };
}
", @"
class C
{
    string[] P { get; } = [""""];
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_ImplicitWhenTypeIsObvious)
            .AddConfigOption(ConfigOptionKeys.UseCollectionExpression, true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyOrImplicitlyTypedArray)]
    public async Task Test_ImplicitToCollectionExpression_ImplicitStyle()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(string[] arr)
    {
        arr = [|new[]|] { """" };
    }
}
", @"
class C
{
    void M(string[] arr)
    {
        arr = [""""];
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_Implicit)
            .AddConfigOption(ConfigOptionKeys.UseCollectionExpression, true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyOrImplicitlyTypedArray)]
    public async Task Test_ImplicitToCollectionExpression_ImplicitWhenObviousStyle()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string[] P { get; } = [|new[]|] { """" };
}
", @"
class C
{
    string[] P { get; } = [""""];
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_ImplicitWhenTypeIsObvious)
            .AddConfigOption(ConfigOptionKeys.UseCollectionExpression, true));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyOrImplicitlyTypedArray)]
    public async Task Test_CollectionExpressionToExplicit_ImplicitStyle()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(string[] arr)
    {
        arr = [|[""""]|];
    }
}
", @"
class C
{
    void M(string[] arr)
    {
        arr = new string[] { """" };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyOrImplicitlyTypedArray)]
    public async Task Test_CollectionExpressionToExplicit_ImplicitWhenObviousStyle()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    private string _f = """";

    void M(string[] arr)
    {
        arr = [|[_f]|];
    }
}
", @"
class C
{
    private string _f = """";

    void M(string[] arr)
    {
        arr = new string[] { _f };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_ImplicitWhenTypeIsObvious));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyOrImplicitlyTypedArray)]
    public async Task Test_CollectionExpressionToImplicit_ImplicitStyle()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(string[] arr)
    {
        arr = [|[""""]|];
    }
}
", @"
class C
{
    void M(string[] arr)
    {
        arr = new[] { """" };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_Implicit)
            .AddConfigOption(ConfigOptionKeys.UseCollectionExpression, false));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyOrImplicitlyTypedArray)]
    public async Task Test_CollectionExpressionToImplicit_ImplicitWhenObviousStyle()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string[] P { get; } = [|[""""]|];
}
", @"
class C
{
    string[] P { get; } = new[] { """" };
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_ImplicitWhenTypeIsObvious)
            .AddConfigOption(ConfigOptionKeys.UseCollectionExpression, false));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyOrImplicitlyTypedArray)]
    public async Task Test_CollectionExpressionToImplicit_ImplicitWhenObviousStyle2()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M(string p1, object p2)
    {
        var nodes = new[] { p1, p2 };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_Implicit)
            .AddConfigOption(ConfigOptionKeys.UseCollectionExpression, true));
    }
}
