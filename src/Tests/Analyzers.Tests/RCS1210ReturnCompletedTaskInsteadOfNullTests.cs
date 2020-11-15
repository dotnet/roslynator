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
    public class RCS1210ReturnCompletedTaskInsteadOfNullTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.ReturnCompletedTaskInsteadOfNull;

        public override DiagnosticAnalyzer Analyzer { get; } = new ReturnCompletedTaskInsteadOfNullAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new ReturnCompletedTaskInsteadOfNullCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ReturnCompletedTaskInsteadOfNull)]
        public async Task Test_TaskOfT_Body()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System.Threading.Tasks;

class C
{
    Task<object> GetAsync()
    {
        return [|null|];
    }
}
", @"
using System.Threading.Tasks;

class C
{
    Task<object> GetAsync()
    {
        return Task.FromResult<object>(null);
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ReturnCompletedTaskInsteadOfNull)]
        public async Task Test_Task_Body()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System.Threading.Tasks;

class C
{
    Task GetAsync()
    {
        return [|null|];
    }
}
", @"
using System.Threading.Tasks;

class C
{
    Task GetAsync()
    {
        return Task.CompletedTask;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ReturnCompletedTaskInsteadOfNull)]
        public async Task Test_TaskOfT_ExpressionBody()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System.Threading.Tasks;

class C
{
    Task<bool> GetAsync() => [|null|];
}
", @"
using System.Threading.Tasks;

class C
{
    Task<bool> GetAsync() => Task.FromResult(false);
}
");
        }
    }
}
