// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1248UseIsNullPatternInsteadOfComparisonOrViceVersaTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UseIsNullPatternInsteadOfComparisonOrViceVersa;

        protected override DiagnosticAnalyzer Analyzer { get; } = new UseIsNullPatternInsteadOfComparisonOrViceVersaAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new UseIsNullPatternInsteadOfComparisonOrViceVersaCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseIsNullPatternInsteadOfComparisonOrViceVersa)]
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
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseIsNullPatternInsteadOfComparisonOrViceVersa)]
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
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseIsNullPatternInsteadOfComparisonOrViceVersa)]
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

        if (!(s is null))
        {
        }
    }
}
", options: Options.WithEnabled(AnalyzerOptions.UseIsNullPatternInsteadOfInequalityOperator));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseIsNullPatternInsteadOfComparisonOrViceVersa)]
        public async Task Test_NotEqualsToNull2()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = null;

        if ([|null != (s)|])
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

        if (!((s) is null))
        {
        }
    }
}
", options: Options.WithEnabled(AnalyzerOptions.UseIsNullPatternInsteadOfInequalityOperator));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseIsNullPatternInsteadOfComparisonOrViceVersa)]
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
", options: Options.WithEnabled(AnalyzerOptions.UseComparisonInsteadOfIsNullPattern));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseIsNullPatternInsteadOfComparisonOrViceVersa)]
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
", options: Options.WithEnabled(AnalyzerOptions.UseComparisonInsteadOfIsNullPattern));
        }
    }
}
