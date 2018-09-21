// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1212RemoveRedundantAssignmentTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.RemoveRedundantAssignment;

        public override DiagnosticAnalyzer Analyzer { get; } = new RemoveRedundantAssignmentAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new AssignmentExpressionCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantAssignment)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    string M()
    {
        string s = null;
        [|s = """"|]; //x
        return s;
    }
}
", @"
class C
{
    string M()
    {
        string s = null;
        //x
        return """";
    }
}
");
        }
    }
}
