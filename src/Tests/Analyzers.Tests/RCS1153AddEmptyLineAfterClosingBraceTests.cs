// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1153AddEmptyLineAfterClosingBraceTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.AddEmptyLineAfterClosingBrace;

        public override DiagnosticAnalyzer Analyzer { get; } = new AddEmptyLineAfterClosingBraceAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new AddEmptyLineAfterClosingBraceCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineAfterClosingBrace)]
        public async Task Test_ElseClause()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = false;

        if (x)
        {
        }
        else
        {
        }[|
|]        M();
    }
}
", @"
class C
{
    void M()
    {
        bool x = false;

        if (x)
        {
        }
        else
        {
        }

        M();
    }
}
");
        }
    }
}
