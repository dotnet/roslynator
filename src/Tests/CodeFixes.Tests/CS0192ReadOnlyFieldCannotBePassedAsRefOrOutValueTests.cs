// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS0192ReadOnlyFieldCannotBePassedAsRefOrOutValueTests : AbstractCSharpCompilerDiagnosticFixVerifier<ArgumentCodeFixProvider>
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.CS0192_ReadOnlyFieldCannotBePassedAsRefOrOutValue;

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS0192_ReadOnlyFieldCannotBePassedAsRefOrOutValue)]
        public async Task Test_MakeFieldWritable()
        {
            await VerifyFixAsync(@"
class C
{
    private readonly string _f;

    void M(out string p)
    {
        M(out _f)
    }
}
", @"
class C
{
    private string _f;

    void M(out string p)
    {
        M(out _f)
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId, CodeFixIdentifiers.MakeFieldWritable));
        }
    }
}
