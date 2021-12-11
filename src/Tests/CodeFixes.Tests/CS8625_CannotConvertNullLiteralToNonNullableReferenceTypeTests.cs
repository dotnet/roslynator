// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS8625_CannotConvertNullLiteralToNonNullableReferenceTypeTests : AbstractCSharpCompilerDiagnosticFixVerifier<UseNullForgivingOperatorCodeFixProvider>
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.CS8625_CannotConvertNullLiteralToNonNullableReferenceType;

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS8625_CannotConvertNullLiteralToNonNullableReferenceType)]
        public async Task Test_Property_Null()
        {
            await VerifyFixAsync(@"
#nullable enable

class C
{
    public string P { get; set; } = null;
}
", @"
#nullable enable

class C
{
    public string P { get; set; } = null!;
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS8625_CannotConvertNullLiteralToNonNullableReferenceType)]
        public async Task Test_Property_DefaultLiteral()
        {
            await VerifyFixAsync(@"
#nullable enable

class C
{
    public string P { get; set; } = default;
}
", @"
#nullable enable

class C
{
    public string P { get; set; } = default!;
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS8625_CannotConvertNullLiteralToNonNullableReferenceType)]
        public async Task Test_Property_DefaultExpression()
        {
            await VerifyFixAsync(@"
#nullable enable

class C
{
    public string P { get; set; } = default(string);
}
", @"
#nullable enable

class C
{
    public string P { get; set; } = default(string)!;
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS8625_CannotConvertNullLiteralToNonNullableReferenceType)]
        public async Task Test_Field()
        {
            await VerifyFixAsync(@"
#nullable enable

class C
{
    private string F = null;
}
", @"
#nullable enable

class C
{
    private string F = null!;
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }


        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS8625_CannotConvertNullLiteralToNonNullableReferenceType)]
        public async Task Test_Field_DefaultLiteral()
        {
            await VerifyFixAsync(@"
#nullable enable

class C
{
    private string F = default;
}
", @"
#nullable enable

class C
{
    private string F = default!;
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS8625_CannotConvertNullLiteralToNonNullableReferenceType)]
        public async Task Test_Field_DefaultExpression()
        {
            await VerifyFixAsync(@"
#nullable enable

class C
{
    private string F = default(string);
}
", @"
#nullable enable

class C
{
    private string F = default(string)!;
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }
    }
}
