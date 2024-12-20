// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1061MergeIfWithNestedIfTests : AbstractCSharpDiagnosticVerifier<MergeIfWithNestedIfAnalyzer, IfStatementCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor => DiagnosticRules.MergeIfWithNestedIf;

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.MergeIfWithNestedIf)]
    public async Task Test_MergeIfStatement()
    {
        await VerifyDiagnosticAndFixAsync("""
using System.Collections.Generic;
class C
{
    public static void M(Dictionary<string, string> settings, string name)
    {
        [|if (name == "name1")
        {
            if (settings.TryGetValue("name1", out var v1))
            {
            }
        }|]
    }
}
""",
"""
using System.Collections.Generic;
class C
{
    public static void M(Dictionary<string, string> settings, string name)
    {
        if (name == "name1" && settings.TryGetValue("name1", out var v1))
        {
        }
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.MergeIfWithNestedIf)]
    public async Task TestNoDiagnostic_WhenLocalVariablesOverlap()
    {
        await VerifyNoDiagnosticAsync("""
using System.Collections.Generic;
class C
{
    public static void M(Dictionary<string, string> settings, string name)
    {
        if (name == "name1")
        {
            if (settings.TryGetValue("name1", out var v1))
            {
            }
        }
        
        if (name == "name2")
        {
            if (settings.TryGetValue("name2", out var v1))
            {
            }
        }
    }
}
""");
    }
}
