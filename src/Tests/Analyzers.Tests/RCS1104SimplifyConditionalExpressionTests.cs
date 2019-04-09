// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1104SimplifyConditionalExpressionTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.SimplifyConditionalExpression;

        public override DiagnosticAnalyzer Analyzer { get; } = new SimplifyConditionalExpressionAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new ConditionalExpressionCodeFixProvider();

        [Theory, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyConditionalExpression)]
        [InlineData("f ? true : false", "f")]
        [InlineData("!f ? false : true", "f")]
        [InlineData("((f)) ? ((true)) : ((false))", "f")]
        [InlineData("f ? false : true", "!f")]
        [InlineData("f == g ? false : true", "f != g")]
        [InlineData("f != g ? false : true", "f == g")]

        [InlineData(@"f
            ? true
            : false", "f")]

        [InlineData(@"[|f //a
              /*b*/ ? /*c*/ true //d
                                 /*e*/ : /*f*/ false|] /*g*/", @"f //a
              /*b*/  /*c*/  //d
                                 /*e*/  /*f*/  /*g*/")]
        public async Task Test_TrueFalse(string fromData, string toData)
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(bool f, bool g)
    {
        if ([||]) { }
}
}
", fromData, toData);
        }

        [Theory, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyConditionalExpression)]
        [InlineData("f ? g : false", "f && g")]
        [InlineData("f ? g || g : false", "f && (g || g)")]
        [InlineData(@"[|f
            ? g
            : false|] /**/", @"f
            && g /**/")]
        public async Task Test_LogicalAnd(string fromData, string toData)
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(bool f, bool g)
    {
        if ([||]) { }
    }
}
", fromData, toData);
        }

        [Theory, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyConditionalExpression)]
        [InlineData("f ? true : g", "f || g")]
        [InlineData(@"[|f
            ? true
            : g|] /**/", @"f
            || g /**/")]
        public async Task Test_LogicalOr(string fromData, string toData)
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(bool f, bool g)
    {
        if ([||]) { }
    }
}
", fromData, toData);
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyConditionalExpression)]
        public async Task TestNoDiagnostic()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M(bool f, bool g, bool h)
    {
        if ((f) ? g : h) { }
        if ((f) ? false : g) { }
        if ((f) ? g : true) { }

        if ((f)
#if DEBUG
                ? false
                : true) { }
#else
                ? true
                : false;
#endif
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyConditionalExpression)]
        public async Task TestNoDiagnostic_NullableBool()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool x = false;

        bool? y = (x) ? default(bool?) : false;
    }
}
");
        }
    }
}
