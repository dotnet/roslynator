// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1197OptimizeStringBuilderAppendCallTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.OptimizeStringBuilderAppendCall;

        public override DiagnosticAnalyzer Analyzer { get; } = new InvocationExpressionAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new OptimizeStringBuilderAppendCallCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
        public async Task Test_InterpolatedString_Argument()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System.Text;

class C
{
    void M()
    {
        string s = null;

        var sb = new StringBuilder();

        sb.Append(
            [|$""a '{s}' b""|]);
    }
}
", @"
using System.Text;

class C
{
    void M()
    {
        string s = null;

        var sb = new StringBuilder();

        sb.Append(
            ""a '"").Append(s).Append(""' b"");
    }
}
");
        }
    }
}
