// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Formatting.CodeFixes.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class AddNewLineBeforeExpressionBodyArrowInsteadOfAfterItTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.AddNewLineBeforeExpressionBodyArrowInsteadOfAfterIt;

        public override DiagnosticAnalyzer Analyzer { get; } = new AddNewLineBeforeExpressionBodyArrowInsteadOfAfterItAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new SyntaxTokenCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBeforeExpressionBodyArrowInsteadOfAfterIt)]
        public async Task Test()
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

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBeforeExpressionBodyArrowInsteadOfAfterIt)]
        public async Task TestNoDiagnostic_Comment()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    string M() => // x
        null;
}
");
        }
    }
}
