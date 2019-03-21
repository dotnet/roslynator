// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1184FormatConditionalExpressionTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.FormatConditionalExpression;

        public override DiagnosticAnalyzer Analyzer { get; } = new FormatConditionalExpressionAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new ConditionalExpressionCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FormatConditionalExpression)]
        public async Task TestNoDiagnostic_PreprocessorDirective()
        {
            await VerifyNoDiagnosticAsync(@"
#define X
class C
{
    bool M(bool a, bool b, bool c)
    {
        return
#if X
        a ? b :
#endif
        c;
    }
}
");
        }
    }
}
