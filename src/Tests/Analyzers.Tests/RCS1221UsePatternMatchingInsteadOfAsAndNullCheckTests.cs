// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Analysis.UsePatternMatching;
using Roslynator.CSharp.CodeFixes;
using Xunit;

#pragma warning disable RCS1090

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1221UsePatternMatchingInsteadOfAsAndNullCheckTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UsePatternMatchingInsteadOfAsAndNullCheck;

        public override DiagnosticAnalyzer Analyzer { get; } = new UsePatternMatchingInsteadOfAsAndNullCheckAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new UsePatternMatchingInsteadOfAsAndNullCheckCodeFixProvider();

        [Fact]
        public async Task Test_EqualsToNull()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        object x = null;

        [|var s = x as string;|]
        if (s == null)
        {
            return;
        }
    }
}
", @"
class C
{
    void M()
    {
        object x = null;

        if (!(x is string s))
        {
            return;
        }
    }
}
");
        }

        [Fact]
        public async Task Test_IsNull()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        object x = null;

        [|var s = x as string;|]
        if (s is null)
        {
            return;
        }
    }
}
", @"
class C
{
    void M()
    {
        object x = null;

        if (!(x is string s))
        {
            return;
        }
    }
}
");
        }

        [Fact]
        public async Task TestNoDiagnostic()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        object x = null;

        string s = x as string, y = x as string;
        if (s == null)
        {
            return;
        }

        var s2 = x as string;
        if (s2 == null)
        {
            return;
        }
        else
        {
        }

        var s3 = x as string;
        if (s3 == null)
        {
            M();
        }

        var s4 = x as string;
        if (s4 != null)
        {
            return;
        }

        var s5 = x as string;
        if (s4 == null)
        {
            return;
        }
    }
}
");
        }

        [Fact]
        public async Task TestNoDiagnostic_Directive()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        object x = null;

#region
        var s = x as string;
#endregion
        if (s == null)
        {
            return;
        }
    }
}
");
        }
    }
}
