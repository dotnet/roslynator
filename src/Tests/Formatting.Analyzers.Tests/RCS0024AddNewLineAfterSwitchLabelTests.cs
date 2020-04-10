// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Formatting.CodeFixes.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class RCS0024AddNewLineAfterSwitchLabelTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.AddNewLineAfterSwitchLabel;

        public override DiagnosticAnalyzer Analyzer { get; } = new AddNewLineAfterSwitchLabelAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new StatementCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineAfterSwitchLabel)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void Foo()
    {
        string s = null;

        switch (s)
        {
            case """": [||]break;
        }
    }
}
", @"
class C
{
    void Foo()
    {
        string s = null;

        switch (s)
        {
            case """":
                break;
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineAfterSwitchLabel)]
        public async Task Test_MultipleLabels()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void Foo()
    {
        string s = null;

        switch (s)
        {
            case ""a"":
            case ""b"": [||]break;
        }
    }
}
", @"
class C
{
    void Foo()
    {
        string s = null;

        switch (s)
        {
            case ""a"":
            case ""b"":
                break;
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineAfterSwitchLabel)]
        public async Task TestNoDiagnostic()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void Foo()
    {
        string s = null;

        switch (s)
        {
            case """":
                break;
        }
    }
}
");
        }
    }
}
