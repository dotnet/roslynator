// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1231MakeParameterRefReadOnlyTests : AbstractCSharpDiagnosticVerifier<RefReadOnlyParameterAnalyzer, ParameterCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.MakeParameterRefReadOnly;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.MakeParameterRefReadOnly)]
    public async Task Test()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;

readonly struct C
{
    void M(C [|c|])
    {
        IEnumerable<object> LF()
        {
            yield return null;
        }
    }
}
", @"
using System.Collections.Generic;

readonly struct C
{
    void M(in C c)
    {
        IEnumerable<object> LF()
        {
            yield return null;
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.MakeParameterRefReadOnly)]
    public async Task TestNoDiagnostic_Assigned()
    {
        await VerifyNoDiagnosticAsync(@"
readonly struct C
{
    void M(C c)
    {
        c = default(C);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.MakeParameterRefReadOnly)]
    public async Task TestNoDiagnostic_ReferencedInLocalFunction()
    {
        await VerifyNoDiagnosticAsync(@"
readonly struct C
{
    void M(C c)
    {
        void LF()
        {
            var x = c;
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.MakeParameterRefReadOnly)]
    public async Task TestNoDiagnostic_ReferencedInLambda()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Linq;

readonly struct C
{
    void M(C c)
    {
        var items = Enumerable.Empty<C>().Select(f => c);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.MakeParameterRefReadOnly)]
    public async Task TestNoDiagnostic_Iterator()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;

readonly struct C
{
    IEnumerable<object> M(C c)
    {
        yield return null;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.MakeParameterRefReadOnly)]
    public async Task TestNoDiagnostic_DuplicateParameterName()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;

readonly struct C
{
    void M(C c, C c)
    {
    }
}
", options: Options.AddAllowedCompilerDiagnosticId("CS0100"));
    }

#if ROSLYN_TEST_4_12_OR_GREATER
    // Test source uses 'params ReadOnlySpan<T>' (params collections, C# 13 / Roslyn 4.12).
    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.MakeParameterRefReadOnly)]
    public async Task TestNoDiagnostic_ParamsReadOnlySpan()
    {
        await VerifyNoDiagnosticAsync(@"
using System;

readonly struct C
{
    void M(params ReadOnlySpan<C> c)
    {
    }
}
");
    }

#endif

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.MakeParameterRefReadOnly)]
    public async Task TestNoDiagnostic_MethodReferencedAsMethodGroup()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    readonly struct B
    {
        public int P { get; }
    }

    bool M(B p) => p.P > 0;

    bool M(List<B> p) => p.Any(M);
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.MakeParameterRefReadOnly)]
    public async Task TestNoDiagnostic_LocalFunctionReferencedAsMethodGroup()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    readonly struct B
    {
        public int P { get; }
    }

    bool M(List<B> p)
    {
        return p.Any(M);

        bool M(B p2) => p2.P > 0;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.MakeParameterRefReadOnly)]
    public async Task TestNoDiagnostic_BoolType()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M(bool value)
    {
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.MakeParameterRefReadOnly)]
    public async Task TestNoDiagnostic_RefStruct_Returned()
    {
        await VerifyNoDiagnosticAsync(@"
public readonly ref struct RefStruct;

public static class Methods
{
    public static RefStruct DoSomething(RefStruct @struct)
    {
        return @struct;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.MakeParameterRefReadOnly)]
    public async Task TestNoDiagnostic_ExpressionTree()
    {
        await VerifyNoDiagnosticAsync(@"
using System;
using System.Linq;

class C
{
    public void M(DateTime dt)
    {
        var items = default(IQueryable<C>);

        var x = from item in items
            where item.P <= dt
            select item;
    }

    public DateTime P { get; set; }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.MakeParameterRefReadOnly)]
    public async Task TestNoDiagnostic_CancellationToken_SyncTaskReturningMethod()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Threading;
using System.Threading.Tasks;

class C
{
    public Task DoThatWay(CancellationToken cancellationToken)
    {
        return Task.FromCanceled(cancellationToken);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.MakeParameterRefReadOnly)]
    public async Task Test_ReadOnlyStruct_SyncTaskReturningMethod()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Threading.Tasks;

readonly struct C
{
    public Task<int> M(C [|c|])
    {
        return Task.FromResult(c.GetHashCode());
    }
}
", @"
using System.Threading.Tasks;

readonly struct C
{
    public Task<int> M(in C c)
    {
        return Task.FromResult(c.GetHashCode());
    }
}
");
    }
}
