// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1044RemoveOriginalExceptionFromThrowStatementTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.RemoveOriginalExceptionFromThrowStatement;

        public override DiagnosticAnalyzer Analyzer { get; } = new RemoveOriginalExceptionFromThrowStatementAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new RemoveOriginalExceptionCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveOriginalExceptionFromThrowStatement)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    void M()
    {
        try
        {
            M();
        }
        catch (Exception ex)
        {
            throw [|ex|];
        }
    }
}
", @"
using System;

class C
{
    void M()
    {
        try
        {
            M();
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
");
        }
    }
}
