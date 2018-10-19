// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1179UseReturnInsteadOfAssignmentTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UseReturnInsteadOfAssignment;

        public override DiagnosticAnalyzer Analyzer { get; } = new UseReturnInsteadOfAssignmentAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new UseReturnInsteadOfAssignmentCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseReturnInsteadOfAssignment)]
        public async Task Test_IfStatement()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    int M()
    {
        bool f = false;
        int x = 1; // x
        [|if (f)
        {
            x = 2;
        }
        else if (f)
        {
            x = 3;
        }|]

        return x;
    }
}
", @"
class C
{
    int M()
    {
        bool f = false;
        // x
        if (f)
        {
            return 2;
        }
        else if (f)
        {
            return 3;
        }

        return 1;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseReturnInsteadOfAssignment)]
        public async Task Test_IfStatement2()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    int M()
    {
        bool f = false;

        // x
        int x = 1; 
        [|if (f)
        {
            x = 2;
        }
        else if (f)
        {
            x = 3;
        }|]

        return x;
    }
}
", @"
class C
{
    int M()
    {
        bool f = false;

        // x
        if (f)
        {
            return 2;
        }
        else if (f)
        {
            return 3;
        }

        return 1;
    }
}
");
        }
    }
}
