﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1197OptimizeStringBuilderAppendCallTests : AbstractCSharpDiagnosticVerifier<InvocationExpressionAnalyzer, OptimizeStringBuilderAppendCallCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.OptimizeStringBuilderAppendCall;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task Test_Substring_Int32_Int32()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Text;

class C
{
    void M()
    {
        string s = null;
        var sb = new StringBuilder();

        sb.Append([|s.Substring(0, 2)|]);
    }
}
", @"
using System.Text;

class C
{
    void M()
    {
        string s = null;
        var sb = new StringBuilder();

        sb.Append(s, 0, 2);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task Test_Substring_Int32_Int32_Calculation()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Text;

class C
{
    void M()
    {
        string s = null;
        var start = 2;
        var len = 5;
        var sb = new StringBuilder();

        sb.Append([|s.Substring(start + len)|]);
    }
}
", @"
using System.Text;

class C
{
    void M()
    {
        string s = null;
        var start = 2;
        var len = 5;
        var sb = new StringBuilder();

        sb.Append(s, start + len, s.Length - (start + len));
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task Test_Substring_Int32()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Text;

class C
{
    void M()
    {
        string s = null;
        var sb = new StringBuilder();

        sb.Append([|s.Substring(2)|]);
    }
}
", @"
using System.Text;

class C
{
    void M()
    {
        string s = null;
        var sb = new StringBuilder();

        sb.Append(s, 2, s.Length - 2);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task Test_Substring_Int32_Int32_AppendLine()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Text;

class C
{
    void M()
    {
        string s = null;
        var sb = new StringBuilder();

        sb.AppendLine([|s.Substring(0, 2)|]);
    }
}
", @"
using System.Text;

class C
{
    void M()
    {
        string s = null;
        var sb = new StringBuilder();

        sb.Append(s, 0, 2).AppendLine();
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task Test_Substring_Int32_AppendLine()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Text;

class C
{
    void M()
    {
        string s = null;
        var sb = new StringBuilder();

        sb.AppendLine([|s.Substring(2)|]);
    }
}
", @"
using System.Text;

class C
{
    void M()
    {
        string s = null;
        var sb = new StringBuilder();

        sb.Append(s, 2, s.Length - 2).AppendLine();
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task Test_Remove()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Text;

class C
{
    void M()
    {
        string s = null;
        var sb = new StringBuilder();

        sb.Append([|s.Remove(2)|]);
    }
}
", @"
using System.Text;

class C
{
    void M()
    {
        string s = null;
        var sb = new StringBuilder();

        sb.Append(s, 0, 2);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task Test_Remove_AppendLine()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Text;

class C
{
    void M()
    {
        string s = null;
        var sb = new StringBuilder();

        sb.AppendLine([|s.Remove(2)|]);
    }
}
", @"
using System.Text;

class C
{
    void M()
    {
        string s = null;
        var sb = new StringBuilder();

        sb.Append(s, 0, 2).AppendLine();
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task Test_StringFormat()
    {
        await VerifyDiagnosticAndFixAsync("""
using System.Text;

class C
{
    void M()
    {
        string s = null;
        var sb = new StringBuilder();

        sb.Append([|string.Format("f", s)|]);
    }
}
""", """
using System.Text;

class C
{
    void M()
    {
        string s = null;
        var sb = new StringBuilder();

        sb.AppendFormat("f", s);
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task Test_StringFormat_AppendLine()
    {
        await VerifyDiagnosticAndFixAsync("""
using System.Text;

class C
{
    void M()
    {
        string s = null;
        var sb = new StringBuilder();

        sb.AppendLine([|string.Format("f", s)|]);
    }
}
""", """
using System.Text;

class C
{
    void M()
    {
        string s = null;
        var sb = new StringBuilder();

        sb.AppendFormat("f", s).AppendLine();
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task TestNoDiagnostic_InterpolatedString()
    {
        await VerifyNoDiagnosticAsync("""
using System.Text;

class C
{
    void M()
    {
        string s = null;
        var sb = new StringBuilder();

        sb.Append($"{s}s");
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task TestNoDiagnostic_InterpolatedString_Braces()
    {
        await VerifyNoDiagnosticAsync("""
using System.Text;

class C
{
    void M()
    {
        string s = null;
        var sb = new StringBuilder();

        sb.Append(
            $"a{{b}}c{s}a{{b}}c");
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task TestNoDiagnostic_InterpolatedRawString_ContainingQuotes()
    {
        await VerifyNoDiagnosticAsync("""""
using System.Text;
class C
{
    void M()
    {
        string s = null;
        var sb = new StringBuilder();
        sb.Append($""""<a href="somelink">{s}</a>"""");
    }
}
""""", options: WellKnownCSharpTestOptions.Default_CSharp11);
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task TestNoDiagnostic_InterpolatedMultilineRawString_ContainingQuotes()
    {
        await VerifyNoDiagnosticAsync(""""
using System.Text;
class C
{
    void M()
    {
        string s = null;
        var sb = new StringBuilder();
        sb.Append($"""
                    <a href="somelink">{s}</a>
"""
        );
    }
}
"""", options: WellKnownCSharpTestOptions.Default_CSharp11);
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task TestNoDiagnostic_InterpolatedString_Char()
    {
        await VerifyNoDiagnosticAsync("""
using System.Text;

class C
{
    void M()
    {
        string s = null;
        var sb = new StringBuilder();

        sb.Append($"\"{s}'");
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task TestNoDiagnostic_InterpolatedString_AppendLine()
    {
        await VerifyNoDiagnosticAsync("""
using System.Text;

class C
{
    void M()
    {
        string s = null;
        var sb = new StringBuilder();

        sb.AppendLine($"{s}s");
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task TestNoDiagnostic_InterpolatedString_AppendLine2()
    {
        await VerifyNoDiagnosticAsync("""
using System.Text;

class C
{
    void M()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"ab{'s'}");
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task TestNoDiagnostic_InterpolatedString_AppendLine3()
    {
        await VerifyNoDiagnosticAsync("""
using System.Text;

class C
{
    void M()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"ab{'s'}s");
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task TestNoDiagnostic_InterpolatedString_WithFormat_AppendLine()
    {
        await VerifyNoDiagnosticAsync("""
using System.Text;

class C
{
    void M()
    {
        string s = null;
        var sb = new StringBuilder();

        sb.AppendLine($"{s,1:f}");
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task TestNoDiagnostic_InterpolatedString_WithFormat_AppendLine2()
    {
        await VerifyNoDiagnosticAsync("""
using System;
using System.Text;

class C
{
    void M(DateTime x)
    {
        string s = null;
        var sb = new StringBuilder();
        sb.Append($@"{x:hh\:mm\:ss\.fff}").ToString();
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task TestNoDiagnostic_Concatenation()
    {
        await VerifyNoDiagnosticAsync("""
using System.Text;

class C
{
    void M()
    {
        string s = null;
        var sb = new StringBuilder();

        sb.Append("ab" + s + "cd").Append("ef" + s + "gh");
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task TestNoDiagnostic_Concatenation_Char()
    {
        await VerifyNoDiagnosticAsync("""
using System.Text;

class C
{
    void M()
    {
        char a = 'a';

        var sb = new StringBuilder();

        sb.Append(a + "b" + "c");
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task TestNoDiagnostic_Concatenation_AppendLine()
    {
        await VerifyNoDiagnosticAsync("""
using System.Text;

class C
{
    void M()
    {
        string s = null;
        var sb = new StringBuilder();

        sb.Append(s + "ab");
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task TestNoDiagnostic_Concatenation_AppendLine2()
    {
        await VerifyNoDiagnosticAsync("""
using System.Text;

class C
{
    void M()
    {
        string s = null;
        var sb = new StringBuilder();

        sb.AppendLine("ab" + s);
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task TestNoDiagnostic_Concatenation_AppendLine3()
    {
        await VerifyNoDiagnosticAsync("""
using System.Text;

class C
{
    void M()
    {
        int i = 0;
        var sb = new StringBuilder();

        sb.AppendLine("ab" + i);
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task TestNoDiagnostic_Concatenation_AppendLine4()
    {
        await VerifyNoDiagnosticAsync("""
using System.Text;

class C
{
    void M()
    {
        object o = null;
        var sb = new StringBuilder();

        sb.AppendLine("ab" + o);
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task TestNoDiagnostic_Concatenation_AppendLine5()
    {
        await VerifyNoDiagnosticAsync("""
using System.Text;

class C
{
    void M()
    {
        string s = null;
        var sb = new StringBuilder();

        sb.AppendLine("ab" + s + "b").AppendLine("ef" + s + "d");
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task Test_AppendLine()
    {
        await VerifyDiagnosticAndFixAsync("""
using System.Text;

class C
{
    void M()
    {
        var sb = new StringBuilder();

        sb.Append("").[|AppendLine|]();
    }
}
""", """
using System.Text;

class C
{
    void M()
    {
        var sb = new StringBuilder();

        sb.AppendLine("");
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task Test_AppendLine2()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Text;

class C
{
    void M()
    {
        string s = null;
        var sb = new StringBuilder();

        sb.Append(s).[|AppendLine|]();
    }
}
", @"
using System.Text;

class C
{
    void M()
    {
        string s = null;
        var sb = new StringBuilder();

        sb.AppendLine(s);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task Test_Join()
    {
        await VerifyDiagnosticAndFixAsync("""
using System.Text;

class C
{
    void M()
    {
        var sb = new StringBuilder();

        sb.Append([|string.Join('x', "", "")|]);
        sb.Append([|string.Join('x', default(object), default(object))|]);
        sb.Append([|string.Join("x", "", "")|]);
        sb.Append([|string.Join("x", default(object), default(object))|]);
        sb.Append([|string.Join('x', new[] { "" })|]);
        sb.Append([|string.Join("x", new[] { default(object) })|]);
    }
}
""", """
using System.Text;

class C
{
    void M()
    {
        var sb = new StringBuilder();

        sb.AppendJoin('x', "", "");
        sb.AppendJoin('x', default(object), default(object));
        sb.AppendJoin("x", "", "");
        sb.AppendJoin("x", default(object), default(object));
        sb.AppendJoin('x', new[] { "" });
        sb.AppendJoin("x", new[] { default(object) });
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task TestNoDiagnostic_Const()
    {
        await VerifyNoDiagnosticAsync("""
using System.Text;

class C
{
    void M()
    {
        const string s = null;

        var sb = new StringBuilder();

        sb.Append("x" + s);
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task TestNoDiagnostic()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Text;

class C
{
    void M()
    {
        string s = null;
        int i = 0;
        object o = null;

        var sb = new StringBuilder();

        sb.Append(s.Remove(2, 3));

        sb.AppendLine(s.Remove(2, 3));

        sb.Insert(0, i);

        sb.Insert(0, o);

        sb.Append(i).AppendLine();

        sb.Append(o).AppendLine();
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task TestNoDiagnostic_ConstantInterpolatedString()
    {
        await VerifyNoDiagnosticAsync("""
using System.Text;

class C
{
    void M()
    {
        const string Foo = "Foo";
        var sb = new StringBuilder().Append($"{Foo}Bar");
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OptimizeStringBuilderAppendCall)]
    public async Task TestNoDiagnostic_NoAppendMethodChain()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Text;

class C
{
    string M()
    {
        var sb = new StringBuilder();
        _ = sb.AppendLine();

        return sb.ToString();
    }
}
");
    }
}
