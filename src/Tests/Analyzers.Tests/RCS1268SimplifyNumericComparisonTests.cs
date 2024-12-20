// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.CSharp.CSharp.Analysis;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1268SimplifyNumericComparisonTests : AbstractCSharpDiagnosticVerifier<SimplifyNumericComparisonAnalyzer, BinaryExpressionCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.SimplifyNumericComparison;

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.SimplifyNumericComparison)]
    public async Task Test_LeftSideSubtraction_EqualsZero()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(int a, int b)
    {
        if ([|(a - b) == 0|])
        {
        }
    }
}
", @"
class C
{
    void M(int a, int b)
    {
        if (a == b)
        {
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.SimplifyNumericComparison)]
    public async Task Test_RightSideSubtraction_EqualsZero()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(int a, int b)
    {
        if ([|0 == (a - b)|])
        {
        }
    }
}
", @"
class C
{
    void M(int a, int b)
    {
        if (a == b)
        {
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.SimplifyNumericComparison)]
    public async Task Test_LeftSideSubtraction_WithoutParenthesis_EqualsZero()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(int a, int b)
    {
        if ([|a - b == 0|])
        {
        }
    }
}
", @"
class C
{
    void M(int a, int b)
    {
        if (a == b)
        {
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.SimplifyNumericComparison)]
    public async Task Test_LeftSideSubtraction_SubtractionGreaterThanZero()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(int a, int b)
    {
        if ([|(a - b) > 0|])
        {
        }
    }
}
", @"
class C
{
    void M(int a, int b)
    {
        if (a > b)
        {
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.SimplifyNumericComparison)]
    public async Task Test_RightSideSubtraction_SubtractionSmallerThanZero()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(int a, int b)
    {
        if ([|0 > (a - b)|])
        {
        }
    }
}
", @"
class C
{
    void M(int a, int b)
    {
        if (a < b)
        {
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.SimplifyNumericComparison)]
    public async Task TestSubtractionLessThanZero()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(int a, int b)
    {
        if ([|(a - b) < 0|])
        {
        }
    }
}
", @"
class C
{
    void M(int a, int b)
    {
        if (a < b)
        {
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.SimplifyNumericComparison)]
    public async Task TestSubtractionGreaterThanOrEqualToZero()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(int a, int b)
    {
        if ([|(a - b) >= 0|])
        {
        }
    }
}
", @"
class C
{
    void M(int a, int b)
    {
        if (a >= b)
        {
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.SimplifyNumericComparison)]
    public async Task TestSubtractionLessThanOrEqualToZero()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(int a, int b)
    {
        if ([|(a - b) <= 0|])
        {
        }
    }
}
", @"
class C
{
    void M(int a, int b)
    {
        if (a <= b)
        {
        }
    }
}
");
    }
}
