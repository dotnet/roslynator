// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Formatting.CodeFixes.CSharp;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class RCS0032PlaceNewLineAfterOrBeforeArrowTokenTests : AbstractCSharpDiagnosticVerifier<PlaceNewLineAfterOrBeforeArrowTokenAnalyzer, SyntaxTokenCodeFixProvider>
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.PlaceNewLineAfterOrBeforeArrowToken;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeArrowToken)]
        public async Task Test_BeforeInsteadOfAfter()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    string M() [|=>|]
        null;
}
", @"
class C
{
    string M()
        => null;
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrowTokenNewLine, "before"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeArrowToken)]
        public async Task Test_AfterInsteadOfBefore()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    string M()
        [|=>|] null;
}
", @"
class C
{
    string M() =>
        null;
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrowTokenNewLine, "after"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeArrowToken)]
        public async Task TestNoDiagnostic_BeforeInsteadOfAfter_Comment()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    string M() => // x
        null;
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrowTokenNewLine, "before"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeArrowToken)]
        public async Task TestNoDiagnostic_AfterInsteadOfBefore_Comment()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    string M() // x
        => null;
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrowTokenNewLine, "after"));
        }
    }
}
