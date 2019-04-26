// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1212RemoveRedundantAssignmentTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.RemoveRedundantAssignment;

        public override DiagnosticAnalyzer Analyzer { get; } = new RemoveRedundantAssignmentAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new AssignmentExpressionCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantAssignment)]
        public async Task Test_Local()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    bool M()
    {
        bool f = false;
        bool g = false;

        [|f = false|];
        return f;
    }
}
", @"
class C
{
    bool M()
    {
        bool f = false;
        bool g = false;

        return false;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantAssignment)]
        public async Task Test_Parameter()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    bool M(bool f)
    {
        [|f = false|];
        return f;
    }
}
", @"
class C
{
    bool M(bool f)
    {
        return false;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantAssignment)]
        public async Task Test_Local_WithComment()
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

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantAssignment)]
        public async Task TestNoDiagnostic_OutParameter()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    bool M(out bool f)
    {
        f = false;
        return f;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantAssignment)]
        public async Task TestNoDiagnostic_SequenceOfAssignments()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    int M()
    {
        int x = 1;
        x = x * 2;
        x = x * 2;
        return x;
    }
}");
        }
    }
}
