// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS8618_NonNullableMemberIsUninitializedTests : AbstractCSharpCompilerDiagnosticFixVerifier<TokenCodeFixProvider>
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.CS8618_NonNullableMemberIsUninitialized;

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS8618_NonNullableMemberIsUninitialized)]
        public async Task Test_Property()
        {
            await VerifyFixAsync(@"
#nullable enable

class C
{
    public string P { get; set; } //x
}
", @"
#nullable enable

class C
{
    public string P { get; set; } = null!; //x
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS8618_NonNullableMemberIsUninitialized)]
        public async Task Test_Field()
        {
            await VerifyFixAsync(@"
#nullable enable

class C
{
    private string F;
}
", @"
#nullable enable

class C
{
    private string F = null!;
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }
    }
}
