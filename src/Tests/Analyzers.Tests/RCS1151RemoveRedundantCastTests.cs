// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1151RemoveRedundantCastTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.RemoveRedundantCast;

        public override DiagnosticAnalyzer Analyzer { get; } = new RemoveRedundantCastAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new RemoveRedundantCastCodeFixProvider();

        //TODO: Add test for RCS1151
        //[Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantCast)]
        public async Task TestNoDiagnostic_DefaultInterfaceImplementation()
        {
            await VerifyNoDiagnosticAsync(@"
interface IC
{
    void M()
    {
    }
}

class C : IC
{
    void M2()
    {
        var c = new C();

        ((IC)c).M();
    }
}
");
        }
    }
}
