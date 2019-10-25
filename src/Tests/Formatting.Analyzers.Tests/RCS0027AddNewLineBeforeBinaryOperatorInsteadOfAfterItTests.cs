// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Formatting.CodeFixes.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class AddNewLineBeforeBinaryOperatorInsteadOfAfterItTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.AddNewLineBeforeBinaryOperatorInsteadOfAfterIt;

        public override DiagnosticAnalyzer Analyzer { get; } = new AddNewLineBeforeBinaryOperatorInsteadOfAfterItAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new BinaryExpressionCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBeforeBinaryOperatorInsteadOfAfterIt)]
        public async Task Test()
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
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBeforeBinaryOperatorInsteadOfAfterIt)]
        public async Task TestNoDiagnostic()
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
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBeforeBinaryOperatorInsteadOfAfterIt)]
        public async Task TestNoDiagnostic_SingleLine()
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
");
        }
    }
}
