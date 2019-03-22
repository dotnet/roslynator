// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1161EnumShouldDeclareExplicitValuesTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.EnumShouldDeclareExplicitValues;

        public override DiagnosticAnalyzer Analyzer { get; } = new EnumShouldDeclareExplicitValuesAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new EnumDeclarationCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.EnumShouldDeclareExplicitValues)]
        public async Task Test_AllValues()
        {
            await VerifyDiagnosticAndFixAsync(@"
enum [|Foo|]
{
    A,
    B,
    C,
    D,
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

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.EnumShouldDeclareExplicitValues)]
        public async Task Test_WithComments()
        {
            await VerifyDiagnosticAndFixAsync(@"
enum [|Foo|]
{
    A, //a
    B, //b
    C, //c
}
", @"
enum Foo
{
    A = 0, //a
    B = 1, //b
    C = 2, //c
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.EnumShouldDeclareExplicitValues)]
        public async Task Test_SomeValues()
        {
            await VerifyDiagnosticAndFixAsync(@"
enum [|Foo|]
{
    _,
    A = 1,
    B = 2,
    C,
    D = 4,
    E,
}
", @"
enum Foo
{
    _ = 0,
    A = 1,
    B = 2,
    C = 3,
    D = 4,
    E = 5,
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.EnumShouldDeclareExplicitValues)]
        public async Task Test_Flags()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

[Flags]
enum [|Foo|]
{
    A,
    B,
    C,
    D,
}
", @"
using System;

[Flags]
enum Foo
{
    A = 0,
    B = 1,
    C = 2,
    D = 4,
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.EnumShouldDeclareExplicitValues)]
        public async Task Test_Flags2()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

[Flags]
enum [|Foo|]
{
    A = 1,
    B,
    C = 4,
    D,
    E = 16,
    F
}
", @"
using System;

[Flags]
enum Foo
{
    A = 1,
    B = 2,
    C = 4,
    D = 8,
    E = 16,
    F = 32
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.EnumShouldDeclareExplicitValues)]
        public async Task Test_Flags_SByte()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

[Flags]
enum [|Foo|] : sbyte
{
    A,
    B,
    C,
    D,
    E,
    F,
    G,
    H
}
", @"
using System;

[Flags]
enum Foo : sbyte
{
    A = 0,
    B = 1,
    C = 2,
    D = 4,
    E = 8,
    F = 16,
    G = 32,
    H = 64
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.EnumShouldDeclareExplicitValues)]
        public async Task Test_Flags_SByte_MaxValue()
        {
            await VerifyNoFixAsync(@"
using System;

[Flags]
enum Foo : sbyte
{
    A = 0,
    B = 1,
    C = 2,
    D = 4,
    E = 8,
    F = 16,
    G = 32,
    H = 64,
    I
}
");
        }
    }
}
