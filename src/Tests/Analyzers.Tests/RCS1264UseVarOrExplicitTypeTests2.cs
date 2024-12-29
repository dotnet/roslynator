// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1264UseVarOrExplicitTypeTests2 : AbstractCSharpDiagnosticVerifier<UseVarOrExplicitTypeAnalyzer, UseVarOrExplicitTypeCodeFixProvider>
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
        [|var|] x = new List<string>();
    }
}
", @"
using System.Collections.Generic;

class C
{
    void M()
    {
        List<string> x = new List<string>();
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.UseVar, ConfigOptionValues.UseVar_Never));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarOrExplicitType)]
    public async Task Test_Tuple_DeclarationExpression()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        [|var|] (x, y) = default((object, System.DateTime));
    }
}
", @"
class C
{
    void M()
    {
        (object x, System.DateTime y) = default((object, System.DateTime));
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.UseVar, ConfigOptionValues.UseVar_Never));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarOrExplicitType)]
    public async Task Test_TupleExpression()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        (object x, [|var|] y) = default((object, System.DateTime));
    }
}
", @"
class C
{
    void M()
    {
        (object x, System.DateTime y) = default((object, System.DateTime));
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.UseVar, ConfigOptionValues.UseVar_Never));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarOrExplicitType)]
    public async Task Test_TupleExpression_AllVar()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        ([|var|] x, [|var|] y) = default((object, System.DateTime));
    }
}
", @"
class C
{
    void M()
    {
        (object x, System.DateTime y) = default((object, System.DateTime));
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.UseVar, ConfigOptionValues.UseVar_Never));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarOrExplicitType)]
    public async Task Test_ParseMethod()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    void M()
    {
        [|var|] timeSpan = TimeSpan.Parse(null);
    }
}
", @"
using System;

class C
{
    void M()
    {
        TimeSpan timeSpan = TimeSpan.Parse(null);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.UseVar, ConfigOptionValues.UseVar_Never));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarOrExplicitType)]
    public async Task Test_NestedTuple()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
#nullable enable

class C
{
    void M()
    {
        [|var|] ((x, (y, y2)), z) = M2();
    }

    ((string?, (string, string?)), string) M2() => default(((string?, (string, string?)), string));
}
", @"
using System;
#nullable enable

class C
{
    void M()
    {
        ((string? x, (string y, string? y2)), string z) = M2();
    }

    ((string?, (string, string?)), string) M2() => default(((string?, (string, string?)), string));
}
", options: Options.AddConfigOption(ConfigOptionKeys.UseVar, ConfigOptionValues.UseVar_Never));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarOrExplicitType)]
    public async Task Test_NotObviousExpression()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Threading.Tasks;
using System.Collections.Generic;

class C
{
    void M()
    {
        [|var|] x1 = string.Empty;
        [|var|] x2 = Task.FromResult(string.Empty);
    }
}
", @"
using System.Threading.Tasks;
using System.Collections.Generic;

class C
{
    void M()
    {
        string x1 = string.Empty;
        Task<string> x2 = Task.FromResult(string.Empty);
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.UseVar, ConfigOptionValues.UseVar_Never));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarOrExplicitType)]
    public async Task TestNoDiagnostic_ForEach_DeclarationExpression()
    {
        await VerifyNoDiagnosticAsync(@"
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
    public async Task TestNoDiagnostic_ForEach_TupleExpression()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;

class C
{
    IEnumerable<(object x, string y)> M()
    {
        foreach ((object x, string y) in M())
        {
        }

        return default;
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.UseVar, ConfigOptionValues.UseVar_Never));
    }
}
