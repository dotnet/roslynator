// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1041RemoveEmptyInitializerTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.RemoveEmptyInitializer;

        public override DiagnosticAnalyzer Analyzer { get; } = new RemoveEmptyInitializerAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new RemoveEmptyInitializerCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveEmptyInitializer)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        var x = new C [|{ }|];
    }
}
", @"
class C
{
    void M()
    {
        var x = new C();
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveEmptyInitializer)]
        public async Task TestNoDiagnostic_ExpressionTree()
        {
            await VerifyNoDiagnosticAsync(@"
using System;
using System.Linq.Expressions;

class C
{
    public void M<T>(Expression<Func<T>> e)
    {
        M(() => new C { });
    }
}
");
        }
    }
}
