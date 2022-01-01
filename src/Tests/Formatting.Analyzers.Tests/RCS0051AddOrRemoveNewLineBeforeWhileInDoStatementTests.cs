// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Formatting.CodeFixes.CSharp;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class RCS0051AddOrRemoveNewLineBeforeWhileInDoStatementTests : AbstractCSharpDiagnosticVerifier<AddOrRemoveNewLineBeforeWhileInDoStatementAnalyzer, SyntaxTokenCodeFixProvider>
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.AddOrRemoveNewLineBeforeWhileInDoStatement;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveNewLineBeforeWhileInDoStatement)]
        public async Task Test_AddNewLine()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = false;

        do
        {
            M();
        } [||]while (x);
    }
}
", @"
class C
{
    void M()
    {
        bool x = false;

        do
        {
            M();
        }
        while (x);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.NewLineBeforeWhileInDoStatement, true));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveNewLineBeforeWhileInDoStatement)]
        public async Task Test_AddNewLine_WithoutTrivia()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = false;

        do
        {
            M();
        }[||]while (x);
    }
}
", @"
class C
{
    void M()
    {
        bool x = false;

        do
        {
            M();
        }
        while (x);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.NewLineBeforeWhileInDoStatement, true));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveNewLineBeforeWhileInDoStatement)]
        public async Task Test_RemoveNewLine()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = false;

        do
        {
            M();
        }[||]
        while (x);
    }
}
", @"
class C
{
    void M()
    {
        bool x = false;

        do
        {
            M();
        } while (x);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.NewLineBeforeWhileInDoStatement, false));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveNewLineBeforeWhileInDoStatement)]
        public async Task Test_RemoveNewLine_EmptyLine()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool x = false;

        do
        {
            M();
        }[||]

        while (x);
    }
}
", @"
class C
{
    void M()
    {
        bool x = false;

        do
        {
            M();
        } while (x);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.NewLineBeforeWhileInDoStatement, false));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveNewLineBeforeWhileInDoStatement)]
        public async Task TestNoDiagnostic_AddNewLine()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool x = false;

        do
        {
            M();
        }
        while (x);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.NewLineBeforeWhileInDoStatement, true));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveNewLineBeforeWhileInDoStatement)]
        public async Task TestNoDiagnostic_AddNewLine_EmbeddedStatement()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool x = false;

        do
            M();
        while (x);
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveNewLineBeforeWhileInDoStatement)]
        public async Task TestNoDiagnostic_RemoveNewLine()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool x = false;

        do
        {
            M();
        } while (x);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.NewLineBeforeWhileInDoStatement, false));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveNewLineBeforeWhileInDoStatement)]
        public async Task TestNoDiagnostic_RemoveNewLine_EmbeddedStatement()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool x = false;

        do
            M();
        while (x);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.NewLineBeforeWhileInDoStatement, false));
        }
    }
}
