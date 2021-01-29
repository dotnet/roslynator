// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1242DoNotPassNonReadOnlyStructByReadOnlyReferenceTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.DoNotPassNonReadOnlyStructByReadOnlyReference;

        protected override DiagnosticAnalyzer Analyzer { get; } = new RefReadOnlyParameterAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new ParameterCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DoNotPassNonReadOnlyStructByReadOnlyReference)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
struct C
{
    void M(in C [|c|])
    {
    }
}
", @"
struct C
{
    void M(C c)
    {
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DoNotPassNonReadOnlyStructByReadOnlyReference)]
        public async Task TestNoDiagnostic_ReadOnlyStruct()
        {
            await VerifyNoDiagnosticAsync(@"
readonly struct C
{
    void M(in C c)
    {
    }
}
");
        }
    }
}
