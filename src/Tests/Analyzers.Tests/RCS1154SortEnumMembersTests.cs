// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1154SortEnumMembersTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.SortEnumMembers;

        public override DiagnosticAnalyzer Analyzer { get; } = new SortEnumMembersAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new EnumDeclarationCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SortEnumMembers)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
enum [|Foo|]
{
    B = 1,
    A = 0,
    D = 3,
    C = 2
}
", @"
enum Foo
{
    A = 0,
    B = 1,
    C = 2,
    D = 3
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SortEnumMembers)]
        public async Task Test_TrailingSeparator()
        {
            await VerifyDiagnosticAndFixAsync(@"
enum [|Foo|]
{
    B = 1,
    A = 0,
    D = 3,
    C = 2,
}
", @"
enum Foo
{
    A = 0,
    B = 1,
    C = 2,
    D = 3,
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SortEnumMembers)]
        public async Task Test_EmptyLines()
        {
            await VerifyDiagnosticAndFixAsync(@"
enum [|Foo|]
{
    B = 1,

    A = 0,

    D = 3,

    C = 2
}
", @"
enum Foo
{
    A = 0,

    B = 1,

    C = 2,

    D = 3
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SortEnumMembers)]
        public async Task Test_WithComments()
        {
            await VerifyDiagnosticAndFixAsync(@"
enum [|Foo|]
{
    /// <summary>B</summary>
    B = 1,
    /// <summary>A</summary>
    A = 0,
    /// <summary>D</summary>
    D = 3,
    /// <summary>C</summary>
    C = 2,
}
", @"
enum Foo
{
    /// <summary>A</summary>
    A = 0,
    /// <summary>B</summary>
    B = 1,
    /// <summary>C</summary>
    C = 2,
    /// <summary>D</summary>
    D = 3,
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SortEnumMembers)]
        public async Task Test_Comments_EmptyLines()
        {
            await VerifyDiagnosticAndFixAsync(@"
enum [|Foo|]
{
    /// <summary>B</summary>
    B = 1,

    /// <summary>A</summary>
    A = 0,

    /// <summary>D</summary>
    D = 3,

    /// <summary>C</summary>
    C = 2
}
", @"
enum Foo
{
    /// <summary>A</summary>
    A = 0,

    /// <summary>B</summary>
    B = 1,

    /// <summary>C</summary>
    C = 2,

    /// <summary>D</summary>
    D = 3
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SortEnumMembers)]
        public async Task Test_Comments_EmptyLines_TrailingSeparator()
        {
            await VerifyDiagnosticAndFixAsync(@"
enum [|Foo|]
{
    /// <summary>B</summary>
    B = 1,

    /// <summary>A</summary>
    A = 0,

    /// <summary>D</summary>
    D = 3,

    /// <summary>C</summary>
    C = 2,
}
", @"
enum Foo
{
    /// <summary>A</summary>
    A = 0,

    /// <summary>B</summary>
    B = 1,

    /// <summary>C</summary>
    C = 2,

    /// <summary>D</summary>
    D = 3,
}
");
        }
    }
}
