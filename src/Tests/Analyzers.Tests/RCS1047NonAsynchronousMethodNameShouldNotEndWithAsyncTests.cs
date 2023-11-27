// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Testing;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1047NonAsynchronousMethodNameShouldNotEndWithAsyncTests : AbstractCSharpDiagnosticVerifier<AsyncSuffixAnalyzer, EmptyCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.NonAsynchronousMethodNameShouldNotEndWithAsync;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NonAsynchronousMethodNameShouldNotEndWithAsync)]
    public async Task Test()
    {
        await VerifyDiagnosticAsync(@"
using System.Threading.Tasks;

class C
{
    void [|FooAsync|]()
    {
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NonAsynchronousMethodNameShouldNotEndWithAsync)]
    public async Task Test_Tuple()
    {
        await VerifyDiagnosticAsync(@"
using System.Threading.Tasks;

class C
{
    (string s1, string s2) [|FooAsync|]()
    {
        return default((string, string));
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NonAsynchronousMethodNameShouldNotEndWithAsync)]
    public async Task Test_String()
    {
        await VerifyDiagnosticAsync(@"
using System.Threading.Tasks;

class C
{
    string [|FooAsync|]()
    {
        return null;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NonAsynchronousMethodNameShouldNotEndWithAsync)]
    public async Task Test_Array()
    {
        await VerifyDiagnosticAsync(@"
using System.Threading.Tasks;

class C
{
    string[] [|FooAsync|]()
    {
        return null;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NonAsynchronousMethodNameShouldNotEndWithAsync)]
    public async Task Test_T()
    {
        await VerifyDiagnosticAsync(@"
using System.Threading.Tasks;

class C
{
    T [|FooAsync|]<T>()
    {
        return default(T);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NonAsynchronousMethodNameShouldNotEndWithAsync)]
    public async Task TestNoDiagnostic_Task()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Threading.Tasks;

class C
{
    async Task<object> GetAsync()
    {
        return await Task.FromResult<object>(null);
    }

    Task<object> TaskOfTAsync()
    {
        return Task.FromResult<object>(null);
    }

    T TaskOfTAsync<T>() where T : Task<object>
    {
        return default(T);
    }

    Task TaskAsync()
    {
        return default(Task);
    }

    T TaskAsync<T>() where T : Task
    {
        return default(T);
    }

    ValueTask<object> ValueTaskOfTAsync()
    {
        return default(ValueTask<object>);
    }

    ValueTask ValueTaskAsync()
    {
        return default(ValueTask);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NonAsynchronousMethodNameShouldNotEndWithAsync)]
    public async Task TestNoDiagnostic_AsyncEnumerable()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

class C
{
    IAsyncEnumerable<object> GetAsyncEnumerableAsync()
    {
        return null;
    }

    AsyncEnumerableImpl GetAsyncEnumerableImplAsync()
    {
        return null;
    }

    T GetAsyncEnumerableImplOfAsync<T>() where T : AsyncEnumerableImpl
    {
        return default(T);
    }

    T AsyncEnumerableOfTAsync<T>() where T : IAsyncEnumerable<object>
    {
        return default(T);
    }

    InheritsImpl GetInheritingImplAsync()
    {
        return null;
    }

    T GetInheritingImplOfAsync<T>() where T : InheritsImpl
    {
        return default(T);
    }

    DuckTyped GetDuckTypedAsync()
    {
        return null;
    }

    T GetDuckTypedOfTAsync<T>() where T : DuckTyped
    {
        return default(T);
    }
}

class AsyncEnumerableImpl : IAsyncEnumerable<object>
{
    public IAsyncEnumerator<object> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return null;
    }
}

class InheritsImpl : AsyncEnumerableImpl
{
}

class DuckTyped
{
    public IAsyncEnumerator<object> GetAsyncEnumerator()
    {
        return null;
    }
}
");
    }
}
