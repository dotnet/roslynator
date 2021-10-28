// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1049SimplifyBooleanComparisonTests : AbstractCSharpDiagnosticVerifier<BooleanLiteralAnalyzer, SimplifyBooleanComparisonCodeFixProvider>
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.SimplifyBooleanComparison;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyBooleanComparison)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(bool x)
    {
        if (
#if DEBUG
            x &&
#endif
            [|x == false|])
        {
        }
    }
}
", @"
class C
{
    void M(bool x)
    {
        if (
#if DEBUG
            x &&
#endif
            !x)
        {
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyBooleanComparison)]
        public async Task Test2()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(bool x)
    {
        if (
#if RELEASE
            x &&
#endif
            [|x == false|])
        {
        }
    }
}
", @"
class C
{
    void M(bool x)
    {
        if (
#if RELEASE
            x &&
#endif
            !x)
        {
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyBooleanComparison)]
        public async Task Test_IsTrue()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = false;
        if ([|x is true|]) { }
    }
}
", @"
class C
{
    void M()
    {
        bool x = false;
        if (x) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyBooleanComparison)]
        public async Task Test_IsFalse()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = false;
        if ([|x is false|]) { }
    }
}
", @"
class C
{
    void M()
    {
        bool x = false;
        if (!x) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyBooleanComparison)]
        public async Task Test_IsNotTrue()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = false;
        if ([|x is not true|]) { }
    }
}
", @"
class C
{
    void M()
    {
        bool x = false;
        if (!x) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyBooleanComparison)]
        public async Task Test_IsNotFalse()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = false;
        if ([|x is not false|]) { }
    }
}
", @"
class C
{
    void M()
    {
        bool x = false;
        if (x) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyBooleanComparison)]
        public async Task TestNoDiagnostic_NullableIsTrue()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool? x = null;
        if (x is true) { }
    }
}
");
        }
    }
}
