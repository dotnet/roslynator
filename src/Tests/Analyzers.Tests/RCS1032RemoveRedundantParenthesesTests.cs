// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;
using System.Threading.Tasks;

#pragma warning disable RCS1090

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1032RemoveRedundantParenthesesTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.RemoveRedundantParentheses;

        public override DiagnosticAnalyzer Analyzer { get; } = new RemoveRedundantParenthesesAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new RemoveRedundantParenthesesCodeFixProvider();

        [Fact]
        public async Task Test_Argument()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(object x)
    {
        M([|(|]x));
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
");
        }

        [Fact]
        public async Task Test_AttributeArgument()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

[Obsolete([|(|]""""))]
class C
{
}
", @"
using System;

[Obsolete("""")]
class C
{
}
");
        }

        [Fact]
        public async Task Test_ReturnExpression()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    object M()
    {
        return [|(|]null);
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
");
        }

        [Fact]
        public async Task Test_YieldReturnExpression()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;

class C
{
    IEnumerable<object> M()
    {
        yield return [|(|]null);
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
");
        }

        [Fact]
        public async Task Test_ExpressionBody()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    object M() => [|(|]null);
}
", @"
class C
{
    object M() => null;
}
");
        }

        [Fact]
        public async Task Test_AwaitExpression()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System.Threading.Tasks;

class C
{
    async Task FooAsync()
    {
        await [|(|]FooAsync());
        await [|(|](Task)FooAsync());
    }
}
", @"
using System.Threading.Tasks;

class C
{
    async Task FooAsync()
    {
        await FooAsync();
        await (Task)FooAsync();
    }
}
");
        }

        [Theory]
        [InlineData("while ([|(|]true)) { }", "while (true) { }")]
        [InlineData("do { } while ([|(|]true));", "do { } while (true);")]
        [InlineData("using ([|(|](IDisposable)null)) { }", "using ((IDisposable)null) { }")]
        [InlineData("lock ([|(|]s)) { }", "lock (s) { }")]
        [InlineData("if ([|(|]true)) { }", "if (true) { }")]
        [InlineData("switch ([|(|]true)) { default: break; }", "switch (true) { default: break; }")]
        [InlineData(@"M([|(|]""""));", @"M("""");")]
        [InlineData("var arr = new string[] { [|(|]null) };", "var arr = new string[] { null };")]
        [InlineData("var items = new List<string>() { [|(|]null) };", "var items = new List<string>() { null };")]
        [InlineData(@"s = $""{[|(|]"""")}"";", @"s = $""{""""}"";")]
        [InlineData("[|(|]i) = (0);", "i = (0);")]
        [InlineData("[|(|]i) += (0);", "i += (0);")]
        [InlineData("[|(|]i) -= (0);", "i -= (0);")]
        [InlineData("[|(|]i) *= (0);", "i *= (0);")]
        [InlineData("[|(|]i) /= (0);", "i /= (0);")]
        [InlineData("[|(|]i) %= (0);", "i %= (0);")]
        [InlineData("[|(|]i) &= (0);", "i &= (0);")]
        [InlineData("[|(|]i) ^= (0);", "i ^= (0);")]
        [InlineData("[|(|]i) |= (0);", "i |= (0);")]
        [InlineData("[|(|]i) <<= (0);", "i <<= (0);")]
        [InlineData("[|(|]i) >>= (0);", "i >>= (0);")]
        public async Task Test_Statement(string fromData, string toData)
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Collections.Generic;

class C
{
    void M(string s)
    {
        int i = 0;

        [||]
    }
}
", fromData, toData);
        }

        [Theory]
        [InlineData("f = ![|(|]f);", "f = !f;")]
        [InlineData(@"f = ![|(|]s.StartsWith(""""));", @"f = !s.StartsWith("""");")]
        [InlineData("f = ![|(|]foo.Value);", "f = !foo.Value;")]
        [InlineData("f = ![|(|]foo[0]);", "f = !foo[0];")]
        public async Task Test_LogicalNot(string fromData, string toData)
        {
            await VerifyDiagnosticAndFixAsync(@"
class Foo
{
    void M()
    {
        bool f = false;
        string s = null;
        var foo = new Foo();

        [||]
    }

    public bool Value { get; }

    public bool this[int i]
    {
        get { return i == 0; }
    }
}
", fromData, toData);
        }

        [Fact]
        public async Task TestNoDiagnostic_AssignmentInInitializer()
        {
            await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;

class C
{
    void M()
    {
        string x;
        var items = new List<string>() { (x = ""x"") };    
    }
}
");
        }

        [Fact]
        public async Task TestNoDiagnostic_ConditionalExpressionInInterpolatedString()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
            string s = $""{ ((true) ? ""a"" : ""b"")}"";
    }
}
");
        }

        [Fact]
        public async Task TestNoDiagnostic_AssignmentInAwaitExpression()
        {
            await VerifyNoDiagnosticAsync(@"
using System;
using System.Threading.Tasks;

class C
{
    async Task FooAsync(Task task) => await (task = Task.Run(default(Action)));
}
");
        }

        [Fact]
        public async Task TestNoDiagnostic_ForEach()
        {
            await VerifyNoDiagnosticAsync(@"
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
");
        }
    }
}
