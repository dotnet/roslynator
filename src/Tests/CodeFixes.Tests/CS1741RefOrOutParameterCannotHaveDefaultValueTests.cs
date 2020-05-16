// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS1741RefOrOutParameterCannotHaveDefaultValueTests : AbstractCSharpCompilerDiagnosticFixVerifier
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.RefOrOutParameterCannotHaveDefaultValue;

        public override CodeFixProvider FixProvider { get; } = new ParameterCodeFixProvider();

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.RefOrOutParameterCannotHaveDefaultValue)]
        public async Task Test_Out()
        {
            await VerifyFixAsync(@"
class C
{
    bool M(out string p = null)
    {
        p = null;
        return true;
    }
}
", @"
class C
{
    bool M(out string p)
    {
        p = null;
        return true;
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.RefOrOutParameterCannotHaveDefaultValue)]
        public async Task Test_Ref()
        {
            await VerifyFixAsync(@"
class C
{
    void M(ref string p = null)
    {
        p = null;
    }
}
", @"
class C
{
    void M(ref string p)
    {
        p = null;
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }
    }
}
