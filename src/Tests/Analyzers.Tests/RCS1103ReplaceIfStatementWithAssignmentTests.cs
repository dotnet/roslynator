// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1103ReplaceIfStatementWithAssignmentTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.ReplaceIfStatementWithAssignment;

        public override DiagnosticAnalyzer Analyzer { get; } = new IfStatementAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new IfStatementCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ReplaceIfStatementWithAssignment)]
        public async Task Test_InvertCondition()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool a = false;
        int x = 0;

        [|if (x >= 1)
        {
            a = false;
        }
        else
        {
            a = true;
        }|]
    }
}
", @"
class C
{
    void M()
    {
        bool a = false;
        int x = 0;

        a = x < 1;
    }
}
");
        }
    }
}
