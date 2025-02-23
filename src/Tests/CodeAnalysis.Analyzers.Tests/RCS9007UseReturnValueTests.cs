﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CodeAnalysis.CSharp.Tests;

public class RCS9007UseReturnValueTests : AbstractCSharpDiagnosticVerifier<InvocationExpressionAnalyzer, InvocationExpressionCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = CodeAnalysisDiagnosticRules.UseReturnValue;

    [Fact, Trait(Traits.Analyzer, CodeAnalysisDiagnosticIdentifiers.UseReturnValue)]
    public async Task Test()
    {
        await VerifyDiagnosticAndFixAsync(@"
using Microsoft.CodeAnalysis.CSharp.Syntax;

class C
{
    void M()
    {
        IfStatementSyntax ifStatement = null;
        [|ifStatement.WithCondition(null)|];
    }
}
", @"
using Microsoft.CodeAnalysis.CSharp.Syntax;

class C
{
    void M()
    {
        IfStatementSyntax ifStatement = null;
        var x = ifStatement.WithCondition(null);
    }
}
");
    }
}
