// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Formatting.CodeFixes.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class RCS0051AddNewLineBetweenClosingBraceAndWhileKeywordTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.AddNewLineBetweenClosingBraceAndWhileKeyword;

        public override DiagnosticAnalyzer Analyzer { get; } = new AddOrRemoveNewLineBetweenClosingBraceAndWhileKeywordAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new SyntaxTokenCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBetweenClosingBraceAndWhileKeyword)]
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
        } [||]while (x);
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
        }
        while (x);
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBetweenClosingBraceAndWhileKeyword)]
        public async Task Test_WithoutTrivia()
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
        }[||]while (x);
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
        }
        while (x);
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBetweenClosingBraceAndWhileKeyword)]
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
        }
        while (x);
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBetweenClosingBraceAndWhileKeyword)]
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
