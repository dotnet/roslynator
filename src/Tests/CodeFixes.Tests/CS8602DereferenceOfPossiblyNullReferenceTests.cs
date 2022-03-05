// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS8602DereferenceOfPossiblyNullReferenceTests : AbstractCSharpCompilerDiagnosticFixVerifier<TokenCodeFixProvider>
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.CS8602_DereferenceOfPossiblyNullReference;

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS8602_DereferenceOfPossiblyNullReference)]
        public async Task Test()
        {
            await VerifyFixAsync(@"
#nullable enable

public class C
{
    void M()
    {
    }

    void M2(string? c)
    {
        c.ToString().ToString().Length.ToString().ToCharArray()[0].E2().M();
    }
}

public static class E
{
    public static C E2(this char s)
    {
        return default;
    }
}
", @"
#nullable enable

public class C
{
    void M()
    {
    }

    void M2(string? c)
    {
        c?.ToString().ToString().Length.ToString().ToCharArray()[0].E2().M();
    }
}

public static class E
{
    public static C E2(this char s)
    {
        return default;
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }
    }
}
