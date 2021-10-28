// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS1012TooManyCharactersInCharacterLiteralTests : AbstractCSharpCompilerDiagnosticFixVerifier<LiteralExpressionCodeFixProvider>
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.CS1012_TooManyCharactersInCharacterLiteral;

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS1012_TooManyCharactersInCharacterLiteral)]
        public async Task Test()
        {
            await VerifyFixAsync(@"
class C
{
    void M()
    {
        var x = 'a\'\""\nb';
    }
}
", @"
class C
{
    void M()
    {
        var x = ""a\'\""\nb"";
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }
    }
}
