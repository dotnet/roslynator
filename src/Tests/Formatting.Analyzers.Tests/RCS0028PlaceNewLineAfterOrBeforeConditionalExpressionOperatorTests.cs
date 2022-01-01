// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Formatting.CodeFixes.CSharp;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class RCS0028PlaceNewLineAfterOrBeforeConditionalExpressionOperatorTests : AbstractCSharpDiagnosticVerifier<PlaceNewLineAfterOrBeforeConditionalOperatorAnalyzer, SyntaxTokenCodeFixProvider>
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.PlaceNewLineAfterOrBeforeConditionalOperator;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeConditionalOperator)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.ConditionalOperatorNewLine, "before"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeConditionalOperator)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.ConditionalOperatorNewLine, "before"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeConditionalOperator)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.ConditionalOperatorNewLine, "before"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeConditionalOperator)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.ConditionalOperatorNewLine, "after"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeConditionalOperator)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.ConditionalOperatorNewLine, "after"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeConditionalOperator)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.ConditionalOperatorNewLine, "after"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeConditionalOperator)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.ConditionalOperatorNewLine, "before"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.PlaceNewLineAfterOrBeforeConditionalOperator)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.ConditionalOperatorNewLine, "after"));
        }
    }
}
