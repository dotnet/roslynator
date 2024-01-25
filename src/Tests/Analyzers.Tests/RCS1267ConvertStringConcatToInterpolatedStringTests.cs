// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1267ConvertStringConcatToInterpolatedStringTests : AbstractCSharpDiagnosticVerifier<InvocationExpressionAnalyzer, InvocationExpressionCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.ConvertStringConcatToInterpolatedString;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConvertStringConcatToInterpolatedString)]
    public async Task Test()
    {
        await VerifyDiagnosticAndFixAsync("""
using System;

class C
{
    void M()
    {
        string s = [|string.Concat("Now: ", DateTime.Now, ", Now UTC: ", DateTime.UtcNow)|];
    }
}
""", """
using System;

class C
{
    void M()
    {
        string s = $"Now: {DateTime.Now}, Now UTC: {DateTime.UtcNow}";
    }
}
"""
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConvertStringConcatToInterpolatedString)]
    public async Task Test_Verbatim()
    {
        await VerifyDiagnosticAndFixAsync("""
using System;

class C
{
    void M()
    {
        string s = [|string.Concat(@"Now: ", DateTime.Now, @", Now UTC: ", DateTime.UtcNow)|];
    }
}
""", """
using System;

class C
{
    void M()
    {
        string s = @$"Now: {DateTime.Now}, Now UTC: {DateTime.UtcNow}";
    }
}
"""
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConvertStringConcatToInterpolatedString)]
    public async Task TestNoDiagnostic_SingleArgument()
    {
        await VerifyNoDiagnosticAsync("""
using System;

class C
{
    void M()
    {
        string s = string.Concat("");
    }
}
"""
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConvertStringConcatToInterpolatedString)]
    public async Task TestNoDiagnostic_NoStringLiteral()
    {
        await VerifyNoDiagnosticAsync("""
using System;

class C
{
    void M()
    {
        string s = string.Concat(DateTime.Now, DateTime.Now);
    }
}
"""
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConvertStringConcatToInterpolatedString)]
    public async Task TestNoDiagnostic_ContainsInterpolatedString()
    {
        await VerifyNoDiagnosticAsync("""
using System;

class C
{
    void M()
    {
        string s = string.Concat(DateTime.Now, "", $"");
    }
}
"""
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConvertStringConcatToInterpolatedString)]
    public async Task TestNoDiagnostic_ContainsVerbatimAndNonVerbatimLiterals()
    {
        await VerifyNoDiagnosticAsync("""
using System;

class C
{
    void M()
    {
        string s = string.Concat(DateTime.Now, "", @"");
    }
}
"""
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConvertStringConcatToInterpolatedString)]
    public async Task TestNoDiagnostic_ContainsVerbatimAndNonVerbatimLiterals2()
    {
        await VerifyNoDiagnosticAsync("""
using System;

class C
{
    void M()
    {
        string s = string.Concat(DateTime.Now, @"", "");
    }
}
"""
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConvertStringConcatToInterpolatedString)]
    public async Task TestNoDiagnostic_SpansOverMultipleLines()
    {
        await VerifyNoDiagnosticAsync("""
using System;

class C
{
    void M()
    {
        string s = string.Concat(
            "Now: ",
            DateTime.Now,
            ", Now UTC: ",
            DateTime.UtcNow);
    }
}
"""
        );
    }
}
