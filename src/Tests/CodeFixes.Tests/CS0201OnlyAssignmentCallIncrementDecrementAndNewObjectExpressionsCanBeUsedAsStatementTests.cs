// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Xunit;

#pragma warning disable RCS1090

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS0201OnlyAssignmentCallIncrementDecrementAndNewObjectExpressionsCanBeUsedAsStatementTests : AbstractCSharpCompilerCodeFixVerifier
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.OnlyAssignmentCallIncrementDecrementAndNewObjectExpressionsCanBeUsedAsStatement;

        public override CodeFixProvider FixProvider { get; } = new ExpressionCodeFixProvider();

        [Fact]
        public async Task Test_RemoveParentheses()
        {
            await VerifyFixAsync(@"
class C
{
    void M()
    {
        (M());
    }
}
", @"
class C
{
    void M()
    {
        M();
    }
}
", EquivalenceKey.Create(DiagnosticId));
        }

        [Fact]
        public async Task Test_IntroduceLocal()
        {
            await VerifyFixAsync(@"
using System;

class C
{
    void M()
    {
        DateTime.Now;
    }
}
", @"
using System;

class C
{
    void M()
    {
        var dateTime = DateTime.Now;
    }
}
", EquivalenceKey.Create(DiagnosticId, CodeFixIdentifiers.IntroduceLocalVariable));
        }

        [Fact]
        public async Task Test_IntroduceField()
        {
            await VerifyFixAsync(@"
using System;

class C
{
    void M()
    {
        DateTime.Now;
    }
}
", @"
using System;

class C
{
    private DateTime _dateTime;

    void M()
    {
        _dateTime = DateTime.Now;
    }
}
", EquivalenceKey.Create(DiagnosticId, CodeFixIdentifiers.IntroduceField));
        }

        [Fact]
        public async Task Test_IntroduceStaticField()
        {
            await VerifyFixAsync(@"
using System;

class C
{
    static void M()
    {
        DateTime.Now;
    }
}
", @"
using System;

class C
{
    private static DateTime _dateTime;

    static void M()
    {
        _dateTime = DateTime.Now;
    }
}
", EquivalenceKey.Create(DiagnosticId, CodeFixIdentifiers.IntroduceField));
        }

        [Fact]
        public async Task Test_AddArgumentList()
        {
            await VerifyFixAsync(@"
class C
{
    void M()
    {
        C x = null;

        M;
        x.M;
    }
}
", @"
class C
{
    void M()
    {
        C x = null;

        M();
        x.M();
    }
}
", EquivalenceKey.Create(DiagnosticId, CodeFixIdentifiers.AddArgumentList));
        }

        [Fact]
        public async Task Test_ReplaceConditionalExpressionWithIfElse()
        {
            await VerifyFixAsync(@"
class C
{
    void M()
    {
        bool f = false;
        (f) ? M() : M2();
    }

    void M2()
    {
    }
}
", @"
class C
{
    void M()
    {
        bool f = false;
        if (f)
        {
            M();
        }
        else
        {
            M2();
        }
    }

    void M2()
    {
    }
}
", EquivalenceKey.Create(DiagnosticId, CodeFixIdentifiers.ReplaceConditionalExpressionWithIfElse));
        }

        [Fact]
        public async Task Test_ReplaceComparisonWithAssignment()
        {
            await VerifyFixAsync(@"
class C
{
    void M(string s)
    {
        s == null;
        s == """";
    }
}
", @"
class C
{
    void M(string s)
    {
        s = null;
        s = """";
    }
}
", EquivalenceKey.Create(DiagnosticId, CodeFixIdentifiers.ReplaceComparisonWithAssignment));
        }
    }
}
