// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Xunit;

#pragma warning disable CA1034

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public abstract class CS0136LocalOrParameterCannotBeDeclaredInThisScopeTests : AbstractCSharpCompilerDiagnosticFixVerifier
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.LocalOrParameterCannotBeDeclaredInThisScopeBecauseThatNameIsUsedInEnclosingScopeToDefineLocalOrParameter;

        public class LocalTests : CS0136LocalOrParameterCannotBeDeclaredInThisScopeTests
        {
            public override CodeFixProvider FixProvider { get; } = new VariableDeclarationCodeFixProvider();

            [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.LocalOrParameterCannotBeDeclaredInThisScopeBecauseThatNameIsUsedInEnclosingScopeToDefineLocalOrParameter)]
            public async Task Test_ReplaceVariableDeclarationWithAssignment()
            {
                await VerifyFixAsync(@"
class C
{
    void M()
    {
        bool f = false;
        string s = null;

        if (f)
        {
            string s = null;
        }
    }
}
", @"
class C
{
    void M()
    {
        bool f = false;
        string s = null;

        if (f)
        {
            s = null;
        }
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
            }

            [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.LocalOrParameterCannotBeDeclaredInThisScopeBecauseThatNameIsUsedInEnclosingScopeToDefineLocalOrParameter)]
            public async Task TestNoFix()
            {
                await VerifyNoFixAsync(
    @"
class C
{
    void M()
    {
            if (true)
            {
                string s = "";
            }

            string s = null;
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
            }
        }

        public class ParameterTests : CS0136LocalOrParameterCannotBeDeclaredInThisScopeTests
        {
            public override CodeFixProvider FixProvider { get; } = new ParameterCannotBeDeclaredInThisScopeCodeFixProvider();

            [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.LocalOrParameterCannotBeDeclaredInThisScopeBecauseThatNameIsUsedInEnclosingScopeToDefineLocalOrParameter)]
            public async Task Test_RemoveParameter()
            {
                await VerifyFixAsync(@"
class C
{
    void M()
    {
        string value = null;

        void LF(string value)
        {
        }
    }
}
", @"
class C
{
    void M()
    {
        string value = null;

        void LF()
        {
        }
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
            }
        }
    }
}
