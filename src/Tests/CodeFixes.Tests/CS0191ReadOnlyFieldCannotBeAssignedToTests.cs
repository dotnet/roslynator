// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS0191ReadOnlyFieldCannotBeAssignedToTests : AbstractCSharpCompilerDiagnosticFixVerifier
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.ReadOnlyFieldCannotBeAssignedTo;

        public override CodeFixProvider FixProvider { get; } = new ExpressionCodeFixProvider();

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.ReadOnlyFieldCannotBeAssignedTo)]
        public async Task Test()
        {
            await VerifyFixAsync(@"
class C
{
    private readonly string _f = null;

    void M()
    {
        _f = null;
    }
}
", @"
class C
{
    private string _f = null;

    void M()
    {
        _f = null;
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }
    }
}
