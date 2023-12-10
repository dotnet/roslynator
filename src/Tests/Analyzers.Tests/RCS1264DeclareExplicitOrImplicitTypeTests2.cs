// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1264DeclareExplicitOrImplicitTypeTests2 : AbstractCSharpDiagnosticVerifier<DeclareExplicitOrImplicitTypeAnalyzer, UseExplicitTypeCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.DeclareExplicitOrImplicitType;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_Explicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_Implicit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.DeclareExplicitOrImplicitType)]
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
", options: Options.AddConfigOption(ConfigOptionKeys.TypeStyle, ConfigOptionValues.TypeStyle_Explicit));
    }
}
