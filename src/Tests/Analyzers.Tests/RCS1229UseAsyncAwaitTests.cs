﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1229UseAsyncAwaitTests : AbstractCSharpDiagnosticVerifier<UseAsyncAwaitAnalyzer, UseAsyncAwaitCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.UseAsyncAwait;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseAsyncAwait)]
    public async Task Test_Method_TaskOfT()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Threading.Tasks;

class C
{
    Task<string> [|M|]()
    {
        using (default(IDisposable))
        {
            return GetAsync();
        }
    }

    Task<string> GetAsync() => Task.FromResult(default(string));
}
", @"
using System;
using System.Threading.Tasks;

class C
{
    async Task<string> M()
    {
        using (default(IDisposable))
        {
            return await GetAsync();
        }
    }

    Task<string> GetAsync() => Task.FromResult(default(string));
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseAsyncAwait)]
    public async Task Test_Method_Task()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Threading.Tasks;

class C
{
    Task [|M|]()
    {
        using (default(IDisposable))
        {
            return DoAsync();
        }
    }

    Task DoAsync() => Task.CompletedTask;
}
", @"
using System;
using System.Threading.Tasks;

class C
{
    async Task M()
    {
        using (default(IDisposable))
        {
            await DoAsync();
        }
    }

    Task DoAsync() => Task.CompletedTask;
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseAsyncAwait)]
    public async Task Test_Method_MultipleReturnStatements()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Threading.Tasks;

class C
{
    Task<string> [|M|]()
    {
        bool f = false;
        if (f)
        {
            using (default(IDisposable))
            {
                return GetAsync();
            }
        }
        else
        {
            using (default(IDisposable))
            {
                return GetAsync();
            }
        }
    }

    Task<string> GetAsync() => Task.FromResult(default(string));
}
", @"
using System;
using System.Threading.Tasks;

class C
{
    async Task<string> M()
    {
        bool f = false;
        if (f)
        {
            using (default(IDisposable))
            {
                return await GetAsync();
            }
        }
        else
        {
            using (default(IDisposable))
            {
                return await GetAsync();
            }
        }
    }

    Task<string> GetAsync() => Task.FromResult(default(string));
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseAsyncAwait)]
    public async Task Test_LocalFunction()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Threading.Tasks;

class C
{
    void M()
    {
        Task<string> [|LF|]()
        {
            using (default(IDisposable))
            {
                return GetAsync();
            }
        }
    }

    Task<string> GetAsync() => Task.FromResult(default(string));
}
", @"
using System;
using System.Threading.Tasks;

class C
{
    void M()
    {
        async Task<string> LF()
        {
            using (default(IDisposable))
            {
                return await GetAsync();
            }
        }
    }

    Task<string> GetAsync() => Task.FromResult(default(string));
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseAsyncAwait)]
    public async Task Test_SimpleLambda()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Threading.Tasks;

class C
{
    void M()
    {
        Func<object, Task<string>> func = [|_ =>
        {
            using (default(IDisposable))
            {
                return GetAsync();
            }
        }|];
    }

    Task<string> GetAsync() => Task.FromResult(default(string));
}
", @"
using System;
using System.Threading.Tasks;

class C
{
    void M()
    {
        Func<object, Task<string>> func = async _ =>
        {
            using (default(IDisposable))
            {
                return await GetAsync();
            }
        };
    }

    Task<string> GetAsync() => Task.FromResult(default(string));
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseAsyncAwait)]
    public async Task Test_ParenthesizedLambda()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Threading.Tasks;

class C
{
    void M()
    {
        Func<object, Task<string>> func = [|(_) =>
        {
            using (default(IDisposable))
            {
                return GetAsync();
            }
        }|];
    }

    Task<string> GetAsync() => Task.FromResult(default(string));
}
", @"
using System;
using System.Threading.Tasks;

class C
{
    void M()
    {
        Func<object, Task<string>> func = async (_) =>
        {
            using (default(IDisposable))
            {
                return await GetAsync();
            }
        };
    }

    Task<string> GetAsync() => Task.FromResult(default(string));
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseAsyncAwait)]
    public async Task Test_AnonymousMethod()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Threading.Tasks;

class C
{
    void M()
    {
        Func<object, Task<string>> func = [|delegate (object o)
        {
            using (default(IDisposable))
            {
                return GetAsync();
            }
        }|];
    }

    Task<string> GetAsync() => Task.FromResult(default(string));
}
", @"
using System;
using System.Threading.Tasks;

