// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1129RemoveRedundantFieldInitializationTests : AbstractCSharpDiagnosticVerifier<RemoveRedundantFieldInitializationAnalyzer, VariableDeclaratorCodeFixProvider>
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.RemoveRedundantFieldInitialization;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantFieldInitialization)]
        public async Task Test_MultipleDeclarations()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    object _f [|= null|], _f2;
}
", @"
class C
{
    object _f, _f2;
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantFieldInitialization)]
        public async Task Test_Number()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    sbyte _sb [|= (sbyte)0|];
    byte _be [|= (byte)0|];
    short _st [|= (short)0|];
    ushort _us [|= (ushort)0|];
    int _ii [|= (int)0|];
    uint _ui [|= (uint)0|];
    long _lg [|= (long)0|];
    ulong _ul [|= (ulong)0|];
    float _ft [|= (float)0|];
    double _de [|= (double)0|];
    decimal _dl [|= (decimal)0|];
}
", @"
class C
{
    sbyte _sb;
    byte _be;
    short _st;
    ushort _us;
    int _ii;
    uint _ui;
    long _lg;
    ulong _ul;
    float _ft;
    double _de;
    decimal _dl;
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantFieldInitialization)]
        public async Task Test_Bool()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    bool _f [|= false|];
}
", @"
class C
{
    bool _f;
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantFieldInitialization)]
        public async Task Test_Bool_Nullable()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    bool? _f [|= null|];
    bool? _f2 [|= default(bool?)|];
}
", @"
class C
{
    bool? _f;
    bool? _f2;
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantFieldInitialization)]
        public async Task Test_Char()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    char _f [|= '\0'|];
}
", @"
class C
{
    char _f;
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantFieldInitialization)]
        public async Task Test_Int()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    int _f [|= 0|];
}
", @"
class C
{
    int _f;
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantFieldInitialization)]
        public async Task Test_ULong()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    ulong _f [|= 0|];
}
", @"
class C
{
    ulong _f;
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantFieldInitialization)]
        public async Task Test_Enum()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System.Text.RegularExpressions;

class C
{
    const RegexOptions RegexOptionsConst = RegexOptions.None;

    RegexOptions _f1 [|= default|];
    RegexOptions _f2 [|= default(RegexOptions)|];
    RegexOptions _f3 [|= (RegexOptions)0|];
}
", @"
using System.Text.RegularExpressions;

class C
{
    const RegexOptions RegexOptionsConst = RegexOptions.None;

    RegexOptions _f1;
    RegexOptions _f2;
    RegexOptions _f3;
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantFieldInitialization)]
        public async Task TestString()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    const string K = null;

    string _s1 [|= null|];
    string _s2 [|= default(string)|];
}
", @"
class C
{
    const string K = null;

    string _s1;
    string _s2;
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantFieldInitialization)]
        public async Task Test_StructWithoutConstructor()
        {
            await VerifyDiagnosticAndFixAsync(@"
struct C
{
    private int _f [|= 0|];
}
", @"
struct C
{
    private int _f;
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantFieldInitialization)]
        public async Task TestNoDiagnostic()
        {
            await VerifyNoDiagnosticAsync(@"
using System.Text.RegularExpressions;

class C
{
const bool K = false;

    bool _k = K;
    RegexOptions _r = RegexOptions.None;
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantFieldInitialization)]
        public async Task TestNoDiagnostic_StructWithConstructor()
        {
            await VerifyNoDiagnosticAsync(@"
struct C
{
    private int _f = 0;

    public C(object p)
    {
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantFieldInitialization)]
        public async Task TestNoDiagnostic_SuppressNullableWarningExpression()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    string _f = null!;
}
", options: CSharpTestOptions.Default
                .AddAllowedCompilerDiagnosticId("CS0414")
                .WithParseOptions(CSharpTestOptions.Default.ParseOptions.WithLanguageVersion(LanguageVersion.CSharp9)));
        }
    }
}
