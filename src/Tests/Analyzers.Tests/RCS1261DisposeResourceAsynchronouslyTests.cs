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
    public async Task Test_Method_WithAsync()
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

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DisposeResourceAsynchronously)]
    public async Task Test_Method_WithoutAsync_Task()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Threading.Tasks;

class C
{
    Task FooAsync()
    {
        [|using var disposable = GetDisposable();|]

        return Task.CompletedTask;
    }

    private Disposable GetDisposable()
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
        await using var disposable = GetDisposable();

        await Task.CompletedTask;
    }

    private Disposable GetDisposable()
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

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DisposeResourceAsynchronously)]
    public async Task Test_Method_WithoutAsync_TaskOfT()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Threading.Tasks;

class C
{
    Task<string> FooAsync()
    {
        [|using var disposable = GetDisposable();|]

        return Task.FromResult("""");
    }

    private Disposable GetDisposable()
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
    async Task<string> FooAsync()
    {
        await using var disposable = GetDisposable();

        return await Task.FromResult("""");
    }

    private Disposable GetDisposable()
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

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DisposeResourceAsynchronously)]
    public async Task Test_LocalFunction_WithAsync()
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
            [|using var disposable = await GetDisposableAsync();|]
        }
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
    void M()
    {
        async Task FooAsync()
        {
            await using var disposable = await GetDisposableAsync();
        }
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

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DisposeResourceAsynchronously)]
    public async Task Test_LocalFunction_WithoutAsync_Task()
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
            [|using var disposable = GetDisposable();|]

            return Task.CompletedTask;
        }
    }

    private Disposable GetDisposable()
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
    void M()
    {
        async Task FooAsync()
        {
            await using var disposable = GetDisposable();

            await Task.CompletedTask;
        }
    }

    private Disposable GetDisposable()
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

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DisposeResourceAsynchronously)]
    public async Task Test_LocalFunction_WithoutAsync_TaskOfT()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Threading.Tasks;

class C
{
    void M()
    {
        Task<string> FooAsync()
        {
            [|using var disposable = GetDisposable();|]

            return Task.FromResult("""");
        }
    }

    private Disposable GetDisposable()
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
    void M()
    {
        async Task<string> FooAsync()
        {
            await using var disposable = GetDisposable();

            return await Task.FromResult("""");
        }
    }

    private Disposable GetDisposable()
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
