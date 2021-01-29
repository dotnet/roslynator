// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Formatting.CodeFixes.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class RCS0028AddNewLineBeforeConditionalOperatorInsteadOfAfterItOrViceVersaTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.AddNewLineBeforeConditionalOperatorInsteadOfAfterItOrViceVersa;

        protected override DiagnosticAnalyzer Analyzer { get; } = new AddNewLineBeforeConditionalOperatorInsteadOfAfterItOrViceVersaAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new SyntaxTokenCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBeforeConditionalOperatorInsteadOfAfterItOrViceVersa)]
        public async Task Test_BeforeInsteadOfAfter()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = false, y = false, z = false;

        x = (x) [||]?
            y :
            z;
    }
}
", @"
class C
{
    void M()
    {
        bool x = false, y = false, z = false;

        x = (x)
            ? y
            : z;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBeforeConditionalOperatorInsteadOfAfterItOrViceVersa)]
        public async Task Test_BeforeInsteadOfAfter_QuestionToken()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = false, y = false, z = false;

        x = (x) [||]?
            y
            : z;
    }
}
", @"
class C
{
    void M()
    {
        bool x = false, y = false, z = false;

        x = (x)
            ? y
            : z;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBeforeConditionalOperatorInsteadOfAfterItOrViceVersa)]
        public async Task Test_BeforeInsteadOfAfter_ColonToken()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = false, y = false, z = false;

        x = (x)
            ? y [||]:
            z;
    }
}
", @"
class C
{
    void M()
    {
        bool x = false, y = false, z = false;

        x = (x)
            ? y
            : z;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBeforeConditionalOperatorInsteadOfAfterItOrViceVersa)]
        public async Task Test_AfterInsteadOfBefore()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = false, y = false, z = false;

        x = (x)
            [||]? y
            : z;
    }
}
", @"
class C
{
    void M()
    {
        bool x = false, y = false, z = false;

        x = (x) ?
            y :
            z;
    }
}
", options: Options.WithEnabled(AnalyzerOptions.AddNewLineAfterConditionalOperatorInsteadOfBeforeIt));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBeforeConditionalOperatorInsteadOfAfterItOrViceVersa)]
        public async Task Test_AfterInsteadOfBefore_QuestionToken()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = false, y = false, z = false;

        x = (x)
            [||]? y :
            z;
    }
}
", @"
class C
{
    void M()
    {
        bool x = false, y = false, z = false;

        x = (x) ?
            y :
            z;
    }
}
", options: Options.WithEnabled(AnalyzerOptions.AddNewLineAfterConditionalOperatorInsteadOfBeforeIt));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBeforeConditionalOperatorInsteadOfAfterItOrViceVersa)]
        public async Task Test_AfterInsteadOfBefore_ColonToken()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = false, y = false, z = false;

        x = (x) ?
            y
            [||]: z;
    }
}
", @"
class C
{
    void M()
    {
        bool x = false, y = false, z = false;

        x = (x) ?
            y :
            z;
    }
}
", options: Options.WithEnabled(AnalyzerOptions.AddNewLineAfterConditionalOperatorInsteadOfBeforeIt));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBeforeConditionalOperatorInsteadOfAfterItOrViceVersa)]
        public async Task TestNoDiagnostic_BeforeInsteadOfAfter()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool x = false, y = false, z = false;

        x = (x)
            ? y
            : z;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBeforeConditionalOperatorInsteadOfAfterItOrViceVersa)]
        public async Task TestNoDiagnostic_AfterInsteadOfBefore()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool x = false, y = false, z = false;

        x = (x) ?
            y :
            z;
    }
}
", options: Options.WithEnabled(AnalyzerOptions.AddNewLineAfterConditionalOperatorInsteadOfBeforeIt));
        }
    }
}
