// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Analysis.ReturnTaskInsteadOfNull;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1210ReturnCompletedTaskInsteadOfNullTests2 : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.ReturnCompletedTaskInsteadOfNull;

        public override DiagnosticAnalyzer Analyzer { get; } = new ReturnCompletedTaskInsteadOfNullAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new ReturnCompletedTaskInsteadOfNullCodeFixProvider2();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ReturnCompletedTaskInsteadOfNull)]
        public async Task Test_TaskOfT_ConditionalAccess()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System.Threading.Tasks;

class C
{
    Task<C> GetAsync(C c)
    {
        return [|c?.GetAsync()|];
    }

    Task<C> GetAsync() => Task.FromResult(default(C));
}
", @"
using System.Threading.Tasks;

class C
{
    Task<C> GetAsync(C c)
    {
        C x = c;
        if (x != null)
        {
            return x.GetAsync();
        }
        else
        {
            return Task.FromResult<C>(null);
        }
    }

    Task<C> GetAsync() => Task.FromResult(default(C));
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ReturnCompletedTaskInsteadOfNull)]
        public async Task Test_Task_ConditionalAccess()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System.Threading.Tasks;

class C
{
    Task GetAsync(C c)
    {
        return [|c?.GetAsync()|];
    }

    Task GetAsync() => Task.CompletedTask;
}
", @"
using System.Threading.Tasks;

class C
{
    Task GetAsync(C c)
    {
        C x = c;
        if (x != null)
        {
            return x.GetAsync();
        }
        else
        {
            return Task.CompletedTask;
        }
    }

    Task GetAsync() => Task.CompletedTask;
}
");
        }
    }
}