class C
{
    void M()
    {
        Func<object, Task<string>> func = async delegate (object o)
        {
            using (default(IDisposable))
            {
                return await GetAsync();
            }
        };
    }

    Task<string> GetAsync() => Task.FromResult(default(string));
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseAsyncAwait)]
    public async Task Test_UsingLocalDeclaration()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Threading.Tasks;

public class C
{
    public Task<string> [|M|]()
    {
        using var disposable = default(IDisposable);
        return GetAsync();
    }

    Task<string> GetAsync() => Task.FromResult(default(string));
}
", @"
using System;
using System.Threading.Tasks;

public class C
{
    public async Task<string> M()
    {
        using var disposable = default(IDisposable);
        return await GetAsync();
    }

    Task<string> GetAsync() => Task.FromResult(default(string));
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseAsyncAwait)]
    public async Task Test_UsingLocalDeclaration2()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Threading.Tasks;

public class C
{
    public Task<string> [|M|]()
    {
        {
            using var disposable = default(IDisposable);

            {
            }

            return GetAsync();
        }
    }

    Task<string> GetAsync() => Task.FromResult(default(string));
}
", @"
using System;
using System.Threading.Tasks;

public class C
{
    public async Task<string> M()
    {
        {
            using var disposable = default(IDisposable);

            {
            }

            return await GetAsync();
        }
    }

    Task<string> GetAsync() => Task.FromResult(default(string));
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseAsyncAwait)]
    public async Task Test_TryCatch()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Threading.Tasks;

class C
{
    Task<int> [|M|]()
    {
        var x = new C();

        try
        {
            return x.GetAsync();
        }
        finally
        {
        }
    }

    Task<int> GetAsync() => Task.FromResult(0);
}
", @"
using System;
using System.Threading.Tasks;

class C
{
    async Task<int> M()
    {
        var x = new C();

        try
        {
            return await x.GetAsync();
        }
        finally
        {
        }
    }

    Task<int> GetAsync() => Task.FromResult(0);
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseAsyncAwait)]
    public async Task Test_DuckTyped_TaskType()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

class C
{
    DuckTyped [|M|]()
    {
        using (default(IDisposable))
        {
            return GetAsync();
        }
    }

    DuckTyped<int> [|M2|]()
    {
        using (default(IDisposable))
        {
            return GetAsync<int>();
        }
    }

    DuckTyped GetAsync() => default;
    DuckTyped<T> GetAsync<T>() => default;
}

[AsyncMethodBuilder(null)]
class DuckTyped
{
    public Awaiter GetAwaiter() => default(Awaiter);
}
[AsyncMethodBuilder(null)]
class DuckTyped<T>
{
    public Awaiter<T> GetAwaiter() => default(Awaiter<T>);
}
public struct Awaiter : INotifyCompletion
{
    public bool IsCompleted => true;
    public void OnCompleted(Action continuation) { }
    public void GetResult() { }
}
public struct Awaiter<T> : INotifyCompletion
{
    public bool IsCompleted => true;
    public void OnCompleted(Action continuation) { }
    public T GetResult() => default(T);
}
", @"
using System;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

class C
{
    async DuckTyped M()
    {
        using (default(IDisposable))
        {
            await GetAsync();
        }
    }

    async DuckTyped<int> M2()
    {
        using (default(IDisposable))
        {
            return await GetAsync<int>();
        }
    }

    DuckTyped GetAsync() => default;
    DuckTyped<T> GetAsync<T>() => default;
}

[AsyncMethodBuilder(null)]
class DuckTyped
{
    public Awaiter GetAwaiter() => default(Awaiter);
}
[AsyncMethodBuilder(null)]
class DuckTyped<T>
{
    public Awaiter<T> GetAwaiter() => default(Awaiter<T>);
}
public struct Awaiter : INotifyCompletion
{
    public bool IsCompleted => true;
    public void OnCompleted(Action continuation) { }
    public void GetResult() { }
}
public struct Awaiter<T> : INotifyCompletion
{
    public bool IsCompleted => true;
    public void OnCompleted(Action continuation) { }
    public T GetResult() => default(T);
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseAsyncAwait)]
    public async Task TestNoDiagnostic_UsingLocalDeclaration()
    {
        await VerifyNoDiagnosticAsync(@"
using System;
using System.Threading.Tasks;

public class C
{
    public Task<string> M()
    {
        {
            {
                using var disposable = default(IDisposable);
            }

            return GetAsync();
        }
    }

    Task<string> GetAsync() => Task.FromResult(default(string));
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseAsyncAwait)]
    public async Task TestNoDiagnostic_TaskCompletedTask()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Threading;
using System.Threading.Tasks;

class C
{
    Task M(CancellationToken cancellationToken)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        return Task.CompletedTask;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseAsyncAwait)]
    public async Task TestNoDiagnostic_TaskFromCanceled()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Threading;
