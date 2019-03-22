// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1229UseAsyncAwaitTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UseAsyncAwait;

        public override DiagnosticAnalyzer Analyzer { get; } = new UseAsyncAwaitAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new UseAsyncAwaitCodeFixProvider();

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
            return;
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
    }
}
