// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1014UseImplicitlyTypedArrayTests : AbstractCSharpDiagnosticVerifier<UseExplicitlyOrImplicitlyTypedArrayAnalyzer, UseExplicitlyOrImplicitlyTypedArrayCodeFixProvider>
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
        var x = new [|string|][] { """" };
    }
}
", @"
class C
{
    void M()
    {
        var x = new[] { """" };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_Implicit));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyOrImplicitlyTypedArray)]
        public async Task Test_TypeIsNotObvious()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        var x = new [|string|][] { M2() };
    }

    string M2() => default;
}
", @"
class C
{
    void M()
    {
        var x = new[] { M2() };
    }

    string M2() => default;
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_Implicit));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyOrImplicitlyTypedArray)]
        public async Task Test_TypeIsObvious()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        var x = new [|string|][] { """" };
    }
}
", @"
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
        public async Task Test_TypesAreNotEqual()
        {
            await VerifyNoDiagnosticAsync(@"
class C : A
{
    void M()
    {
        var x = new A[]
        {
            default(B),
            default(C)
        };
    }
}

class A
{
}

class B : A
{
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_ImplicitWhenTypeIsObvious));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyOrImplicitlyTypedArray)]
        public async Task Test_NestedArray()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    string[][] _f = new [|string[]|][]
    {
        new[] { """" },
    };
}
", @"
class C
{
    string[][] _f = new[]
    {
        new[] { """" },
    };
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_Implicit));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyOrImplicitlyTypedArray)]
        public async Task TestNoDiagnostic_TypeIsObvious()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        var x = new[]
        {
            new[] { """", new string('a', 1) },
            new[] { new string('a', 1), """" }
        };
    }

    string M2() => null;
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
        public async Task TestNoDiagnostic_ArrayInitializer_DerivedType()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        var items = new object[]
        {
            new double[] { 1d },
            new double[] { 1d }
        };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_ImplicitWhenTypeIsObvious));
        }
    }
}