using System.Threading.Tasks;

class C
{
    Task M(CancellationToken cancellationToken)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        return Task.FromCanceled(cancellationToken);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseAsyncAwait)]
    public async Task TestNoDiagnostic_TaskFromException()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Threading;
using System.Threading.Tasks;

class C
{
    Task M(CancellationToken cancellationToken)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        return Task.FromException(default(System.Exception));
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseAsyncAwait)]
    public async Task TestNoDiagnostic_TaskOfTFromResult()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Threading;
using System.Threading.Tasks;

class C
{
    Task<int> M(CancellationToken cancellationToken)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        return Task.FromResult(1);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseAsyncAwait)]
    public async Task TestNoDiagnostic_TaskOfTFromCanceled()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Threading;
using System.Threading.Tasks;

class C
{
    Task<int> M(CancellationToken cancellationToken)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        return Task.FromCanceled<int>(cancellationToken);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseAsyncAwait)]
    public async Task TestNoDiagnostic_TaskOfTFromException()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Threading;
using System.Threading.Tasks;

class C
{
    Task<int> M(CancellationToken cancellationToken)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        return Task.FromException<int>(default(System.Exception));
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseAsyncAwait)]
    public async Task TestNoDiagnostic_IAsyncEnumerable()
    {
        await VerifyNoDiagnosticAsync(@"
using System;
using System.Collections.Generic;

class C
{
    public IAsyncEnumerable<string> GetAsync()
    {
        using var disposable = new System.IO.StringWriter();

        IAsyncEnumerable<string> enumerable = GetAsync2(disposable);

        return enumerable;
    }

    async IAsyncEnumerable<string> GetAsync2(IDisposable disposable)
    {
        await System.Threading.Tasks.Task.CompletedTask;
        yield break;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseAsyncAwait)]
    public async Task TestNoDiagnostic_DuckTyped_NotTaskType()
    {
        await VerifyNoDiagnosticAsync(@"
using System;
using System.Runtime.CompilerServices;

class C
{
    DuckTyped M()
    {
        using (default(IDisposable))
        {
            return GetAsync();
        }
    }

    DuckTyped<int> M2()
    {
        using (default(IDisposable))
        {
            return GetAsync<int>();
        }
    }

    DuckTyped GetAsync() => default;
    DuckTyped<T> GetAsync<T>() => default;
}

//[AsyncMethodBuilder(null)]
class DuckTyped
{
    public Awaiter GetAwaiter() => default(Awaiter);
}
//[AsyncMethodBuilder(null)]
class DuckTyped<T>
{
    public Awaiter<T> GetAwaiter() => default(Awaiter<T>);
}

public struct Awaiter : INotifyCompletion
{
    public bool IsCompleted => true;
    public void OnCompleted(Action continuation) { }
    public void GetResult() { }
}
public struct Awaiter<T> : INotifyCompletion
{
    public bool IsCompleted => true;
    public void OnCompleted(Action continuation) { }
    public T GetResult() => default(T);
}
");
    }
    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseAsyncAwait)]
    public async Task TestNoDiagnostic_NonAwaitable_TaskType()
    {
        await VerifyNoDiagnosticAsync(@"
using System;
using System.Runtime.CompilerServices;

class C
{
    NonAwaitableTaskType M()
    {
        using (default(IDisposable))
        {
            return GetAsync();
        }
    }

    NonAwaitableTaskType<int> M2()
    {
        using (default(IDisposable))
        {
            return GetAsync<int>();
        }
    }

    NonAwaitableTaskType GetAsync() => default;
    NonAwaitableTaskType<T> GetAsync<T>() => default;
}

[AsyncMethodBuilder(null)]
class NonAwaitableTaskType { }
[AsyncMethodBuilder(null)]
class NonAwaitableTaskType<T> { }
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseAsyncAwait)]
    public async Task TestNoDiagnostic_ValueTask()
    {
        await VerifyNoDiagnosticAsync(@"
using System;
using System.IO;
using System.Threading.Tasks;

class C
{
    ValueTask M()
    {
        using var memoryStream = new MemoryStream();

        return ValueTask.CompletedTask;
    }
}
");
    }
}
