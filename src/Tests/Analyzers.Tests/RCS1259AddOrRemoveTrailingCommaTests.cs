// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1259AddOrRemoveTrailingCommaTests : AbstractCSharpDiagnosticVerifier<AddOrRemoveTrailingCommaAnalyzer, AddOrRemoveTrailingCommaCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.AddOrRemoveTrailingComma;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveTrailingComma)]
    public async Task Test_AddComma_ArrayInitializer()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        var arr = new[]
        {
            0,
            1[||]
        };
    }
}
", @"
class C
{
    void M()
    {
        var arr = new[]
        {
            0,
            1,
        };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.TrailingCommaStyle, ConfigOptionValues.TrailingCommaStyle_Include));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveTrailingComma)]
    public async Task Test_RemoveComma_ArrayInitializer()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        var arr = new[]
        {
            0,
            1[|,|]
        };
    }
}
", @"
class C
{
    void M()
    {
        var arr = new[]
        {
            0,
            1
        };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.TrailingCommaStyle, ConfigOptionValues.TrailingCommaStyle_Omit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveTrailingComma)]
    public async Task Test_AddComma_ObjectInitializer()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    int P1 { get; set; }
    int P2 { get; set; }

    void M()
    {
        var items = new C()
        {
            P1 = 0,
            P2 = 1[||]
        };
    }
}
", @"
class C
{
    int P1 { get; set; }
    int P2 { get; set; }

    void M()
    {
        var items = new C()
        {
            P1 = 0,
            P2 = 1,
        };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.TrailingCommaStyle, ConfigOptionValues.TrailingCommaStyle_Include));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveTrailingComma)]
    public async Task Test_RemoveComma_ObjectInitializer()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    int P1 { get; set; }
    int P2 { get; set; }

    void M()
    {
        var items = new C()
        {
            P1 = 0,
            P2 = 1[|,|]
        };
    }
}
", @"
class C
{
    int P1 { get; set; }
    int P2 { get; set; }

    void M()
    {
        var items = new C()
        {
            P1 = 0,
            P2 = 1
        };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.TrailingCommaStyle, ConfigOptionValues.TrailingCommaStyle_Omit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveTrailingComma)]
    public async Task Test_AddComma_CollectionInitializer()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;

class C
{
    void M()
    {
        var items = new List<int>()
        {
            0,
            1[||]
        };
    }
}
", @"
using System.Collections.Generic;

class C
{
    void M()
    {
        var items = new List<int>()
        {
            0,
            1,
        };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.TrailingCommaStyle, ConfigOptionValues.TrailingCommaStyle_Include));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveTrailingComma)]
    public async Task Test_RemoveComma_CollectionInitializer()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;

class C
{
    void M()
    {
        var items = new List<int>()
        {
            0,
            1[|,|]
        };
    }
}
", @"
using System.Collections.Generic;

class C
{
    void M()
    {
        var items = new List<int>()
        {
            0,
            1
        };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.TrailingCommaStyle, ConfigOptionValues.TrailingCommaStyle_Omit));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveTrailingComma)]
    public async Task Test_AddComma_AnonymousObjectCreationExpression()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        var obj = new
        {
            A = 0,
            B = 1[||]
        };
    }
}
", @"
class C
{
    void M()
    {
        var obj = new
        {
            A = 0,
            B = 1,
        };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.TrailingCommaStyle, ConfigOptionValues.TrailingCommaStyle_Include));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddOrRemoveTrailingComma)]
    public async Task Test_RemoveComma_AnonymousObjectCreationExpression()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        var obj = new
        {
            A = 0,
            B = 1[|,|]
        };
    }
}
", @"
class C
{
    void M()
    {
        var obj = new
        {
            A = 0,
            B = 1
        };
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.TrailingCommaStyle, ConfigOptionValues.TrailingCommaStyle_Omit));
    }
}
