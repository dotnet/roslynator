// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Formatting.CodeFixes.CSharp;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class RCS0027PlaceNewLineAfterOrBeforeBinaryOperatorTests : AbstractCSharpDiagnosticVerifier<PlaceNewLineAfterOrBeforeBinaryOperatorAnalyzer, BinaryExpressionCodeFixProvider>
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.PlaceNewLineAfterOrBeforeBinaryOperator;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeBinaryOperator)]
        public async Task Test_BeforeInsteadOfAfter()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = false;
        bool y = false;
        bool z = false;

        if (x [||]&&
            y [||]&&
            z)
        {
        }
    }
}
", @"
class C
{
    void M()
    {
        bool x = false;
        bool y = false;
        bool z = false;

        if (x
            && y
            && z)
        {
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.BinaryOperatorNewLine, "before"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeBinaryOperator)]
        public async Task Test_AfterInsteadOfBefore()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = false;
        bool y = false;
        bool z = false;

        if (x
            [||]&& y
            [||]&& z)
        {
        }
    }
}
", @"
class C
{
    void M()
    {
        bool x = false;
        bool y = false;
        bool z = false;

        if (x &&
            y &&
            z)
        {
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.BinaryOperatorNewLine, "after"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeBinaryOperator)]
        public async Task TestNoDiagnostic_BeforeInsteadOfAfter()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool x = false;
        bool y = false;
        bool z = false;

        if (x
            && y
            && z)
        {
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.BinaryOperatorNewLine, "before"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeBinaryOperator)]
        public async Task TestNoDiagnostic_BeforeInsteadOfAfter_SingleLine()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool x = false;
        bool y = false;
        bool z = false;

        if (x && y && z)
        {
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.BinaryOperatorNewLine, "before"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeBinaryOperator)]
        public async Task TestNoDiagnostic_AfterInsteadOfBefore()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool x = false;
        bool y = false;
        bool z = false;

        if (x &&
            y &&
            z)
        {
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.BinaryOperatorNewLine, "after"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeBinaryOperator)]
        public async Task TestNoDiagnostic_AfterInsteadOfBefore_SingleLine()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool x = false;
        bool y = false;
        bool z = false;

        if (x && y && z)
        {
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.BinaryOperatorNewLine, "after"));
        }
    }
}
