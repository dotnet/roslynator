// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1252NormalizeUsageOfInfiniteLoopTests : AbstractCSharpDiagnosticVerifier<NormalizeUsageOfInfiniteLoopAnalyzer, NormalizeUsageOfInfiniteLoopCodeFixProvider>
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.NormalizeUsageOfInfiniteLoop;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeUsageOfInfiniteLoop)]
        public async Task Test_ForToWhile()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        [|for|] (; ; )
        {
            M();
        }
    }
}
", @"
class C
{
    void M()
    {
        while (true)
        {
            M();
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.InfiniteLoopStyle, ConfigOptionValues.InfiniteLoopStyle_While));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeUsageOfInfiniteLoop)]
        public async Task Test_WhileToFor()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        [|while|] (true)
        {
            M();
        }
    }
}
", @"
class C
{
    void M()
    {
        for (; ; )
        {
            M();
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.InfiniteLoopStyle, ConfigOptionValues.InfiniteLoopStyle_For));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeUsageOfInfiniteLoop)]
        public async Task Test_DoToWhile()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        [|do|]
        {
            M();
        }
        while (true);
    }
}
", @"
class C
{
    void M()
    {
        while (true)
        {
            M();
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.InfiniteLoopStyle, ConfigOptionValues.InfiniteLoopStyle_While));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeUsageOfInfiniteLoop)]
        public async Task Test_DoToFor()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        [|do|]
        {
            M();
        }
        while (true);
    }
}
", @"
class C
{
    void M()
    {
        for (; ; )
        {
            M();
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.InfiniteLoopStyle, ConfigOptionValues.InfiniteLoopStyle_For));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeUsageOfInfiniteLoop)]
        public async Task TestNoDiagnostic_ForToWhile()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        for (; ; )
        {
            M();
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.InfiniteLoopStyle, ConfigOptionValues.InfiniteLoopStyle_For));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeUsageOfInfiniteLoop)]
        public async Task TestNoDiagnostic_ForToWhile2()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        for (; ; )
        {
            M();
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeUsageOfInfiniteLoop)]
        public async Task TestNoDiagnostic_WhileToFor()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        while (true)
        {
            M();
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.InfiniteLoopStyle, ConfigOptionValues.InfiniteLoopStyle_While));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeUsageOfInfiniteLoop)]
        public async Task TestNoDiagnostic_WhileToFor2()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        while (true)
        {
            M();
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeUsageOfInfiniteLoop)]
        public async Task TestNoDiagnostic_DoToWhile()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        do
        {
            M();
        }
        while (true);
    }
}
");
        }
    }
}
