// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1261DisposeResourceAsynchronouslyTests : AbstractCSharpDiagnosticVerifier<DisposeResourceAsynchronouslyAnalyzer, LocalDeclarationStatementCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.DisposeResourceAsynchronously;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DisposeResourceAsynchronously)]
    public async Task Test_WithAwait()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Threading.Tasks;

class C
{
    async Task FooAsync()
    {
        [|using var disposable = await GetDisposableAsync();|]
    }

    private Task<Disposable> GetDisposableAsync()
    {
        throw new NotImplementedException();
    }
}

internal class Disposable : IDisposable, IAsyncDisposable
{
    public void Dispose() => throw new NotImplementedException();
    public ValueTask DisposeAsync() => throw new NotImplementedException();
}
", @"
using System;
using System.Threading.Tasks;

class C
{
    async Task FooAsync()
    {
        await using var disposable = await GetDisposableAsync();
    }

    private Task<Disposable> GetDisposableAsync()
    {
        throw new NotImplementedException();
    }
}

internal class Disposable : IDisposable, IAsyncDisposable
{
    public void Dispose() => throw new NotImplementedException();
    public ValueTask DisposeAsync() => throw new NotImplementedException();
}
");
    }
}
