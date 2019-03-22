// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1057AddEmptyLineBetweenDeclarationsTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.AddEmptyLineBetweenDeclarations;

        public override DiagnosticAnalyzer Analyzer { get; } = new AddEmptyLineBetweenDeclarationsAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new AddEmptyLineBetweenDeclarationsCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineBetweenDeclarations)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    void M1()
    {
    }[|
|]    void M2()
    {
    }[|
|]    /// <summary>
    /// x
    /// </summary>
    void M3()
    {
    }[|
|]    void M4() { }[|
|]    /// <summary>
    /// x
    /// </summary>
    void M5() { }[|
|]    [Obsolete]
    string P1 { get; set; }[|
|]    string P2 { get; set; }[|
|]    [Obsolete]
    string P3 { get; set; }
}
", @"
using System;

class C
{
    void M1()
    {
    }

    void M2()
    {
    }

    /// <summary>
    /// x
    /// </summary>
    void M3()
    {
    }

    void M4() { }

    /// <summary>
    /// x
    /// </summary>
    void M5() { }

    [Obsolete]
    string P1 { get; set; }

    string P2 { get; set; }

    [Obsolete]
    string P3 { get; set; }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineBetweenDeclarations)]
        public async Task Test_Enum()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

enum E
{
    A = 0,[|
|]    /// <summary>
    /// x
    /// </summary>
    B = 1,[|
|]    [Obsolete]
    C = 2,[|
|]    D = 3,[|
|]    [Obsolete]
    E = 4,
}
", @"
using System;

enum E
{
    A = 0,

    /// <summary>
    /// x
    /// </summary>
    B = 1,

    [Obsolete]
    C = 2,

    D = 3,

    [Obsolete]
    E = 4,
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineBetweenDeclarations)]
        public async Task TestNoDiagnostic()
        {
            await VerifyNoDiagnosticAsync(@"
using System;

class C
{
    void M1() { }

    /// <summary>
    /// x
    /// </summary>
    void M2() { }

    void M3()
    {
    }

    /// <summary>
    /// x
    /// </summary>
    void M4()
    {
    }

    [Obsolete]
    string P1 { get; set; }

    string P2 { get; set; }
    string P3 { get; set; }

    [Obsolete]
    string P4 { get; set; }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineBetweenDeclarations)]
        public async Task TestNoDiagnostic_Enum()
        {
            await VerifyNoDiagnosticAsync(@"
using System;

enum E
{
    A = 0,

    /// <summary>
    /// x
    /// </summary>
    B = 1,

    C = 2,
    D = 3, E = 4,
}
");
        }
    }
}
