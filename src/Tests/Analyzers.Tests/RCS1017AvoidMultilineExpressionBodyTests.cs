// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1017AvoidMultilineExpressionBodyTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.AvoidMultilineExpressionBody;

        public override DiagnosticAnalyzer Analyzer { get; } = new AvoidMultilineExpressionBodyAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new ArrowExpressionClauseCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AvoidMultilineExpressionBody)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    string M() =>
        [|@""
""|];
}
", @"
class C
{
    string M()
    {
        return @""
"";
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AvoidMultilineExpressionBody)]
        public async Task TestNoDiagnostic_PreprocessorDirective()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    string M() =>
#if DEBUG
        null;
#else
        null;
#endif
}
");
        }
    }
}
