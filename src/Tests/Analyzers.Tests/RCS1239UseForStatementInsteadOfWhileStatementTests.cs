// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1239UseForStatementInsteadOfWhileStatementTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UseForStatementInsteadOfWhileStatement;

        public override DiagnosticAnalyzer Analyzer { get; } = new UseForStatementInsteadOfWhileStatementAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new WhileStatementCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseForStatementInsteadOfWhileStatement)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool f = false;

        int i = 0;
        [|while|] (f)
        {
            M();
            i++;
        }
    }
}
", @"
class C
{
    void M()
    {
        bool f = false;

        for (int i = 0; f; i++)
        {
            M();
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseForStatementInsteadOfWhileStatement)]
        public async Task TestNoDiagnostic_MultipleIncrementedVariables()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool f = false;

        int i = 0;
        int j = 0;
        while (f)
        {
            M();
            i++;
            j++;
        }
    }
}
");
        }
    }
}
