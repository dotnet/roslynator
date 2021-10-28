// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS0103NameDoesNotExistInCurrentContextTests : AbstractCSharpCompilerDiagnosticFixVerifier<IdentifierNameCodeFixProvider>
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.CS0103_NameDoesNotExistInCurrentContext;

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS0103_NameDoesNotExistInCurrentContext)]
        public async Task Test()
        {
            await VerifyFixAsync(@"
class C
{
    void M(out string x)
    {
        M(out abc);
    }
}
", @"
class C
{
    void M(out string x)
    {
        M(out string abc);
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId, "string"));
        }

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS0103_NameDoesNotExistInCurrentContext)]
        public async Task Test_NullableReferenceType()
        {
            await VerifyFixAsync(@"
class C
{
    void M(out string? x)
    {
        M(out abc);
    }
}
", @"
class C
{
    void M(out string? x)
    {
        M(out string? abc);
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId, "string?"), options: WellKnownCSharpTestOptions.Default_NullableReferenceTypes);
        }
    }
}
