// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS0119NameIsNotValidInGivenContextTests : AbstractCSharpCompilerDiagnosticFixVerifier
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.NameIsNotValidInGivenContext;

        public override CodeFixProvider FixProvider { get; } = new SimpleNameCodeFixProvider();

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.NameIsNotValidInGivenContext)]
        public async Task Test()
        {
            await VerifyFixAsync(@"
class C
{
    void M()
    {
        string s = null;
        s = s.ToString.ToString();
    }
}
", @"
class C
{
    void M()
    {
        string s = null;
        s = s.ToString().ToString();
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }
    }
}
