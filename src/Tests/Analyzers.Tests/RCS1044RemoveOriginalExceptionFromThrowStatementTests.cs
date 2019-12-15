// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using System.Threading.Tasks;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1044RemoveOriginalExceptionFromThrowStatementTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.RemoveOriginalExceptionFromThrowStatement;

        public override DiagnosticAnalyzer Analyzer { get; } = new RemoveOriginalExceptionFromThrowStatementAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new RemoveOriginalExceptionCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveOriginalExceptionFromThrowStatement)]
        public async Task Test_OriginalExceptionUsed()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

public class A
{
    public void Foo()
    {
        try
        {
            Foo();
        }
        catch (Exception ex)
        {        
            throw [|ex|];
        }
    }
}
", @"
using System;

public class A
{
    public void Foo()
    {
        try
        {
            Foo();
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveOriginalExceptionFromThrowStatement)]
        public async Task TestNoDiagnostic_OnlyThrowStatement()
        {
            await VerifyNoDiagnosticAsync(@"
using System;

public class A
{
    public void Foo()
    {
        try
        {
            Foo();
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveOriginalExceptionFromThrowStatement)]
        public async Task TestNoDiagnostic_NewExceptionInstantiated()
        {
            await VerifyNoDiagnosticAsync(@"
using System;

public class A
{
    public void Foo()
    {
        try
        {
            Foo();
        }
        catch (Exception ex)
        {        
            throw new Exception();
        }
    }
}
");
        }
    }
}
