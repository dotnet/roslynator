// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1090RemoveCallToConfigureAwaitTests : AbstractCSharpDiagnosticVerifier<ConfigureAwaitAnalyzer, AwaitExpressionCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.ConfigureAwait;

    public override CSharpTestOptions Options
    {
        get { return base.Options.AddConfigOption(ConfigOptionKeys.ConfigureAwait, "false"); }
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConfigureAwait)]
    public async Task Test_Task_Field()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Threading.Tasks;

class C
{
    Task _task;

    async Task M()
    {
        await _task[|.ConfigureAwait(false)|];
    }
}
", @"
using System.Threading.Tasks;

class C
{
    Task _task;

    async Task M()
    {
        await _task;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConfigureAwait)]
    public async Task Test_Task_Local()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Threading.Tasks;

class C
{
    async Task M()
    {
        Task task = default;
        await task[|.ConfigureAwait(false)|];
    }
}
", @"
using System.Threading.Tasks;

class C
{
    async Task M()
    {
        Task task = default;
        await task;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConfigureAwait)]
    public async Task Test_Task_Method()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Threading.Tasks;

class C
{
    async Task M()
    {
        await M()[|.ConfigureAwait(false)|];
    }
}
", @"
using System.Threading.Tasks;

class C
{
    async Task M()
    {
        await M();
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConfigureAwait)]
    public async Task Test_Task_Parameter()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Threading.Tasks;

class C
{
    async Task M(Task task)
    {
        await task[|.ConfigureAwait(false)|];
    }
}
", @"
using System.Threading.Tasks;

class C
{
    async Task M(Task task)
    {
        await task;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConfigureAwait)]
    public async Task Test_Task_Property()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Threading.Tasks;

class C
{
    Task P { get; }

    async Task M()
    {
        await P[|.ConfigureAwait(false)|];
    }
}
", @"
using System.Threading.Tasks;

class C
{
    Task P { get; }

    async Task M()
    {
        await P;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConfigureAwait)]
    public async Task Test_TaskOfT()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Threading.Tasks;

class C
{
    async Task<object> M()
    {
        return await M()[|.ConfigureAwait(false)|];
    }
}
", @"
using System.Threading.Tasks;

class C
{
    async Task<object> M()
    {
        return await M();
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConfigureAwait)]
    public async Task Test_ValueTask()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Threading.Tasks;

class C
{
    async Task M()
    {
        await M2()[|.ConfigureAwait(false)|];
    }

    ValueTask M2() => default;
}
", @"
using System.Threading.Tasks;

class C
{
    async Task M()
    {
        await M2();
    }

    ValueTask M2() => default;
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConfigureAwait)]
    public async Task Test_ValueTaskOfT()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Threading.Tasks;

class C
{
    async Task<object> M()
    {
        var result = await M2()[|.ConfigureAwait(false)|];
        return Task.FromResult(default(object));
    }

    ValueTask<object> M2() => default;
}
", @"
using System.Threading.Tasks;

class C
{
    async Task<object> M()
    {
        var result = await M2();
        return Task.FromResult(default(object));
    }

    ValueTask<object> M2() => default;
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConfigureAwait)]
    public async Task Test_DuckTyped()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

class C
{
    async Task M()
    {
        await M2()[|.ConfigureAwait(false)|];
    }
    DuckTyped M2() => default(DuckTyped);
}

class DuckTyped
{
    public Awaiter GetAwaiter() => default(Awaiter);

    public struct Awaiter : INotifyCompletion
    {
        public bool IsCompleted => false;
        public void OnCompleted(System.Action continuation) { }
        public void GetResult() { }
    }

    public ConfiguredDuckTyped ConfigureAwait(bool continueOnCapturedContext)
    {
        return default(ConfiguredDuckTyped);
    }
    public struct ConfiguredDuckTyped
    {
        public ConfiguredDuckAwaiter GetAwaiter() => default(ConfiguredDuckAwaiter);

        public struct ConfiguredDuckAwaiter : INotifyCompletion
        {
            public bool IsCompleted => false;
            public void OnCompleted(System.Action continuation) { }
            public void GetResult() { }
        }
    }
}
", @"
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

class C
{
    async Task M()
    {
        await M2();
    }
    DuckTyped M2() => default(DuckTyped);
}

class DuckTyped
{
    public Awaiter GetAwaiter() => default(Awaiter);

    public struct Awaiter : INotifyCompletion
    {
        public bool IsCompleted => false;
        public void OnCompleted(System.Action continuation) { }
        public void GetResult() { }
    }

