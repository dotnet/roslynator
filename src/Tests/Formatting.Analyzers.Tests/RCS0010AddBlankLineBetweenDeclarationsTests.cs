﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Formatting.CodeFixes.CSharp;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests;

public class RCS0010AddBlankLineBetweenDeclarationsTests : AbstractCSharpDiagnosticVerifier<BlankLineBetweenDeclarationsAnalyzer, BlankLineBetweenDeclarationsCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.AddBlankLineBetweenDeclarations;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBetweenDeclarations)]
    public async Task Test_MemberDeclaration_FirstIsMultiline()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
    }[|
|]    string P { get; set; }
}
", @"
class C
{
    void M()
    {
    }

    string P { get; set; }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBetweenDeclarations)]
    public async Task Test_MemberDeclaration_SecondIsMultiline()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string P { get; set; }[|
|]    void M()
    {
    }
}
", @"
class C
{
    string P { get; set; }

    void M()
    {
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBetweenDeclarations)]
    public async Task Test_MemberDeclaration_BothAreMultiline()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M1()
    {
    }[|
|]    void M2()
    {
    }
}
", @"
class C
{
    void M1()
    {
    }

    void M2()
    {
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBetweenDeclarations)]
    public async Task Test_EnumMemberDeclaration_FirstIsMultiline()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;

enum E
{
    [Obsolete]
    A = 0,[|
|]    B = 1
}
", @"
using System;

enum E
{
    [Obsolete]
    A = 0,

    B = 1
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBetweenDeclarations)]
    public async Task Test_EnumMemberDeclaration_SecondIsMultiline()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;

enum E
{
    A = 0,[|
|]    [Obsolete]
    B = 1
}
", @"
using System;

enum E
{
    A = 0,

    [Obsolete]
    B = 1
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBetweenDeclarations)]
    public async Task Test_EnumMemberDeclaration_BothAreMultiline()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;

enum E
{
    [Obsolete]
    A = 0,[|
|]    [Obsolete]
    B = 1
}
", @"
using System;

enum E
{
    [Obsolete]
    A = 0,

    [Obsolete]
    B = 1
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBetweenDeclarations)]
    public async Task Test_FileScopedNamespace()
    {
        await VerifyDiagnosticAndFixAsync("""
namespace N;

class Foo
{
  void M() { }
}[|
|]enum Bar
{
  A = 0
}
""", """
namespace N;

class Foo
{
  void M() { }
}

enum Bar
{
  A = 0
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBetweenDeclarations)]
    public async Task TestNoDiagnostic_MemberDeclaration_SingleLine()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    string P1 { get; set; }
    string P2 { get; set; }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBetweenDeclarations)]
    public async Task TestNoDiagnostic_EnumMemberDeclaration_SingleLine()
    {
        await VerifyNoDiagnosticAsync(@"
enum E
{
    A = 0,
    B = 1
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBetweenDeclarations)]
    public async Task TestNoDiagnostic_MemberDeclaration_DocumentationComment()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M1()
    {
    }
    /// <summary>
    /// x
    /// </summary>
    void M2()
    {
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBetweenDeclarations)]
    public async Task TestNoDiagnostic_EnumMemberDeclaration_DocumentationComment()
    {
        await VerifyNoDiagnosticAsync(@"
enum E
{
    A = 0,
    /// <summary>
    /// x
    /// </summary>
    B = 1
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBetweenDeclarations)]
    public async Task TestNoDiagnostic_GlobalStatement()
    {
        await VerifyNoDiagnosticAsync("""
var s = "";
s = s
    .ToString();
""", options: Options.WithCompilationOptions(Options.CompilationOptions.WithOutputKind(OutputKind.ConsoleApplication)));
    }
}
