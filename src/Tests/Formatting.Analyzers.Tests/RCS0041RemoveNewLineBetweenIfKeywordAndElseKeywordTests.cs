// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Formatting.CodeFixes.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class RCS0041RemoveNewLineBetweenIfKeywordAndElseKeywordTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.RemoveNewLineBetweenIfKeywordAndElseKeyword;

        protected override DiagnosticAnalyzer Analyzer { get; } = new RemoveNewLineBetweenIfKeywordAndElseKeywordAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new SyntaxTriviaCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveNewLineBetweenIfKeywordAndElseKeyword)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = false, y = false;

        if (x)
        {
        }
        else[||]
        if (y)
        {
        }
    }
}
", @"
class C
{
    void M()
    {
        bool x = false, y = false;

        if (x)
        {
        }
        else if (y)
        {
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveNewLineBetweenIfKeywordAndElseKeyword)]
        public async Task Test_EmptyLine()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = false, y = false;

        if (x)
        {
        }
        else[||]

        if (y)
        {
        }
    }
}
", @"
class C
{
    void M()
    {
        bool x = false, y = false;

        if (x)
        {
        }
        else if (y)
        {
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveNewLineBetweenIfKeywordAndElseKeyword)]
        public async Task TestNoDiagnostic()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool x = false, y = false;

        if (x)
        {
        }
        else if (y)
        {
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveNewLineBetweenIfKeywordAndElseKeyword)]
        public async Task TestNoDiagnostic_Comment()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool x = false, y = false;

        if (x)
        {
        }
        else // x
        if (y)
        {
        }
    }
}
");
        }
    }
}
