﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Testing;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1046AsynchronousMethodNameShouldEndWithAsyncTests : AbstractCSharpDiagnosticVerifier<AsyncSuffixAnalyzer, EmptyCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.AsynchronousMethodNameShouldEndWithAsync;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AsynchronousMethodNameShouldEndWithAsync)]
    public async Task Test_Task()
    {
        await VerifyDiagnosticAsync(@"
using System.Threading.Tasks;

class B
{
    public virtual Task [|Foo|]()
    {
        return Task.CompletedTask;
    }
}

class C : B
{
    public override Task Foo()
    {
        return base.Foo();
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AsynchronousMethodNameShouldEndWithAsync)]
    public async Task Test_Task_TypeParameter()
    {
        await VerifyDiagnosticAsync(@"
using System.Threading.Tasks;

class C
{
    T [|Foo|]<T>() where T : Task
    {
        return default(T);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AsynchronousMethodNameShouldEndWithAsync)]
    public async Task Test_TaskOfT()
    {
        await VerifyDiagnosticAsync(@"
using System.Threading.Tasks;

class C
{
    Task<object> [|Foo|]()
    {
        return Task.FromResult(default(object));
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AsynchronousMethodNameShouldEndWithAsync)]
    public async Task Test_TaskOfT_TypeParameter()
    {
        await VerifyDiagnosticAsync(@"
using System.Threading.Tasks;

class C
{
    T [|Foo|]<T>() where T : Task<object>
    {
        return default(T);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AsynchronousMethodNameShouldEndWithAsync)]
    public async Task Test_ValueTaskOfT()
    {
        await VerifyDiagnosticAsync(@"
using System.Threading.Tasks;

class C
{
    ValueTask<object> [|Foo|]()
    {
        return default(ValueTask<object>);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AsynchronousMethodNameShouldEndWithAsync)]
    public async Task Test_DuckTyped()
    {
        await VerifyDiagnosticAsync(@"
class C
{
    DuckTyped [|Foo|]() => new();
    DuckTyped<object> [|Foo2|]() => new();
}

class DuckTyped
{
    public CustomAwaiter GetAwaiter() => new();
}

class DuckTyped<T>
{
    public CustomAwaiter<T> GetAwaiter() => new();
}

struct CustomAwaiter : System.Runtime.CompilerServices.INotifyCompletion
{
    public bool IsCompleted => true;
    public void OnCompleted(System.Action continuation) { }
    public void GetResult() { }
}

struct CustomAwaiter<T> : System.Runtime.CompilerServices.INotifyCompletion
{
    public bool IsCompleted => true;
    public void OnCompleted(System.Action continuation) { }
    public T GetResult() => default(T);
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AsynchronousMethodNameShouldEndWithAsync)]
    public async Task TestNoDiagnostic_EntryPointMethod()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        await Task.CompletedTask;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AsynchronousMethodNameShouldEndWithAsync)]
    public async Task TestNoDiagnostic_Interface()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Threading.Tasks;

interface IFoo
{
#pragma warning disable RCS1046
    Task Foo();
#pragma warning restore RCS1046
}

class C : IFoo
{
    public Task Foo() => Task.CompletedTask;
}
");
    }
}
