// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.Analyzers.Tests;

public class RCS1166ValueTypeObjectIsNeverEqualToNullTests : AbstractCSharpDiagnosticVerifier<ValueTypeObjectIsNeverEqualToNullAnalyzer, BinaryExpressionCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.ValueTypeObjectIsNeverEqualToNull;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ValueTypeObjectIsNeverEqualToNull)]
    public async Task Test_NullOnLeft_Equals()
    {
        await VerifyDiagnosticAsync(@"
using System.Collections.Immutable;

class C
{
    void M()
    {
        var a = ImmutableArray<int>.Empty;

        if ([|null == a|])
        {
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ValueTypeObjectIsNeverEqualToNull)]
    public async Task Test_NullOnLeft_NotEquals()
    {
        await VerifyDiagnosticAsync(@"
using System.Collections.Immutable;

class C
{
    void M()
    {
        var a = ImmutableArray<int>.Empty;

        if ([|null != a|])
        {
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ValueTypeObjectIsNeverEqualToNull)]
    public async Task TestNoDiagnostic_NullOnLeft_NullableValueType()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        int? a = null;

        if (null == a)
        {
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ValueTypeObjectIsNeverEqualToNull)]
    public async Task TestNoDiagnostic_NullOnLeft_ReferenceType()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        string a = null;

        if (null == a)
        {
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ValueTypeObjectIsNeverEqualToNull)]
    public async Task TestFix_NullOnLeft_Equals()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Immutable;

class C
{
    void M()
    {
        var a = ImmutableArray<int>.Empty;
        if ([|null == a|])
        {
        }
    }
}
", @"
using System.Collections.Immutable;

class C
{
    void M()
    {
        var a = ImmutableArray<int>.Empty;
        if (default == a)
        {
        }
    }
}
");
    }
}
