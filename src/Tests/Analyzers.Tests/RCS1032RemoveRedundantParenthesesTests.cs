// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1032RemoveRedundantParenthesesTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.RemoveRedundantParentheses;

        public override DiagnosticAnalyzer Analyzer { get; } = new RemoveRedundantParenthesesAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new RemoveRedundantParenthesesCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantParentheses)]
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

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantParentheses)]
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

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantParentheses)]
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

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantParentheses)]
        public async Task Test_ReturnExpression_NoSpace()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    object M()
    {
        return[|(|]null);
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

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantParentheses)]
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

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantParentheses)]
        public async Task Test_YieldReturnExpression_NoSpace()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;

class C
{
    IEnumerable<object> M()
    {
        yield return[|(|]null);
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

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantParentheses)]
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

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantParentheses)]
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

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantParentheses)]
        public async Task Test_AwaitExpression_NoSpace()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System.Threading.Tasks;

class C
{
    async Task FooAsync()
    {
        await[|(|]FooAsync());
        await[|(|](Task)FooAsync());
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

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantParentheses)]
        public async Task Test_ArrayRankSpecifier()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        var arr = new object[[|(|]0)];
    }
}
", @"
class C
{
    void M()
    {
        var arr = new object[0];
    }
}
");
        }

        [Theory, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantParentheses)]
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
        [InlineData("[|(|]i) = [|(|]0);", "i = 0;")]
        [InlineData("[|(|]i) += [|(|]0);", "i += 0;")]
        [InlineData("[|(|]i) -= [|(|]0);", "i -= 0;")]
        [InlineData("[|(|]i) *= [|(|]0);", "i *= 0;")]
        [InlineData("[|(|]i) /= [|(|]0);", "i /= 0;")]
        [InlineData("[|(|]i) %= [|(|]0);", "i %= 0;")]
        [InlineData("[|(|]i) &= [|(|]0);", "i &= 0;")]
        [InlineData("[|(|]i) ^= [|(|]0);", "i ^= 0;")]
        [InlineData("[|(|]i) |= [|(|]0);", "i |= 0;")]
        [InlineData("[|(|]i) <<= [|(|]0);", "i <<= 0;")]
        [InlineData("[|(|]i) >>= [|(|]0);", "i >>= 0;")]
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

        [Theory, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantParentheses)]
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

        [Theory, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantParentheses)]
        [InlineData("[|(|]f) == [|(|]true)", "f == true")]
        [InlineData("[|(|]f) != [|(|]true)", "f != true")]
        public async Task Test_EqualsNotEquals(string fromData, string toData)
        {
            await VerifyDiagnosticAndFixAsync(@"
class Foo
{
    void M()
    {
        bool f = false;

        if ([||]) { }
    }
}
", fromData, toData);
        }

        [Theory, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantParentheses)]
        [InlineData("[|(|]i) > [|(|]0)", "i > 0")]
        [InlineData("[|(|]i) >= [|(|]0)", "i >= 0")]
        [InlineData("[|(|]i) < [|(|]0)", "i < 0")]
        [InlineData("[|(|]i) <= [|(|]0)", "i <= 0")]
        public async Task Test_GreaterThanLessThan(string fromData, string toData)
        {
            await VerifyDiagnosticAndFixAsync(@"
class Foo
{
    void M()
    {
        int i = 0;

        if ([||]) { }
    }
}
", fromData, toData);
        }

        [Theory, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantParentheses)]
        [InlineData("[|(|]i) * [|(|]0)", "i * 0")]
        [InlineData("[|(|]i) % [|(|]0)", "i % 0")]
        [InlineData("[|(|]i) / [|(|]0)", "i / 0")]
        [InlineData("[|(|]i) + [|(|]0)", "i + 0")]
        [InlineData("[|(|]i) - [|(|]0)", "i - 0")]
        [InlineData("[|(|]i) << [|(|]0)", "i << 0")]
        [InlineData("[|(|]i) >> [|(|]0)", "i >> 0")]
        [InlineData("[|(|]i) & [|(|]0)", "i & 0")]
        [InlineData("[|(|]i) ^ [|(|]0)", "i ^ 0")]
        [InlineData("[|(|]i) | [|(|]0)", "i | 0")]
        [InlineData("[|(|]f) && [|(|]f2)", "f && f2")]
        [InlineData("[|(|]f) || [|(|]f2)", "f || f2")]
        public async Task Test_BinaryExpression(string fromData, string toData)
        {
            await VerifyDiagnosticAndFixAsync(@"
class Foo
{
    void M()
    {
        bool f = false;
        bool f2 = false;
        int i = 0;

        var x = [||];
    }
}
", fromData, toData);
        }

        [Theory, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantParentheses)]
        [InlineData("[|(|]i * 0) * i", "i * 0 * i")]
        [InlineData("[|(|]i % 0) % i", "i % 0 % i")]
        [InlineData("[|(|]i / 0) / i", "i / 0 / i")]
        [InlineData("[|(|]i + 0) + i", "i + 0 + i")]
        [InlineData("[|(|]i - 0) - i", "i - 0 - i")]
        [InlineData("[|(|]i << 0) << i", "i << 0 << i")]
        [InlineData("[|(|]i >> 0) >> i", "i >> 0 >> i")]
        [InlineData("[|(|]i & 0) & i", "i & 0 & i")]
        [InlineData("[|(|]i ^ 0) ^ i", "i ^ 0 ^ i")]
        [InlineData("[|(|]i | 0) | i", "i | 0 | i")]
        [InlineData("[|(|]f && f2) && f", "f && f2 && f")]
        [InlineData("[|(|]f || f2) || f", "f || f2 || f")]
        public async Task Test_BinaryExpressionChain(string fromData, string toData)
        {
            await VerifyDiagnosticAndFixAsync(@"
class Foo
{
    void M()
    {
        bool f = false;
        bool f2 = false;
        int i = 0;

        var x = [||];
    }
}
", fromData, toData);
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantParentheses)]
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

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantParentheses)]
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

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantParentheses)]
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

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantParentheses)]
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

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantParentheses)]
        public async Task TestNoDiagnostic_BinaryExpressionChain_ParenthesizedRight()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        int i = 0;

        i = i * (0 * i);
        i = i % (0 % i);
        i = i / (0 / i);
        i = i + (0 + i);
        i = i - (0 - i);
        i = i << (0 << i);
        i = i >> (0 >> i);
        i = i & (0 & i);
        i = i ^ (0 ^ i);
        i = i | (0 | i);

        bool f = false;

        f = f && (true && f);
        f = f || (true || f);
    }
}
");
        }
    }
}
