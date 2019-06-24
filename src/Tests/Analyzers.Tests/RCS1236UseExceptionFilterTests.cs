// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1236UseExceptionFilterTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UseExceptionFilter;

        public override DiagnosticAnalyzer Analyzer { get; } = new UseExceptionFilterAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new IfStatementCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExceptionFilter)]
        public async Task Test_IfThrow()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    void M()
    {
        try
        {
        }
        catch (Exception ex)
        {
            [|if|] (!(ex is InvalidOperationException))
            {
                throw;
            }

            return;
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
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {

            return;
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExceptionFilter)]
        public async Task Test_IfThrow_Embedded()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    void M()
    {
        try
        {
        }
        catch (Exception ex)
        {
            [|if|] (!(ex is InvalidOperationException))
                throw;

            return;
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
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {

            return;
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExceptionFilter)]
        public async Task Test_IfThrowElse()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    void M()
    {
        try
        {
        }
        catch (Exception ex)
        {
            [|if|] (!(ex is InvalidOperationException))
            {
                throw;
            }
            else
            {
                return;
            }
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
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
            return;
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExceptionFilter)]
        public async Task Test_IfThrowElse_Embedded()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    void M()
    {
        try
        {
        }
        catch (Exception ex)
        {
            [|if|] (!(ex is InvalidOperationException))
                throw;
            else
                return;
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
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
            return;
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExceptionFilter)]
        public async Task Test_IfThrowElseIf()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    void M()
    {
        bool f = false;

        try
        {
        }
        catch (Exception ex)
        {
            [|if|] (!(ex is InvalidOperationException))
            {
                throw;
            }
            else if (f)
            {
                return;
            }
        }
    }
}
", @"
using System;

class C
{
    void M()
    {
        bool f = false;

        try
        {
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
            if (f)
            {
                return;
            }
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExceptionFilter)]
        public async Task Test_IfElseThrow()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    void M()
    {
        try
        {
        }
        catch (Exception ex)
        {
            [|if|] (ex is InvalidOperationException)
            {
                return;
            }
            else
            {
                throw;
            }
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
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
            return;
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExceptionFilter)]
        public async Task TestNoDiagnostic_HasFilter()
        {
            await VerifyNoDiagnosticAsync(@"
using System;

class C
{
    void M()
    {
        try
        {
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
            if (!(ex is InvalidOperationException))
            {
                throw;
            }

            return;
        }
    }
}
");
        }
    }
}
