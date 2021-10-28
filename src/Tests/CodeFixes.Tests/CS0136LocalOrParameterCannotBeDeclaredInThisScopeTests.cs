// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS0136LocalOrParameterCannotBeDeclaredInThisScopeTests : AbstractCSharpCompilerDiagnosticFixVerifier<VariableDeclarationCodeFixProvider>
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.CS0136_LocalOrParameterCannotBeDeclaredInThisScopeBecauseThatNameIsUsedInEnclosingScopeToDefineLocalOrParameter;

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS0136_LocalOrParameterCannotBeDeclaredInThisScopeBecauseThatNameIsUsedInEnclosingScopeToDefineLocalOrParameter)]
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

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS0136_LocalOrParameterCannotBeDeclaredInThisScopeBecauseThatNameIsUsedInEnclosingScopeToDefineLocalOrParameter)]
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
}
