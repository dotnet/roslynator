// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS1012TooManyCharactersInCharacterLiteralTests : AbstractCSharpCompilerDiagnosticFixVerifier
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.TooManyCharactersInCharacterLiteral;

        public override CodeFixProvider FixProvider { get; } = new LiteralExpressionCodeFixProvider();

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.TooManyCharactersInCharacterLiteral)]
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
