// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS8604PossibleNullReferenceArgumentForParameter : AbstractCSharpCompilerDiagnosticFixVerifier<TokenCodeFixProvider>
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.CS8604_PossibleNullReferenceArgumentForParameter;

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS8604_PossibleNullReferenceArgumentForParameter)]
        public async Task Test()
        {
            await VerifyFixAsync(@"
#nullable enable

class C
{
    public C(string p)
    {
        P = p;
    }

    public string? P { get; }

    void M()
    {
        var x = new C(P);
    }
}
", @"
#nullable enable

class C
{
    public C(string p)
    {
        P = p;
    }

    public string? P { get; }

    void M()
    {
        var x = new C(P!);
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }
    }
}
