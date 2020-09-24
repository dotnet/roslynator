// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1078UseEmptyStringLiteralInsteadOfStringEmptyOrViceVersaTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UseEmptyStringLiteralInsteadOfStringEmptyOrViceVersa;

        public override DiagnosticAnalyzer Analyzer { get; } = new UseEmptyStringLiteralInsteadOfStringEmptyOrViceVersaAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new UseEmptyStringLiteralInsteadOfStringEmptyOrViceVersaCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseEmptyStringLiteralInsteadOfStringEmptyOrViceVersa)]
        public async Task Test_StringEmpty()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    void M()
    {
        string s = null;
        s = [|string.Empty|];
        s = [|String.Empty|];
        s = [|System.String.Empty|];
        s = [|global::System.String.Empty|];
    }
}
", @"
using System;

class C
{
    void M()
    {
        string s = null;
        s = """";
        s = """";
        s = """";
        s = """";
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseEmptyStringLiteralInsteadOfStringEmptyOrViceVersa)]
        public async Task Test_EmptyString()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string s = null;
        s = [|""""|];
        s = [|@""""|];
        s = [|$""""|];
        s = [|@$""""|];
        s = [|$@""""|];
    }
}
", @"
class C
{
    void M()
    {
        string s = null;
        s = string.Empty;
        s = string.Empty;
        s = string.Empty;
        s = string.Empty;
        s = string.Empty;
    }
}
", options: Options.WithEnabled(AnalyzerOptions.UseStringEmptyInsteadOfEmptyStringLiteral));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseEmptyStringLiteralInsteadOfStringEmptyOrViceVersa)]
        public async Task TestNoDiagnostic()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        string s = null;
        s = "" "";
        s = ""a"";
    }
}
", options: Options.WithEnabled(AnalyzerOptions.UseStringEmptyInsteadOfEmptyStringLiteral));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseEmptyStringLiteralInsteadOfStringEmptyOrViceVersa)]
        public async Task TestNoDiagnostic_ExpressionMustBeConstant()
        {
            await VerifyNoDiagnosticAsync(@"
using System;

class C
{
     private const string _f = """";

    [Obsolete("""")]
    void M(string p = """")
    {
        const string s = """";

        switch (s)
        {
            case """":
                break;
        }
    }
}
", options: Options.WithEnabled(AnalyzerOptions.UseStringEmptyInsteadOfEmptyStringLiteral));
        }
    }
}
