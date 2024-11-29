// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1261DisposeResourceAsynchronouslyTests : AbstractCSharpDiagnosticVerifier<DisposeResourceAsynchronouslyAnalyzer, DisposeResourceAsynchronouslyCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.DisposeResourceAsynchronously;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DisposeResourceAsynchronously)]
    public async Task Test_Method_LocalStatement_WithAsync()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Threading.Tasks;

class C
{
    async Task FooAsync()
    {
        string s = null;

        [|using|] var disposable = await GetDisposableAsync();
    }

    private Task<Disposable> GetDisposableAsync() => throw new NotImplementedException();
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
        string s = null;

        await using var disposable = await GetDisposableAsync();
    }

    private Task<Disposable> GetDisposableAsync() => throw new NotImplementedException();
}

internal class Disposable : IDisposable, IAsyncDisposable
{
    public void Dispose() => throw new NotImplementedException();
    public ValueTask DisposeAsync() => throw new NotImplementedException();
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DisposeResourceAsynchronously)]
    public async Task Test_Method_LocalStatement_WithoutAsync_Task()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Threading.Tasks;

class C
{
    Task FooAsync()
    {
        [|using|] var disposable = GetDisposable();

        return Task.CompletedTask;
    }

    private Disposable GetDisposable() => throw new NotImplementedException();
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
        await using var disposable = GetDisposable();

        await Task.CompletedTask;
    }

    private Disposable GetDisposable() => throw new NotImplementedException();
}

internal class Disposable : IDisposable, IAsyncDisposable
{
    public void Dispose() => throw new NotImplementedException();
    public ValueTask DisposeAsync() => throw new NotImplementedException();
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DisposeResourceAsynchronously)]
    public async Task Test_Method_LocalStatement_WithoutAsync_TaskOfT()
    {
        await VerifyDiagnosticAndFixAsync("""
using System;
using System.Threading.Tasks;

class C
{
    Task<string> FooAsync()
    {
        [|using|] var disposable = GetDisposable();

        return Task.FromResult("");
    }

    private Disposable GetDisposable() => throw new NotImplementedException();
}

internal class Disposable : IDisposable, IAsyncDisposable
{
    public void Dispose() => throw new NotImplementedException();
    public ValueTask DisposeAsync() => throw new NotImplementedException();
}
""", """
using System;
using System.Threading.Tasks;

class C
{
    async Task<string> FooAsync()
    {
        await using var disposable = GetDisposable();

        return await Task.FromResult("");
    }

    private Disposable GetDisposable() => throw new NotImplementedException();
}

internal class Disposable : IDisposable, IAsyncDisposable
{
    public void Dispose() => throw new NotImplementedException();
    public ValueTask DisposeAsync() => throw new NotImplementedException();
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DisposeResourceAsynchronously)]
    public async Task Test_LocalFunction_LocalStatement_WithAsync()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Threading.Tasks;

class C
{
    void M()
    {
        async Task FooAsync()
        {
            [|using|] var disposable = await GetDisposableAsync();
        }
    }

    private Task<Disposable> GetDisposableAsync() => throw new NotImplementedException();
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
    void M()
    {
        async Task FooAsync()
        {
            await using var disposable = await GetDisposableAsync();
        }
    }

    private Task<Disposable> GetDisposableAsync() => throw new NotImplementedException();
}

internal class Disposable : IDisposable, IAsyncDisposable
{
    public void Dispose() => throw new NotImplementedException();
    public ValueTask DisposeAsync() => throw new NotImplementedException();
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DisposeResourceAsynchronously)]
    public async Task Test_LocalFunction_LocalStatement_WithoutAsync_Task()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Threading.Tasks;

class C
{
    void M()
    {
        Task FooAsync()
        {
            [|using|] var disposable = GetDisposable();

            return Task.CompletedTask;
        }
    }

    private Disposable GetDisposable() => throw new NotImplementedException();
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
    void M()
    {
        async Task FooAsync()
        {
            await using var disposable = GetDisposable();

            await Task.CompletedTask;
        }
    }

    private Disposable GetDisposable() => throw new NotImplementedException();
}

internal class Disposable : IDisposable, IAsyncDisposable
{
    public void Dispose() => throw new NotImplementedException();
    public ValueTask DisposeAsync() => throw new NotImplementedException();
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DisposeResourceAsynchronously)]
    public async Task Test_LocalFunction_LocalStatement_WithoutAsync_TaskOfT()
    {
        await VerifyDiagnosticAndFixAsync("""
using System;
using System.Threading.Tasks;

class C
{
    void M()
    {
        Task<string> FooAsync()
        {
            [|using|] var disposable = GetDisposable();

            return Task.FromResult("");
        }
    }

    private Disposable GetDisposable() => throw new NotImplementedException();
}

internal class Disposable : IDisposable, IAsyncDisposable
{
    public void Dispose() => throw new NotImplementedException();
    public ValueTask DisposeAsync() => throw new NotImplementedException();
}
""", """
using System;
using System.Threading.Tasks;

class C
{
    void M()
    {
        async Task<string> FooAsync()
        {
            await using var disposable = GetDisposable();

            return await Task.FromResult("");
        }
    }

    private Disposable GetDisposable() => throw new NotImplementedException();
}

internal class Disposable : IDisposable, IAsyncDisposable
{
    public void Dispose() => throw new NotImplementedException();
    public ValueTask DisposeAsync() => throw new NotImplementedException();
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DisposeResourceAsynchronously)]
    public async Task Test_Method_UsingStatement_WithAsync()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Threading.Tasks;

class C
{
    async Task FooAsync()
    {
        string s = null;

        [|using|] (var disposable = await GetDisposableAsync()) { }
    }

    private Task<Disposable> GetDisposableAsync() => throw new NotImplementedException();
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
        string s = null;

        await using (var disposable = await GetDisposableAsync()) { }
    }

    private Task<Disposable> GetDisposableAsync() => throw new NotImplementedException();
}

