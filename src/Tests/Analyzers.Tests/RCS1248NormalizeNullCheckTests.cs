// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1248NormalizeNullCheckTests : AbstractCSharpDiagnosticVerifier<NormalizeNullCheckAnalyzer, NormalizeNullCheckProvider>
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.NormalizeNullCheck;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeNullCheck)]
        public async Task Test_EqualsToNull()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = null;

        if ([|s == null|])
        {
        }
    }
}
", @"
class C
{
    void M()
    {
        string s = null;

        if (s is null)
        {
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.NullCheckStyle, ConfigOptionValues.NullCheckStyle_PatternMatching));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeNullCheck)]
        public async Task Test_EqualsToNull2()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = null;

        if (
            [|null == (s)|]
            )
        {
        }
    }
}
", @"
class C
{
    void M()
    {
        string s = null;

        if (
            (s) is null
            )
        {
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.NullCheckStyle, ConfigOptionValues.NullCheckStyle_PatternMatching));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeNullCheck)]
        public async Task Test_NotEqualsToNull()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = null;

        if ([|s != null|])
        {
        }
    }
}
", @"
class C
{
    void M()
    {
        string s = null;

        if (s is not null)
        {
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.NullCheckStyle, ConfigOptionValues.NullCheckStyle_PatternMatching));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeNullCheck)]
        public async Task Test_IsNull()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = null;

        if ([|s is null|])
        {
        }
    }
}
", @"
class C
{
    void M()
    {
        string s = null;

        if (s == null)
        {
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.NullCheckStyle, ConfigOptionValues.NullCheckStyle_EqualityOperator));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeNullCheck)]
        public async Task Test_NotIsNull()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = null;

        if (!([|s is null|]))
        {
        }
    }
}
", @"
class C
{
    void M()
    {
        string s = null;

        if (s != null)
        {
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.NullCheckStyle, ConfigOptionValues.NullCheckStyle_EqualityOperator));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeNullCheck)]
        public async Task TestNoDiagnostic_NotEqualsToNull_CSharp8()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        string s = null;

        if (s != null)
        {
        }
    }
}
", options: WellKnownCSharpTestOptions.Default_CSharp8
                .AddConfigOption(ConfigOptionKeys.NullCheckStyle, ConfigOptionValues.NullCheckStyle_PatternMatching));
        }
    }
}
