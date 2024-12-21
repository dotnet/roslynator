// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1196CallExtensionMethodAsInstanceMethodTests : AbstractCSharpDiagnosticVerifier<CallExtensionMethodAsInstanceMethodAnalyzer, InvocationExpressionCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.CallExtensionMethodAsInstanceMethod;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.CallExtensionMethodAsInstanceMethod)]
    public async Task Test()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Linq;
using System.Collections.Generic;

class C
{
    void M(List<int> items)
    {
        var x = [|Enumerable.Select(items, f => f.ToString())|];
    }
}
", @"
using System;
using System.Linq;
using System.Collections.Generic;

class C
{
    void M(List<int> items)
    {
        var x = items.Select(f => f.ToString());
    }
}
");
    }
}
