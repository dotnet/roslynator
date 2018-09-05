// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1163UnusedParameterTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UnusedParameter;

        public override DiagnosticAnalyzer Analyzer { get; } = new UnusedParameter.UnusedParameterAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new UnusedParameterCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnusedParameter)]
        public async Task Test_Method()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    void M()
    {
        object _ = null;

        Action<string> action = [|f|] => M();
    }
}
", @"
using System;

class C
{
    void M()
    {
        object _ = null;

        Action<string> action = __ => M();
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnusedParameter)]
        public async Task TestNoDiagnostic_StackAllocArrayCreationExpression()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    unsafe void M(int length)
    {
        var memory = stackalloc byte[length];
    }
}
");
        }
    }
}
