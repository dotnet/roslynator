﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Formatting.CodeFixes.CSharp;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests;

public class RCS0013AddBlankLineBetweenSingleLineDeclarationsOfDifferentKindTests : AbstractCSharpDiagnosticVerifier<BlankLineBetweenDeclarationsAnalyzer, BlankLineBetweenDeclarationsCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.AddBlankLineBetweenSingleLineDeclarationsOfDifferentKind;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBetweenSingleLineDeclarationsOfDifferentKind)]
    public async Task Test_MemberDeclaration_PropertyAndIndexer()
    {
        await VerifyDiagnosticAndFixAsync(@"
abstract class C
{
    public abstract string P { get; set; }[|
|]    public abstract string this[int index] { get; }
}
", @"
abstract class C
{
    public abstract string P { get; set; }

    public abstract string this[int index] { get; }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBetweenSingleLineDeclarationsOfDifferentKind)]
    public async Task Test_ConstAndField()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    const string K = null;[|
|]    string F = null;
}
", @"
class C
{
    const string K = null;

    string F = null;
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBetweenSingleLineDeclarationsOfDifferentKind)]
    public async Task TestNoDiagnostic_MultilineMethod()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
    }
    string P { get; set; }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBetweenSingleLineDeclarationsOfDifferentKind)]
    public async Task TestNoDiagnostic_Properties()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    string P1 { get; set; }
    string P2 { get; set; }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBlankLineBetweenSingleLineDeclarationsOfDifferentKind)]
    public async Task TestNoDiagnostic_Event2()
    {
        await VerifyNoDiagnosticAsync(@"
using System;

class C
{
    public event EventHandler E1;
    public event EventHandler E2
    {
        add { }
        remove { }
    }
}
");
    }
}
