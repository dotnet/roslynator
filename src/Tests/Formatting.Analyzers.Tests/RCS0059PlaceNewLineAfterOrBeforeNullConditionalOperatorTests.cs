// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Formatting.CodeFixes.CSharp;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class RCS0059PlaceNewLineAfterOrBeforeNullConditionalOperatorTests : AbstractCSharpDiagnosticVerifier<PlaceNewLineAfterOrBeforeNullConditionalOperatorAnalyzer, ConditionalAccessExpressionCodeFixProvider>
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.PlaceNewLineAfterOrBeforeNullConditionalOperator;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeNullConditionalOperator)]
        public async Task Test_BeforeInsteadOfAfter()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System.Linq;

class C
{
    void M()
    {
        string s = """"
            .Select(f => f.ToString())
            .FirstOrDefault()[|?|]
            .ToString();
    }
}
", @"
using System.Linq;

class C
{
    void M()
    {
        string s = """"
            .Select(f => f.ToString())
            .FirstOrDefault()
            ?.ToString();
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.NullConditionalOperatorNewLine, "before"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeNullConditionalOperator)]
        public async Task Test_AfterInsteadOfBefore()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System.Linq;

class C
{
    void M()
    {
        string s = """"
            .Select(f => f.ToString())
            .FirstOrDefault()
            [|?|].ToString();
    }
}
", @"
using System.Linq;

class C
{
    void M()
    {
        string s = """"
            .Select(f => f.ToString())
            .FirstOrDefault()?
            .ToString();
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.NullConditionalOperatorNewLine, "after"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeNullConditionalOperator)]
        public async Task Test_AfterInsteadOfBefore2()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System.Linq;

class C
{
    void M()
    {
        var x = ToString()
            .ToString()
            [|?|].Length;
    }
}
", @"
using System.Linq;

class C
{
    void M()
    {
        var x = ToString()
            .ToString()?
            .Length;
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.NullConditionalOperatorNewLine, "after"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeNullConditionalOperator)]
        public async Task TestNoDiagnostic_BeforeInsteadOfAfter()
        {
            await VerifyNoDiagnosticAsync(@"
using System.Linq;

class C
{
    void M()
    {
        string s = """"
            .Select(f => f.ToString())
            .FirstOrDefault()
            ?.ToString();
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.NullConditionalOperatorNewLine, "before"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeNullConditionalOperator)]
        public async Task TestNoDiagnostic_BeforeInsteadOfAfter_SingleLine()
        {
            await VerifyNoDiagnosticAsync(@"
using System.Linq;

class C
{
    void M()
    {
        string s = """"
            .Select(f => f.ToString())
            .FirstOrDefault()?.ToString();
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.NullConditionalOperatorNewLine, "before"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeNullConditionalOperator)]
        public async Task TestNoDiagnostic_AfterInsteadOfBefore()
        {
            await VerifyNoDiagnosticAsync(@"
using System.Linq;

class C
{
    void M()
    {
        string s = """"
            .Select(f => f.ToString())
            .FirstOrDefault()?
            .ToString();
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.NullConditionalOperatorNewLine, "after"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeNullConditionalOperator)]
        public async Task TestNoDiagnostic_AfterInsteadOfBefore_SingleLine()
        {
            await VerifyNoDiagnosticAsync(@"
using System.Linq;

class C
{
    void M()
    {
        string s = """"
            .Select(f => f.ToString())
            .FirstOrDefault()?.ToString();
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.NullConditionalOperatorNewLine, "after"));
        }
    }
}
