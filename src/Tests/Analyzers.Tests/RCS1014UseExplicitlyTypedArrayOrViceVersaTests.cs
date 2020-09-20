// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1014UseExplicitlyTypedArrayOrViceVersaTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UseExplicitlyTypedArrayOrViceVersa;

        public override DiagnosticAnalyzer Analyzer { get; } = new UseExplicitlyTypedArrayOrViceVersaAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new UseExplicitlyTypedArrayOrViceVersaCodeFixProvider();

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
", options: Options.WithEnabled(AnalyzerOptions.UseImplicitlyTypedArrayWhenTypeIsObvious));
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
", options: Options.WithEnabled(AnalyzerOptions.UseImplicitlyTypedArrayWhenTypeIsObvious));
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