internal class Disposable : IDisposable, IAsyncDisposable
{
    public void Dispose() => throw new NotImplementedException();
    public ValueTask DisposeAsync() => throw new NotImplementedException();
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DisposeResourceAsynchronously)]
    public async Task Test_Method_UsingStatement_WithoutAsync_Task()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Threading.Tasks;

class C
{
    Task FooAsync()
    {
        [|using|] (var disposable = GetDisposable()) { }

        return Task.CompletedTask;
    }

    private Disposable GetDisposable() => throw new NotImplementedException();
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
        await using (var disposable = GetDisposable()) { }

        await Task.CompletedTask;
    }

    private Disposable GetDisposable() => throw new NotImplementedException();
}

internal class Disposable : IDisposable, IAsyncDisposable
{
    public void Dispose() => throw new NotImplementedException();
    public ValueTask DisposeAsync() => throw new NotImplementedException();
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DisposeResourceAsynchronously)]
    public async Task Test_Method_UsingStatement_WithoutAsync_TaskOfT()
    {
        await VerifyDiagnosticAndFixAsync("""
using System;
using System.Threading.Tasks;

class C
{
    Task<string> FooAsync()
    {
        [|using|] (var disposable = GetDisposable()) { }

        return Task.FromResult("");
    }

    private Disposable GetDisposable() => throw new NotImplementedException();
}

internal class Disposable : IDisposable, IAsyncDisposable
{
    public void Dispose() => throw new NotImplementedException();
    public ValueTask DisposeAsync() => throw new NotImplementedException();
}
""", """
using System;
using System.Threading.Tasks;

class C
{
    async Task<string> FooAsync()
    {
        await using (var disposable = GetDisposable()) { }

        return await Task.FromResult("");
    }

    private Disposable GetDisposable() => throw new NotImplementedException();
}

internal class Disposable : IDisposable, IAsyncDisposable
{
    public void Dispose() => throw new NotImplementedException();
    public ValueTask DisposeAsync() => throw new NotImplementedException();
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DisposeResourceAsynchronously)]
    public async Task Test_LocalFunction_UsingStatement_WithAsync()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Threading.Tasks;

class C
{
    void M()
    {
        async Task FooAsync()
        {
            [|using|] (var disposable = await GetDisposableAsync()) { }
        }
    }

    private Task<Disposable> GetDisposableAsync() => throw new NotImplementedException();
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
    void M()
    {
        async Task FooAsync()
        {
            await using (var disposable = await GetDisposableAsync()) { }
        }
    }

    private Task<Disposable> GetDisposableAsync() => throw new NotImplementedException();
}

internal class Disposable : IDisposable, IAsyncDisposable
{
    public void Dispose() => throw new NotImplementedException();
    public ValueTask DisposeAsync() => throw new NotImplementedException();
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DisposeResourceAsynchronously)]
    public async Task Test_LocalFunction_UsingStatement_WithoutAsync_Task()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Threading.Tasks;

class C
{
    void M()
    {
        Task FooAsync()
        {
            [|using|] (var disposable = GetDisposable()) { }

            return Task.CompletedTask;
        }
    }

    private Disposable GetDisposable() => throw new NotImplementedException();
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
    void M()
    {
        async Task FooAsync()
        {
            await using (var disposable = GetDisposable()) { }

            await Task.CompletedTask;
        }
    }

    private Disposable GetDisposable() => throw new NotImplementedException();
}

internal class Disposable : IDisposable, IAsyncDisposable
{
    public void Dispose() => throw new NotImplementedException();
    public ValueTask DisposeAsync() => throw new NotImplementedException();
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DisposeResourceAsynchronously)]
    public async Task Test_LocalFunction_UsingStatement_WithoutAsync_TaskOfT()
    {
        await VerifyDiagnosticAndFixAsync("""
using System;
using System.Threading.Tasks;

class C
{
    void M()
    {
        Task<string> FooAsync()
        {
            [|using|] (var disposable = GetDisposable()) { }

            return Task.FromResult("");
        }
    }

    private Disposable GetDisposable() => throw new NotImplementedException();
}

internal class Disposable : IDisposable, IAsyncDisposable
{
    public void Dispose() => throw new NotImplementedException();
    public ValueTask DisposeAsync() => throw new NotImplementedException();
}
""", """
using System;
using System.Threading.Tasks;

class C
{
    void M()
    {
        async Task<string> FooAsync()
        {
            await using (var disposable = GetDisposable()) { }

            return await Task.FromResult("");
        }
    }

    private Disposable GetDisposable() => throw new NotImplementedException();
}

internal class Disposable : IDisposable, IAsyncDisposable
{
    public void Dispose() => throw new NotImplementedException();
    public ValueTask DisposeAsync() => throw new NotImplementedException();
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DisposeResourceAsynchronously)]
    public async Task Test_SimpleLambda_LocalStatement_WithAsync()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Threading.Tasks;

class C
{
    void Foo()
    {
        Func<string, Task> action = async f =>
        {
            [|using|] var disposable = await GetDisposableAsync();
        };
    }

    private Task<Disposable> GetDisposableAsync() => throw new NotImplementedException();
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
    void Foo()
    {
        Func<string, Task> action = async f =>
        {
            await using var disposable = await GetDisposableAsync();
        };
    }

    private Task<Disposable> GetDisposableAsync() => throw new NotImplementedException();
}

internal class Disposable : IDisposable, IAsyncDisposable
{
    public void Dispose() => throw new NotImplementedException();
    public ValueTask DisposeAsync() => throw new NotImplementedException();
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DisposeResourceAsynchronously)]
    public async Task Test_SimpleLambda_LocalStatement_WithoutAsync()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Threading.Tasks;

