﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CodeAnalysis.CSharp.Tests;

public class RCS9006UseElementAccessTests : AbstractCSharpDiagnosticVerifier<InvocationExpressionAnalyzer, InvocationExpressionCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = CodeAnalysisDiagnosticRules.UseElementAccess;

    [Fact, Trait(Traits.Analyzer, CodeAnalysisDiagnosticIdentifiers.UseElementAccess)]
    public async Task Test_SyntaxList_First()
    {
        await VerifyDiagnosticAndFixAsync(@"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SyntaxList<SyntaxNode> list = default;

        var first = list.[|First()|];
    }
}
", @"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SyntaxList<SyntaxNode> list = default;

        var first = list[0];
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, CodeAnalysisDiagnosticIdentifiers.UseElementAccess)]
    public async Task Test_SyntaxList_First_Multiline()
    {
        await VerifyDiagnosticAndFixAsync(@"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SyntaxList<SyntaxNode> list = default;

        var first = list
            .[|First()|];
    }
}
", @"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SyntaxList<SyntaxNode> list = default;

        var first = list[0];
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, CodeAnalysisDiagnosticIdentifiers.UseElementAccess)]
    public async Task Test_SyntaxTriviaList_ElementAt()
    {
        await VerifyDiagnosticAndFixAsync(@"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SyntaxTriviaList list = default;

        var second = list.[|ElementAt(1)|];
    }
}
", @"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SyntaxTriviaList list = default;

        var second = list[1];
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, CodeAnalysisDiagnosticIdentifiers.UseElementAccess)]
    public async Task TestNoDiagnostic_FirstWithPredicate()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Linq;
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SyntaxList<SyntaxNode> list = default;

        var first = list.First(f => true);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, CodeAnalysisDiagnosticIdentifiers.UseElementAccess)]
    public async Task TestNoDiagnostic_NotSyntaxList()
    {
        await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        List<SyntaxNode> list = default;

        var first = list.First();
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, CodeAnalysisDiagnosticIdentifiers.UseElementAccess)]
    public async Task TestNoDiagnostic_TrailingTrivia()
    {
        await VerifyNoDiagnosticAsync(@"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SyntaxList<SyntaxNode> list = default;

        var first = list // x
            .First();
    }
}
");
    }
}
