// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;
using Roslynator.CSharp.Analysis.ReturnTaskInsteadOfNull;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1210ReturnTaskInsteadOfNullTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.ReturnTaskInsteadOfNull;

        public override DiagnosticAnalyzer Analyzer { get; } = new ReturnTaskInsteadOfNullAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new ReturnTaskInsteadOfNullCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ReturnTaskInsteadOfNull)]
        public async Task Test_Body()
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

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ReturnTaskInsteadOfNull)]
        public async Task Test_ExpressionBody()
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