class C
{
    void Foo()
    {
        Func<string, Task> action = f =>
        {
            [|using|] var disposable = GetDisposable();
            return Task.CompletedTask;
        };
    }

    private Disposable GetDisposable() => throw new NotImplementedException();
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
    void Foo()
    {
        Func<string, Task> action = async f =>
        {
            await using var disposable = GetDisposable();
            await Task.CompletedTask;
        };
    }

    private Disposable GetDisposable() => throw new NotImplementedException();
}

internal class Disposable : IDisposable, IAsyncDisposable
{
    public void Dispose() => throw new NotImplementedException();
    public ValueTask DisposeAsync() => throw new NotImplementedException();
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DisposeResourceAsynchronously)]
    public async Task Test_ParenthesizedLambda_LocalStatement_WithAsync()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Threading.Tasks;

class C
{
    void Foo()
    {
        var action = async () =>
        {
            [|using|] var disposable = await GetDisposableAsync();
        };
    }

    private Task<Disposable> GetDisposableAsync() => throw new NotImplementedException();
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
    void Foo()
    {
        var action = async () =>
        {
            await using var disposable = await GetDisposableAsync();
        };
    }

    private Task<Disposable> GetDisposableAsync() => throw new NotImplementedException();
}

internal class Disposable : IDisposable, IAsyncDisposable
{
    public void Dispose() => throw new NotImplementedException();
    public ValueTask DisposeAsync() => throw new NotImplementedException();
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DisposeResourceAsynchronously)]
    public async Task Test_ParenthesizedLambda_LocalStatement_WithoutAsync()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Threading.Tasks;

class C
{
    void Foo()
    {
        Func<string, Task> action = (f) =>
        {
            [|using|] var disposable = GetDisposable();
            return Task.CompletedTask;
        };
    }

    private Disposable GetDisposable() => throw new NotImplementedException();
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
    void Foo()
    {
        Func<string, Task> action = async (f) =>
        {
            await using var disposable = GetDisposable();
            await Task.CompletedTask;
        };
    }

    private Disposable GetDisposable() => throw new NotImplementedException();
}

