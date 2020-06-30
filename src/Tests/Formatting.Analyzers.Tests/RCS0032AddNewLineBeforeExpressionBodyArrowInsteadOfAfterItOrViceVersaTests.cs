// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Formatting.CodeFixes.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class RCS0032AddNewLineBeforeExpressionBodyArrowInsteadOfAfterItOrViceVersaTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.AddNewLineBeforeExpressionBodyArrowInsteadOfAfterItOrViceVersa;

        public override DiagnosticAnalyzer Analyzer { get; } = new AddNewLineBeforeExpressionBodyArrowInsteadOfAfterItOrViceVersaAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new SyntaxTokenCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBeforeExpressionBodyArrowInsteadOfAfterItOrViceVersa)]
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
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBeforeExpressionBodyArrowInsteadOfAfterItOrViceVersa)]
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
", options: Options.WithEnabled(AnalyzerOptions.AddNewLineAfterExpressionBodyArrowInsteadOfBeforeIt));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBeforeExpressionBodyArrowInsteadOfAfterItOrViceVersa)]
        public async Task TestNoDiagnostic_BeforeInsteadOfAfter_Comment()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    string M() => // x
        null;
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBeforeExpressionBodyArrowInsteadOfAfterItOrViceVersa)]
        public async Task TestNoDiagnostic_AfterInsteadOfBefore_Comment()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    string M() // x
        => null;
}
", options: Options.WithEnabled(AnalyzerOptions.AddNewLineAfterExpressionBodyArrowInsteadOfBeforeIt));
        }
    }
}
