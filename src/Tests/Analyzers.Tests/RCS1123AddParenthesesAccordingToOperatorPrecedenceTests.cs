// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1123AddParenthesesAccordingToOperatorPrecedenceTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.AddParenthesesAccordingToOperatorPrecedence;

        public override DiagnosticAnalyzer Analyzer { get; } = new AddParenthesesAccordingToOperatorPrecedenceAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new AddParenthesesCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddParenthesesAccordingToOperatorPrecedence)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool a = false, b = false, c = false, d = false;

        if ([|a
#if DEBUG
            && b
#endif
            && c|] || d)
        {
        }
    }
}
", @"
class C
{
    void M()
    {
        bool a = false, b = false, c = false, d = false;

        if ((a
#if DEBUG
            && b
#endif
            && c) || d)
        {
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddParenthesesAccordingToOperatorPrecedence)]
        public async Task TestNoDiagnostic_PreprocessorDirectives()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool a = false, b = false, c = false, d = false;

        if (a
#if X
            && c || d)
#else //X
#if X2
#else //X2
#endif //X2
            && b
            && c || d)
#endif //X
        {
        }
    }
}
");
        }
    }
}