internal class Disposable : IDisposable, IAsyncDisposable
{
    public void Dispose() => throw new NotImplementedException();
    public ValueTask DisposeAsync() => throw new NotImplementedException();
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DisposeResourceAsynchronously)]
    public async Task Test_AnonymousMethod_LocalStatement_WithAsync()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Threading.Tasks;

class C
{
    void Foo()
    {
        Func<string, Task> action = async delegate (string f)
        {
            [|using|] var disposable = await GetDisposableAsync();
        };
    }

    private Task<Disposable> GetDisposableAsync() => throw new NotImplementedException();
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
    void Foo()
    {
        Func<string, Task> action = async delegate (string f)
        {
            await using var disposable = await GetDisposableAsync();
        };
    }

    private Task<Disposable> GetDisposableAsync() => throw new NotImplementedException();
}

internal class Disposable : IDisposable, IAsyncDisposable
{
    public void Dispose() => throw new NotImplementedException();
    public ValueTask DisposeAsync() => throw new NotImplementedException();
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DisposeResourceAsynchronously)]
    public async Task Test_AnonymousMethod_LocalStatement_WithoutAsync()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Threading.Tasks;

class C
{
    void Foo()
    {
        Func<string, Task> action = delegate (string f)
        {
            [|using|] var disposable = GetDisposable();
            return Task.CompletedTask;
        };
    }

    private Disposable GetDisposable() => throw new NotImplementedException();
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
    void Foo()
    {
        Func<string, Task> action = async delegate (string f)
        {
            await using var disposable = GetDisposable();
            await Task.CompletedTask;
        };
    }

    private Disposable GetDisposable() => throw new NotImplementedException();
}

internal class Disposable : IDisposable, IAsyncDisposable
{
    public void Dispose() => throw new NotImplementedException();
    public ValueTask DisposeAsync() => throw new NotImplementedException();
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DisposeResourceAsynchronously)]
    public async Task Test_DuckTyped_TaskType_WithAsync()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.IO;
using System.Runtime.CompilerServices;

class C
{
    async DuckTyped M()
    {
        [|using|] FileStream fs = default;
        await GetAsync();
    }

    async DuckTyped<int> M2()
    {
        [|using|] FileStream fs = default;
        return await GetAsync<int>();
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
using System.IO;
using System.Runtime.CompilerServices;

class C
{
    async DuckTyped M()
    {
        await using FileStream fs = default;
        await GetAsync();
    }

    async DuckTyped<int> M2()
    {
        await using FileStream fs = default;
        return await GetAsync<int>();
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

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DisposeResourceAsynchronously)]
    public async Task Test_DuckTyped_TaskType_WithoutAsync()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.IO;
using System.Runtime.CompilerServices;

class C
{
    DuckTyped M()
    {
        [|using|] FileStream fs = default;
        return GetAsync();
    }

    DuckTyped<int> M2()
    {
        [|using|] FileStream fs = default;
        return GetAsync<int>();
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
using System.IO;
using System.Runtime.CompilerServices;

class C
{
    async DuckTyped M()
    {
        await using FileStream fs = default;
        await GetAsync();
    }

    async DuckTyped<int> M2()
    {
        await using FileStream fs = default;
        return await GetAsync<int>();
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

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DisposeResourceAsynchronously)]
    public async Task TestNoDiagnostic_LockStatement()
    {
        await VerifyNoDiagnosticAsync("""
using System.IO;
using System.Threading.Tasks;

abstract class C
{
    object _lock = new();

    async Task Foo()
    {
        lock (_lock)
        {
            using FileStream fs = new("test.txt", FileMode.OpenOrCreate);
        }

        await Task.Yield();
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DisposeResourceAsynchronously)]
    public async Task TestNoDiagnostic_DuckTyped_NotTaskType()
    {
        await VerifyNoDiagnosticAsync(@"
using System;
using System.IO;
using System.Runtime.CompilerServices;

class C
{
    DuckTyped M()
    {
        using FileStream fs = default;
        return GetAsync();
    }

    DuckTyped<int> M2()
    {
        using FileStream fs = default;
        return GetAsync<int>();
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

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DisposeResourceAsynchronously)]
    public async Task TestNoDiagnostic_NonAwaitable_TaskType()
    {
        await VerifyNoDiagnosticAsync(@"
using System;
using System.IO;
using System.Runtime.CompilerServices;

class C
{
    NonAwaitableTaskType M()
    {
        using FileStream fs = default;
        return GetAsync();
    }

    NonAwaitableTaskType<int> M2()
    {
        using FileStream fs = default;
        return GetAsync<int>();
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
}
