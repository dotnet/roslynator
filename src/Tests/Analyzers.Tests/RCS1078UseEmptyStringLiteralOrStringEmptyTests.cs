// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1078UseEmptyStringLiteralOrStringEmptyTests : AbstractCSharpDiagnosticVerifier<UseEmptyStringLiteralOrStringEmptyAnalyzer, UseEmptyStringLiteralOrStringEmptyCodeFixProvider>
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.UseEmptyStringLiteralOrStringEmpty;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseEmptyStringLiteralOrStringEmpty)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.EmptyStringStyle, ConfigOptionValues.EmptyStringStyle_Literal));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseEmptyStringLiteralOrStringEmpty)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.EmptyStringStyle, ConfigOptionValues.EmptyStringStyle_Field));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseEmptyStringLiteralOrStringEmpty)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.EmptyStringStyle, ConfigOptionValues.EmptyStringStyle_Field));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseEmptyStringLiteralOrStringEmpty)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.EmptyStringStyle, ConfigOptionValues.EmptyStringStyle_Field));
        }
    }
}
