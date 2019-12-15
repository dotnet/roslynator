// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Formatting.CodeFixes.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class RemoveNewLineBetweenClosingBraceAndWhileKeywordTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.RemoveNewLineBetweenClosingBraceAndWhileKeyword;

        public override DiagnosticAnalyzer Analyzer { get; } = new RemoveNewLineBetweenClosingBraceAndWhileKeywordAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new SyntaxTriviaCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveNewLineBetweenClosingBraceAndWhileKeyword)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = false;

        do
        {
            M();
        }[||]
        while (x);
    }
}
", @"
class C
{
    void M()
    {
        bool x = false;

        do
        {
            M();
        } while (x);
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveNewLineBetweenClosingBraceAndWhileKeyword)]
        public async Task Test_EmptyLine()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = false;

        do
        {
            M();
        }[||]

        while (x);
    }
}
", @"
class C
{
    void M()
    {
        bool x = false;

        do
        {
            M();
        } while (x);
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveNewLineBetweenClosingBraceAndWhileKeyword)]
        public async Task TestNoDiagnostic()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool x = false;

        do
        {
            M();
        } while (x);
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveNewLineBetweenClosingBraceAndWhileKeyword)]
        public async Task TestNoDiagnostic_EmbeddedStatement()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool x = false;

        do
            M();
        while (x);
    }
}
");
        }
    }
}
