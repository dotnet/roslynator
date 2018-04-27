// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.CodeFixes;
using Xunit;
using static Roslynator.Tests.CSharp.CSharpDiagnosticVerifier;

namespace Roslynator.Analyzers.Tests
{
    public static class RCS1032RemoveRedundantParenthesesTests
    {
        private static DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.RemoveRedundantParentheses;

        private static DiagnosticAnalyzer Analyzer { get; } = new RemoveRedundantParenthesesAnalyzer();

        private static CodeFixProvider CodeFixProvider { get; } = new RemoveRedundantParenthesesCodeFixProvider();

        [Fact]
        public static void TestDiagnosticWithCodeFix_Argument()
        {
            VerifyDiagnosticAndFix(@"
class C
{
    void M(object x)
    {
        M(<<<(>>>x));
    }
}
", @"
class C
{
    void M(object x)
    {
        M(x);
    }
}
", Descriptor, Analyzer, CodeFixProvider);
        }

        [Fact]
        public static void TestDiagnosticWithCodeFix_AttributeArgument()
        {
            VerifyDiagnosticAndFix(@"
using System;

[Obsolete(<<<(>>>""""))]
class C
{
}
", @"
using System;

[Obsolete("""")]
class C
{
}
", Descriptor, Analyzer, CodeFixProvider);
        }

        [Fact]
        public static void TestDiagnosticWithCodeFix_ReturnExpression()
        {
            VerifyDiagnosticAndFix(@"
class C
{
    object M()
    {
        return <<<(>>>null);
    }
}
", @"
class C
{
    object M()
    {
        return null;
    }
}
", Descriptor, Analyzer, CodeFixProvider);
        }

        [Fact]
        public static void TestDiagnosticWithCodeFix_YieldReturnExpression()
        {
            VerifyDiagnosticAndFix(@"
using System.Collections.Generic;

class C
{
    IEnumerable<object> M()
    {
        yield return <<<(>>>null);
    }
}
", @"
using System.Collections.Generic;

class C
{
    IEnumerable<object> M()
    {
        yield return null;
    }
}
", Descriptor, Analyzer, CodeFixProvider);
        }

        [Fact]
        public static void TestDiagnosticWithCodeFix_ExpressionBody()
        {
            VerifyDiagnosticAndFix(@"
class C
{
    object M() => <<<(>>>null);
}
", @"
class C
{
    object M() => null;
}
", Descriptor, Analyzer, CodeFixProvider);
        }

        [Fact]
        public static void TestDiagnosticWithCodeFix_AwaitExpression()
        {
            VerifyDiagnosticAndFix(@"
using System.Threading.Tasks;

class C
{
    async Task FooAsync()
    {
        await <<<(>>>FooAsync().ConfigureAwait(false));
        await <<<(>>>(Task)FooAsync());
    }
}
", @"
using System.Threading.Tasks;

class C
{
    async Task FooAsync()
    {
        await FooAsync().ConfigureAwait(false);
        await (Task)FooAsync();
    }
}
", Descriptor, Analyzer, CodeFixProvider);
        }

        [Theory]
        [InlineData("while (<<<(>>>true)) { }", "while (true) { }")]
        [InlineData("do { } while (<<<(>>>true));", "do { } while (true);")]
        [InlineData("using (<<<(>>>(IDisposable)null)) { }", "using ((IDisposable)null) { }")]
        [InlineData("lock (<<<(>>>s)) { }", "lock (s) { }")]
        [InlineData("if (<<<(>>>true)) { }", "if (true) { }")]
        [InlineData("switch (<<<(>>>true)) { }", "switch (true) { }")]
        [InlineData(@"M(<<<(>>>""""));", @"M("""");")]
        [InlineData("var arr = new string[] { <<<(>>>null) };", "var arr = new string[] { null };")]
        [InlineData("var items = new List<string>() { <<<(>>>null) };", "var items = new List<string>() { null };")]
        [InlineData(@"s = $""{<<<(>>>"""")}"";", @"s = $""{""""}"";")]
        [InlineData("<<<(>>>i) = (0);", "i = (0);")]
        [InlineData("<<<(>>>i) += (0);", "i += (0);")]
        [InlineData("<<<(>>>i) -= (0);", "i -= (0);")]
        [InlineData("<<<(>>>i) *= (0);", "i *= (0);")]
        [InlineData("<<<(>>>i) /= (0);", "i /= (0);")]
        [InlineData("<<<(>>>i) %= (0);", "i %= (0);")]
        [InlineData("<<<(>>>i) &= (0);", "i &= (0);")]
        [InlineData("<<<(>>>i) ^= (0);", "i ^= (0);")]
        [InlineData("<<<(>>>i) |= (0);", "i |= (0);")]
        [InlineData("<<<(>>>i) <<= (0);", "i <<= (0);")]
        [InlineData("<<<(>>>i) >>= (0);", "i >>= (0);")]
        public static void TestDiagnosticWithCodeFix_Statement(string fixableCode, string fixedCode)
        {
            VerifyDiagnosticAndFix(@"
using System;
using System.Collections.Generic;

class C
{
    void M(string s)
    {
        int i = 0;

        <<<>>>
    }
}
", fixableCode, fixedCode, Descriptor, Analyzer, CodeFixProvider);
        }

        [Theory]
        [InlineData("f = !<<<(>>>f);", "f = !f;")]
        [InlineData(@"f = !<<<(>>>s.StartsWith(""""));", @"f = !s.StartsWith("""");")]
        [InlineData("f = !<<<(>>>foo.Value);", "f = !foo.Value;")]
        [InlineData("f = !<<<(>>>foo[0]);", "f = !foo[0];")]
        public static void TestDiagnosticWithCodeFix_LogicalNot(string fixableCode, string fixedCode)
        {
            VerifyDiagnosticAndFix(@"
class Foo
{
    void M()
    {
        bool f = false;
        string s = null;
        var foo = new Foo();

        <<<>>>
    }

    public bool Value { get; }

    public bool this[int i]
    {
        get { return i == 0; }
    }
}
", fixableCode, fixedCode, Descriptor, Analyzer, CodeFixProvider);
        }

        [Fact]
        public static void TestNoDiagnostic_AssignmentInInitializer()
        {
            VerifyNoDiagnostic(@"
using System.Collections.Generic;

class C
{
    void M()
    {
        string x;
        var items = new List<string>() { (x = ""x"") };    
    }
}
", Descriptor, Analyzer);
        }

        [Fact]
        public static void TestNoDiagnostic_ConditionalExpressionInInterpolatedString()
        {
            VerifyNoDiagnostic(@"
class C
{
    void M()
    {
            string s = $""{ ((true) ? ""a"" : ""b"")}"";
    }
}
", Descriptor, Analyzer);
        }

        [Fact]
        public static void TestNoDiagnostic_AssignmentInAwaitExpression()
        {
            VerifyNoDiagnostic(@"
using System;
using System.Threading.Tasks;

class C
{
    async Task FooAsync(Task task) => await (task = Task.Run(default(Action)));
}
", Descriptor, Analyzer);
        }

        [Fact]
        public static void TestNoDiagnostic_ForEach()
        {
            VerifyNoDiagnostic(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M(IEnumerable<string> items)
    {
        foreach (string item in (items))
        {
        }

        foreach ((string, string) item in (Enumerable.Empty<(string, string)>()))
        {
        }
    }
}
", Descriptor, Analyzer);
        }
    }
}
