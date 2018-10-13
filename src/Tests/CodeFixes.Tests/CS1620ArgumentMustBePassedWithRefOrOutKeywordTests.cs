// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS1620ArgumentMustBePassedWithRefOrOutKeywordTests : AbstractCSharpCompilerDiagnosticFixVerifier
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.ArgumentMustBePassedWithRefOrOutKeyword;

        public override CodeFixProvider FixProvider { get; } = new ArgumentCodeFixProvider();

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.ArgumentMustBePassedWithRefOrOutKeyword)]
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

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.ArgumentMustBePassedWithRefOrOutKeyword)]
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
