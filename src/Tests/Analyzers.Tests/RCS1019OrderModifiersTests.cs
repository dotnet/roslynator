// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1019OrderModifiersTests : AbstractCSharpDiagnosticVerifier<OrderModifiersAnalyzer, MemberDeclarationCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.OrderModifiers;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.OrderModifiers)]
    public async Task Test()
    {
        await VerifyNoDiagnosticAsync("""
file static class C
{
}
""");
    }
}
