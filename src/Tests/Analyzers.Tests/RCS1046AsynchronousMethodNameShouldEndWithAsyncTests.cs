// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1046AsynchronousMethodNameShouldEndWithAsyncTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.AsynchronousMethodNameShouldEndWithAsync;

        public override DiagnosticAnalyzer Analyzer { get; } = new AsynchronousMethodNameShouldEndWithAsyncAnalyzer();

        public override CodeFixProvider FixProvider { get; }

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
}");
        }
    }
}
