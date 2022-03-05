// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1254NormalizeFormatOfEnumFlagValueTests : AbstractCSharpDiagnosticVerifier<NormalizeFormatOfEnumFlagValueAnalyzer, EnumMemberDeclarationCodeFixProvider>
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.NormalizeFormatOfEnumFlagValue;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeFormatOfEnumFlagValue)]
        public async Task Test_DecimalToShift()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

[Flags]
enum Foo
{
    _ = 0,
    A = 1,
    B = [|2|],
    AB = 3,
    C = ([|4|]),
}
", @"
using System;

[Flags]
enum Foo
{
    _ = 0,
    A = 1,
    B = 1 << 1,
    AB = 3,
    C = (1 << 2),
}
", options: Options.AddConfigOption(ConfigOptionKeys.EnumFlagValueStyle, ConfigOptionValues.EnumFlagValueStyle_ShiftOperator));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeFormatOfEnumFlagValue)]
        public async Task Test_ShiftToDecimal()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

[Flags]
enum Foo
{
    _ = 0,
    A = 1,
    B = [|1 << 1|],
    AB = 3,
    C = ([|1 << 2|]),
}
", @"
using System;

[Flags]
enum Foo
{
    _ = 0,
    A = 1,
    B = 2,
    AB = 3,
    C = (4),
}
", options: Options.AddConfigOption(ConfigOptionKeys.EnumFlagValueStyle, ConfigOptionValues.EnumFlagValueStyle_DecimalNumber));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeFormatOfEnumFlagValue)]
        public async Task Test_HexadecimalToShift()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

[Flags]
enum Foo
{
    _ = 0,
    A = 1,
    B = [|0x2|],
    C = [|0X4|],
}
", @"
using System;

[Flags]
enum Foo
{
    _ = 0,
    A = 1,
    B = 1 << 1,
    C = 1 << 2,
}
", options: Options.AddConfigOption(ConfigOptionKeys.EnumFlagValueStyle, ConfigOptionValues.EnumFlagValueStyle_ShiftOperator));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeFormatOfEnumFlagValue)]
        public async Task Test_HexadecimalToDecimal()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

[Flags]
enum Foo
{
    _ = 0,
    A = 1,
    B = [|0x2|],
    C = [|0X4|],
}
", @"
using System;

[Flags]
enum Foo
{
    _ = 0,
    A = 1,
    B = 2,
    C = 4,
}
", options: Options.AddConfigOption(ConfigOptionKeys.EnumFlagValueStyle, ConfigOptionValues.EnumFlagValueStyle_DecimalNumber));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeFormatOfEnumFlagValue)]
        public async Task Test_BinaryToShift()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

[Flags]
enum Foo
{
    _ = 0,
    A = 1,
    B = [|0b10|],
    C = [|0B100|],
}
", @"
using System;

[Flags]
enum Foo
{
    _ = 0,
    A = 1,
    B = 1 << 1,
    C = 1 << 2,
}
", options: Options.AddConfigOption(ConfigOptionKeys.EnumFlagValueStyle, ConfigOptionValues.EnumFlagValueStyle_ShiftOperator));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeFormatOfEnumFlagValue)]
        public async Task Test_BinaryToDecimal()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

[Flags]
enum Foo
{
    _ = 0,
    A = 1,
    B = [|0b10|],
    C = [|0B100|],
}
", @"
using System;

[Flags]
enum Foo
{
    _ = 0,
    A = 1,
    B = 2,
    C = 4,
}
", options: Options.AddConfigOption(ConfigOptionKeys.EnumFlagValueStyle, ConfigOptionValues.EnumFlagValueStyle_DecimalNumber));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeFormatOfEnumFlagValue)]
        public async Task TestNoDiagnostic_WithoutFlags()
        {
            await VerifyNoDiagnosticAsync(@"
enum Foo
{
    _ = 0,
    A = 1,
    B = 2,
    C = 4,
}
", options: Options.AddConfigOption(ConfigOptionKeys.EnumFlagValueStyle, ConfigOptionValues.EnumFlagValueStyle_ShiftOperator));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeFormatOfEnumFlagValue)]
        public async Task TestNoDiagnostic_BitShift()
        {
            await VerifyNoDiagnosticAsync(@"
using System;

[Flags]
enum Foo
{
    _ = 0,
    A = 1,
    B = 1 << 1,
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeFormatOfEnumFlagValue)]
        public async Task TestNoDiagnostic_CombinedValue()
        {
            await VerifyNoDiagnosticAsync(@"
using System;

[Flags]
enum Foo
{
    _ = 0,
    A = 1,
    B = 1 << 1,
    C = 1 << 2,
    D = 1 << 3,
    AB = 3,
    AC = 1 | 4,
    AD = 0b1001,
    X = int.MaxValue,
}
", options: Options.AddConfigOption(ConfigOptionKeys.EnumFlagValueStyle, ConfigOptionValues.EnumFlagValueStyle_ShiftOperator));
        }
    }
}
