// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1014UseExplicitlyTypedArrayOrViceVersaTests : AbstractCSharpDiagnosticVerifier<UseExplicitlyTypedArrayOrViceVersaAnalyzer, UseExplicitlyTypedArrayOrViceVersaCodeFixProvider>
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.UseExplicitlyTypedArrayOrViceVersa;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyTypedArrayOrViceVersa)]
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
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyTypedArrayOrViceVersa)]
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
", options: Options.EnableConfigOption(AnalyzerOptions.UseImplicitlyTypedArrayWhenTypeIsObvious.OptionKey));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyTypedArrayOrViceVersa)]
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
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyTypedArrayOrViceVersa)]
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
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyTypedArrayOrViceVersa)]
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
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyTypedArrayOrViceVersa)]
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
", options: Options.EnableConfigOption(AnalyzerOptions.UseImplicitlyTypedArrayWhenTypeIsObvious.OptionKey));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitlyTypedArrayOrViceVersa)]
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
");
        }
    }
}
