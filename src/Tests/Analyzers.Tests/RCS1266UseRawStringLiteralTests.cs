#if ROSLYN_4_2
// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1266UseRawStringLiteralTests : AbstractCSharpDiagnosticVerifier<UseRawStringLiteralAnalyzer, RawStringLiteralCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.UseRawStringLiteral;

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UseRawStringLiteral)]
    public async Task Test_StringLiteral()
    {
        await VerifyDiagnosticAndFixAsync("""
class C
{
    void M()
    {
        string s = [|@"|]
 ""foo""
";
    }
}
""", """"
class C
{
    void M()
    {
        string s = """

 "foo"

""";
    }
}
"""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UseRawStringLiteral)]
    public async Task Test_InterpolatedString_DollarFirst()
    {
        await VerifyDiagnosticAndFixAsync("""
class C
{
    void M()
    {
        string s = [|$@"|]
"" {""} "" {"foo"}  
";
    }
}
""", """"
class C
{
    void M()
    {
        string s = $"""

" {""} " {"foo"}  

""";
    }
}
"""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UseRawStringLiteral)]
    public async Task Test_InterpolatedString_AtFirst()
    {
        await VerifyDiagnosticAndFixAsync("""
class C
{
    void M()
    {
        string s = [|@$"|]
"" {""} "" {"foo"}  
";
    }
}
""", """"
class C
{
    void M()
    {
        string s = $"""

" {""} " {"foo"}  

""";
    }
}
"""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UseRawStringLiteral)]
    public async Task Test_InterpolatedString_MoreQuotes()
    {
        await VerifyDiagnosticAndFixAsync("""""""""
class C
{
    void M()
    {
        string s = [|@$"|]
"""""""" {""} "" {"foo"}  
";
    }
}
""""""""", """"""
class C
{
    void M()
    {
        string s = $"""""

"""" {""} " {"foo"}  

""""";
    }
}
"""""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UseRawStringLiteral)]
    public async Task Test_InterpolatedString_ContainsBrace()
    {
        await VerifyDiagnosticAndFixAsync("""
class C
{
    void M()
    {
        string s = [|@$"|]
"" {string.Empty} {{ }}
";
    }
}
""", """"
class C
{
    void M()
    {
        string s = $$"""

" {{string.Empty}} { }

""";
    }
}
"""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UseRawStringLiteral)]
    public async Task Test_InterpolatedString_ContainsBraces()
    {
        await VerifyDiagnosticAndFixAsync("""
class C
{
    void M()
    {
        string s = [|@$"|]
"" {string.Empty} {{{{ }}}}
";
    }
}
""", """"
class C
{
    void M()
    {
        string s = $$$"""

" {{{string.Empty}}} {{ }}

""";
    }
}
"""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UseRawStringLiteral)]
    public async Task Test_LiteralExpression2()
    {
        await VerifyDiagnosticAndFixAsync(""""
class C
{
    void M()
    {
        string s = [|@"|]""
""";
    }
}
"""", """"
class C
{
    void M()
    {
        string s = """
"
"
""";
    }
}
"""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UseRawStringLiteral)]
    public async Task Test_StringLiteral_MoreQuotes()
    {
        await VerifyDiagnosticAndFixAsync("""""""
class C
{
    void M()
    {
        string s = [|@"|]
 """"""foo""
";
    }
}
""""""", """"""
class C
{
    void M()
    {
        string s = """"

 """foo"

"""";
    }
}
"""""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UseRawStringLiteral)]
    public async Task TestNoDiagnostic_StringLiteral_NoQuotes()
    {
        await VerifyNoDiagnosticAsync("""
class C
{
    void M()
    {
        string s = @"
 foo
";
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UseRawStringLiteral)]
    public async Task TestNoDiagnostic_InterpolatedString_NoQuotes()
    {
        await VerifyNoDiagnosticAsync("""
class C
{
    void M()
    {
        string s = $@"
 {""} {"foo"}
";
    }
}
""");
    }
}
#endif