    public ConfiguredDuckTyped ConfigureAwait(bool continueOnCapturedContext)
    {
        return default(ConfiguredDuckTyped);
    }
    public struct ConfiguredDuckTyped
    {
        public ConfiguredDuckAwaiter GetAwaiter() => default(ConfiguredDuckAwaiter);

        public struct ConfiguredDuckAwaiter : INotifyCompletion
        {
            public bool IsCompleted => false;
            public void OnCompleted(System.Action continuation) { }
            public void GetResult() { }
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConfigureAwait)]
    public async Task Test_ExtensionMethod()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

class C
{
    async Task M()
    {
        await Task.Yield()[|.ConfigureAwait(false)|];
    }
}

static class E
{
    public static ConfiguredYieldAwaitable ConfigureAwait(this YieldAwaitable yieldAwaitable, bool continueOnCapturedContext)
    {
        return default(ConfiguredYieldAwaitable);
    }
}

struct ConfiguredYieldAwaitable
{
    public ConfiguredYieldAwaitable(YieldAwaitable yieldAwaitable, bool continueOnCapturedContext)
    {
    }

    public Awaiter GetAwaiter() => default(Awaiter);

    public struct Awaiter : INotifyCompletion
    {
        public bool IsCompleted => false;
        public void OnCompleted(System.Action continuation) { }
        public void GetResult() { }
    }
}
", @"
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

class C
{
    async Task M()
    {
        await Task.Yield();
    }
}

static class E
{
    public static ConfiguredYieldAwaitable ConfigureAwait(this YieldAwaitable yieldAwaitable, bool continueOnCapturedContext)
    {
        return default(ConfiguredYieldAwaitable);
    }
}

struct ConfiguredYieldAwaitable
{
    public ConfiguredYieldAwaitable(YieldAwaitable yieldAwaitable, bool continueOnCapturedContext)
    {
    }

    public Awaiter GetAwaiter() => default(Awaiter);

    public struct Awaiter : INotifyCompletion
    {
        public bool IsCompleted => false;
        public void OnCompleted(System.Action continuation) { }
        public void GetResult() { }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConfigureAwait)]
    public async Task TestNoDiagnostic_Task()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Threading.Tasks;

class C
{
    async Task M()
    {
        await M();
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConfigureAwait)]
    public async Task TestNoDiagnostic_TaskOfT()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Threading.Tasks;

class C
{
    async Task<object> M()
    {
        return await M();
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConfigureAwait)]
    public async Task TestNoDiagnostic_ValueTaskOfT()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Threading.Tasks;

class C
{
    async Task M()
    {
        await M2();
    }

    ValueTask<object> M2() => default;
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConfigureAwait)]
    public async Task TestNoDiagnostic_ValueTask()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Threading.Tasks;

class C
{
    async Task M()
    {
        await M2();
    }

    ValueTask<object> M2() => default;
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConfigureAwait)]
    public async Task TestNoDiagnostic_ExtensionMethod()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

class C
{
    async Task M()
    {
        await Task.Yield();
    }
}

static class E
{
    public static ConfiguredYieldAwaitable ConfigureAwait(this YieldAwaitable yieldAwaitable, bool continueOnCapturedContext)
    {
        return default(ConfiguredYieldAwaitable);
    }
}

struct ConfiguredYieldAwaitable
{
    public ConfiguredYieldAwaitable(YieldAwaitable yieldAwaitable, bool continueOnCapturedContext)
    {
    }

    public Awaiter GetAwaiter() => default(Awaiter);

    public struct Awaiter : INotifyCompletion
    {
        public bool IsCompleted => false;
        public void OnCompleted(System.Action continuation) { }
        public void GetResult() { }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConfigureAwait)]
    public async Task TestNoDiagnostic_NonAwaitable_Lookalike()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

class C
{
    async Task M()
    {
        await M2().ConfigureAwait(false);
    }
    NonAwaitable M2() => default;
}

struct Awaitable
{
    public Awaiter GetAwaiter() => default(Awaiter);

    public struct Awaiter : INotifyCompletion
    {
        public bool IsCompleted => false;
        public void OnCompleted(System.Action continuation) { }
        public void GetResult() { }
    }
}

struct NonAwaitable
{
    // no awaiter

    public Awaitable ConfigureAwait(bool continueOnCapturedContext)
    {
        return default(Awaitable);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConfigureAwait)]
    public async Task TestNoDiagnostic_ConfigureAwaitTrue()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Threading.Tasks;

class C
{
    async Task M()
    {
        Task task = default;
        await task.ConfigureAwait(true);
    }
}
");
    }
}
