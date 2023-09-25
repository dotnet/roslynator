// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests;

public class CS0164LabelHasNotBeenReferencedTests : AbstractCSharpCompilerDiagnosticFixVerifier<LabeledStatementCodeFixProvider>
{
    public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.CS0164_LabelHasNotBeenReferenced;

    [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS0164_LabelHasNotBeenReferenced)]
    public async Task Test()
    {
        await VerifyFixAsync(@"
using System;

class C
{
    int M()
    {
    start:
        return 1;
    }
}
", @"
using System;

class C
{
    int M()
    {
        return 1;
    }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
    }
}
