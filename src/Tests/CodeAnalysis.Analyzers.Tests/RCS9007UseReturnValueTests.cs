// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace Roslynator.CodeAnalysis.CSharp.Tests
{
    public class RCS9007UseReturnValueTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UseReturnValue;

        protected override DiagnosticAnalyzer Analyzer { get; } = new InvocationExpressionAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new InvocationExpressionCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseReturnValue)]
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
}
