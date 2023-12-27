// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1264UseVarOrExplicitTypeTests6 : AbstractCSharpDiagnosticVerifier<UseVarOrExplicitTypeAnalyzer, UseVarOrExplicitTypeCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.UseVarOrExplicitType;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarOrExplicitType)]
    public async Task Test()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;

class C
{
    void M()
    {
        var items = new List<string>();

        foreach ([|string|] item in items)
        {
        }
    }
}
", @"
using System.Collections.Generic;

class C
{
    void M()
    {
        var items = new List<string>();

        foreach (var item in items)
        {
        }
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.UseVar, ConfigOptionValues.UseVar_Always));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarOrExplicitType)]
    public async Task Test_TupleExpression()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;

class C
{
    IEnumerable<(object x, object y)> M()
    {
        foreach ([|(object x, object y)|] in M())
        {
        }

        return default;
    }
}
", @"
using System.Collections.Generic;

class C
{
    IEnumerable<(object x, object y)> M()
    {
        foreach (var (x, y) in M())
        {
        }

        return default;
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.UseVar, ConfigOptionValues.UseVar_Always));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarOrExplicitType)]
    public async Task Test_TupleExpression_Var()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;

class C
{
    IEnumerable<(object x, object y)> M()
    {
        foreach ([|(object x, var y)|] in M())
        {
        }

        return default;
    }
}
", @"
using System.Collections.Generic;

class C
{
    IEnumerable<(object x, object y)> M()
    {
        foreach (var (x, y) in M())
        {
        }

        return default;
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.UseVar, ConfigOptionValues.UseVar_Always));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarOrExplicitType)]
    public async Task Test_TupleExpression2()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;

class C
{
    IEnumerable<(object x, string y)> M()
    {
        foreach ([|(object x, string y)|] in M())
        {
        }

        return default;
    }
}
", @"
using System.Collections.Generic;

class C
{
    IEnumerable<(object x, string y)> M()
    {
        foreach (var (x, y) in M())
        {
        }

        return default;
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.UseVar, ConfigOptionValues.UseVar_Always));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarOrExplicitType)]
    public async Task TestNoDiagnostic_TupleExpression()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    (object x, object y) M()
    {
        (object x, object y) = M();

        return default;
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.UseVar, ConfigOptionValues.UseVar_WhenTypeIsObvious));
    }
}
