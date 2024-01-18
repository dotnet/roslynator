// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CSharp.Analysis;
using Roslynator.CSharp.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1265RemoveRedundantCatchBlockTests : AbstractCSharpDiagnosticVerifier<RemoveRedundantCatchBlockAnalyzer, RemoveRedundantCatchBlockCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.RemoveRedundantCatchBlock;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantCatchBlock)]
    public async Task Test_TryCatchFinally()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        try
        {
            DoSomething();
        }
        [|catch
        {
           throw;
        }|]
        finally
        {
            DoSomething();
        }
    }

    void DoSomething()
    {
    }
}
", @"
class C
{
    void M()
    {
        try
        {
            DoSomething();
        }
        finally
        {
            DoSomething();
        }
    }

    void DoSomething()
    {
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantCatchBlock)]
    public async Task Test_TryMultipleCatches()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    void M()
    {
        try
        {
           DoSomething();
        }
        catch (SystemException ex)
        {
           DoSomething();
        }
        [|catch
        {
           throw;
        }|]
    }

    void DoSomething()
    {
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
           DoSomething();
        }
        catch (SystemException ex)
        {
           DoSomething();
        }
    }

    void DoSomething()
    {
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantCatchBlock)]
    public async Task Test_TryCatch()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        try
        {
           DoSomething();
        }
        [|catch
        {
           throw;
        }|]
    }

    void DoSomething()
    {
    }
}
", @"
class C
{
    void M()
    {
        DoSomething();
    }

    void DoSomething()
    {
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantCatchBlock)]
    public async Task Test_NoDiagnostic()
    {
        await VerifyNoDiagnosticAsync(@"
using System;

class C
{
    void M()
    {
        try
        {
            DoSomething();
        }
        catch
        {
            throw new SystemException();
        }

        void DoSomething()
        {
        }
    }
}
");
    }
}
