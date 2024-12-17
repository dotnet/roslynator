// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Formatting.CodeFixes.CSharp;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests;

public class RCS0062PutExpressionBodyOnItsOwnLineTests : AbstractCSharpDiagnosticVerifier<PutExpressionBodyOnItsOwnLineAnalyzer, SyntaxTokenCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.PutExpressionBodyOnItsOwnLine;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PutExpressionBodyOnItsOwnLine)]
    public async Task Test_Constructor_BreakAfterArrow()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    public C() =>[| |]M();

    void M() { }
}
", @"
class C
{
    public C() =>
        M();

    void M() { }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrowTokenNewLine, ConfigOptionValues.ArrowTokenNewLine_After));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PutExpressionBodyOnItsOwnLine)]
    public async Task Test_Constructor_BreakBeforeArrow()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    public C()[| |]=> M();

    void M() { }
}
", @"
class C
{
    public C()
        => M();

    void M() { }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrowTokenNewLine, ConfigOptionValues.ArrowTokenNewLine_Before));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PutExpressionBodyOnItsOwnLine)]
    public async Task Test_Destructor_BreakAfterArrow()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    ~C() =>[| |]M();

    void M() { }
}
", @"
class C
{
    ~C() =>
        M();

    void M() { }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrowTokenNewLine, ConfigOptionValues.ArrowTokenNewLine_After));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PutExpressionBodyOnItsOwnLine)]
    public async Task Test_Destructor_BreakBeforeArrow()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    ~C()[| |]=> M();

    void M() { }
}
", @"
class C
{
    ~C()
        => M();

    void M() { }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrowTokenNewLine, ConfigOptionValues.ArrowTokenNewLine_Before));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PutExpressionBodyOnItsOwnLine)]
    public async Task Test_Method_BreakAfterArrow()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string M() =>[| |]null;
}
", @"
class C
{
    string M() =>
        null;
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrowTokenNewLine, ConfigOptionValues.ArrowTokenNewLine_After));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PutExpressionBodyOnItsOwnLine)]
    public async Task Test_Method_BreakBeforeArrow()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string M()[| |]=> null;
}
", @"
class C
{
    string M()
        => null;
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrowTokenNewLine, ConfigOptionValues.ArrowTokenNewLine_Before));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PutExpressionBodyOnItsOwnLine)]
    public async Task Test_Operator_BreakAfterArrow()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    public static C operator !(C value) =>[| |]value;
}
", @"
class C
{
    public static C operator !(C value) =>
        value;
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrowTokenNewLine, ConfigOptionValues.ArrowTokenNewLine_After));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PutExpressionBodyOnItsOwnLine)]
    public async Task Test_Operator_BreakBeforeArrow()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    public static C operator !(C value)[| |]=> value;
}
", @"
class C
{
    public static C operator !(C value)
        => value;
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrowTokenNewLine, ConfigOptionValues.ArrowTokenNewLine_Before));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PutExpressionBodyOnItsOwnLine)]
    public async Task Test_ConversionOperator_BreakAfterArrow()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    public static explicit operator C(string value) =>[| |]new C();
}
", @"
class C
{
    public static explicit operator C(string value) =>
        new C();
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrowTokenNewLine, ConfigOptionValues.ArrowTokenNewLine_After));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PutExpressionBodyOnItsOwnLine)]
    public async Task Test_ConversionOperator_BreakBeforeArrow()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    public static explicit operator C(string value)[| |]=> new C();
}
", @"
class C
{
    public static explicit operator C(string value)
        => new C();
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrowTokenNewLine, ConfigOptionValues.ArrowTokenNewLine_Before));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PutExpressionBodyOnItsOwnLine)]
    public async Task Test_PropertyWithoutAccessor_BreakAfterArrow()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string P =>[| |]null;
}
", @"
class C
{
    string P =>
        null;
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrowTokenNewLine, ConfigOptionValues.ArrowTokenNewLine_After));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PutExpressionBodyOnItsOwnLine)]
    public async Task Test_PropertyWithoutAccessor_BreakBeforeArrow()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string P[| |]=> null;
}
", @"
class C
{
    string P
        => null;
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrowTokenNewLine, ConfigOptionValues.ArrowTokenNewLine_Before));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PutExpressionBodyOnItsOwnLine)]
    public async Task TestNoDiagnostic_LocalFunction()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        string LF() => null;
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrowTokenNewLine, ConfigOptionValues.ArrowTokenNewLine_Before));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PutExpressionBodyOnItsOwnLine)]
    public async Task TestNoDiagnostic_PropertyWithGetter()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    string P
    {
        get => null;
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrowTokenNewLine, ConfigOptionValues.ArrowTokenNewLine_Before));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PutExpressionBodyOnItsOwnLine)]
    public async Task TestNoDiagnostic_IndexerWithGetter()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    string this[int index]
    {
        get => null;
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrowTokenNewLine, ConfigOptionValues.ArrowTokenNewLine_Before));
    }
}
