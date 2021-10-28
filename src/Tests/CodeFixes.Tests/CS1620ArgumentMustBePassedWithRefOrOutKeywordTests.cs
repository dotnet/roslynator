// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS1620ArgumentMustBePassedWithRefOrOutKeywordTests : AbstractCSharpCompilerDiagnosticFixVerifier<ArgumentCodeFixProvider>
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.CS1620_ArgumentMustBePassedWithRefOrOutKeyword;

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS1620_ArgumentMustBePassedWithRefOrOutKeyword)]
        public async Task Test_Out()
        {
            await VerifyFixAsync(@"
class C
{
    void M(out string value)
    {
        value = null;
        M(value);
    }
}
", @"
class C
{
    void M(out string value)
    {
        value = null;
        M(out value);
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS1620_ArgumentMustBePassedWithRefOrOutKeyword)]
        public async Task Test_Ref()
        {
            await VerifyFixAsync(@"
class C
{
    void M(ref string value)
    {
        value = null;
        M(value);
    }
}
", @"
class C
{
    void M(ref string value)
    {
        value = null;
        M(ref value);
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }
    }
}
