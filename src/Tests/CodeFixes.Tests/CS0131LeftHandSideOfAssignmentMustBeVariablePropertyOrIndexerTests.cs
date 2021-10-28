// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS0131LeftHandSideOfAssignmentMustBeVariablePropertyOrIndexerTests : AbstractCSharpCompilerDiagnosticFixVerifier<ExpressionCodeFixProvider>
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.CS0131_LeftHandSideOfAssignmentMustBeVariablePropertyOrIndexer;

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS0131_LeftHandSideOfAssignmentMustBeVariablePropertyOrIndexer)]
        public async Task Test()
        {
            await VerifyFixAsync(@"
class C
{
    void M()
    {
        const string s = null;
        s = null;
    }
}
", @"
class C
{
    void M()
    {
        string s = null;
        s = null;
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }
    }
}
