// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Formatting.CodeFixes.LineIsTooLong;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests;

public class RCS0056LineIsTooLongTests : AbstractCSharpDiagnosticVerifier<LineIsTooLongAnalyzer, LineIsTooLongCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.LineIsTooLong;

    public override CSharpTestOptions Options => base.Options.AddConfigOption(ConfigOptionKeys.MaxLineLength, "125");

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_ExpressionBody_AddNewLineBeforeArrow()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
[|    string M(object ppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppp) => null;|]
}
",
@"
class C
{
    string M(object ppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppp)
        => null;
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_ExpressionBody_AddNewLineAfterArrow()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
[|    string M(object pppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppp) => null;|]
}
",
@"
class C
{
    string M(object pppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppp) =>
        null;
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrowTokenNewLine, "after"));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_ParameterList()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
[|    void M(object x, object y, object z, object xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx)|]
    {
    }
}
",
@"
class C
{
    void M(
        object x,
        object y,
        object z,
        object xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx)
    {
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrowTokenNewLine, "after"));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_ArgumentList()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(
        object x,
        object y,
        object z,
        object xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx)
    {
[|        M(x, y, z, xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx);|]
    }
}
",
@"
class C
{
    void M(
        object x,
        object y,
        object z,
        object xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx)
    {
        M(
            x,
            y,
            z,
            xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_ArgumentList_PreferOuterArgumentList()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string M(
        object x,
        object y,
        object z,
        string xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx)
    {
[|        return M(x, y, z, M(x, y, z, xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx));|]
    }
}
",
@"
class C
{
    string M(
        object x,
        object y,
        object z,
        string xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx)
    {
        return M(
            x,
            y,
            z,
            M(x, y, z, xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx));
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_PreferArgumentListOverCallChain()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    C M(
        object x,
        object y,
        object zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz)
    {
[|        return M(x, y, zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz).M(x, y, zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz);|]
    }
}
",
@"
class C
{
    C M(
        object x,
        object y,
        object zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz)
    {
        return M(x, y, zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz).M(
            x,
            y,
            zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_PreferArgumentListOverCallChain2()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    C M(
        string x,
        string yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy)
    {
[|        return M(x.ToString(), yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy.ToString().ToString().ToString());|]
    }
}
",
@"
class C
{
    C M(
        string x,
        string yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy)
    {
        return M(
            x.ToString(),
            yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy.ToString().ToString().ToString());
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_PreferArgumentListOverCallChain_WhenLeftIsSimpleName()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        var items = new List<string>();

[|        foreach ((string ffff, string gggg) item in items.Join(items, ffff => ffff, ffff => ffff, (ffff, gggg) => (ffff, gggg)))|]
        {
        }
    }
}
",
@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        var items = new List<string>();

        foreach ((string ffff, string gggg) item in items.Join(
            items,
            ffff => ffff,
            ffff => ffff,
            (ffff, gggg) => (ffff, gggg)))
        {
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_PreferArgumentListOverCallChain_WhenLeftIsCastExpression()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        var items = new List<string>();

[|        foreach ((string ffff, string gggg) item in ((IEnumerable<string>)items).Join(items, ffff => ffff, ffff => ffff, (ffff, gggg) => (ffff, gggg)))|]
        {
        }
    }
}
",
@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        var items = new List<string>();

        foreach ((string ffff, string gggg) item in ((IEnumerable<string>)items).Join(
            items,
            ffff => ffff,
            ffff => ffff,
            (ffff, gggg) => (ffff, gggg)))
        {
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_PreferArgumentListOverBinaryExpression()
    {
        await VerifyDiagnosticAndFixAsync("""
using System;

class C
{
    void M()
    {
[|        if (string.Compare("xxxxxxxxxxxxxxxxxxxxxx", 0, "xxxxxxxxxxxxxxxxxxxxxx", 1 + 1, 0, StringComparison.OrdinalIgnoreCase) == 0)|]
        {
        }
    }
}
""",
"""
using System;

class C
{
    void M()
    {
        if (string.Compare(
            "xxxxxxxxxxxxxxxxxxxxxx",
            0,
            "xxxxxxxxxxxxxxxxxxxxxx",
            1 + 1,
            0,
            StringComparison.OrdinalIgnoreCase) == 0)
        {
        }
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_PreferArgumentListOverBinaryExpression2()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        string[] items = null;

[|        items[items.Length - 1] = items[items.Length - 1].Mxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx(items.Skip(items.Length));|]
    }
}

static class E
{
    public static string Mxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx(this string s, IEnumerable<string> items) => null;
}
",
@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        string[] items = null;

        items[items.Length - 1] = items[items.Length - 1].Mxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx(
            items.Skip(items.Length));
    }
}

static class E
{
    public static string Mxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx(this string s, IEnumerable<string> items) => null;
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_BracketedArgumentList()
    {
        await VerifyDiagnosticAndFixAsync("""
class C
{
    string M()
    {
        var c = new C();

[|        return c["xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"];|]
    }

    string this[string p] => null;
}
""",
"""
class C
{
    string M()
    {
        var c = new C();

        return c[
            "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"];
    }

    string this[string p] => null;
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_CallChain()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    string Xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx()
    {
[|        return Xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx().ToString().ToString().ToString()|]
            .ToString();
    }
}
",
@"
class C
{
    string Xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx()
    {
        return Xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx().ToString()
            .ToString()
            .ToString()
            .ToString();
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_CallChain_SimpleMemberAccess()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    int M()
    {
[|        return C.Pxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.Length;|]
    }

    static string Pxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        => default;
}
",
@"
class C
{
    int M()
    {
        return C.Pxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            .Length;
    }

    static string Pxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        => default;
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_PreferCallChainOverArgumentList()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx = null;

[|        var x = xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.Length.ToString(xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.ToString());|]
    }
}
",
@"
class C
{
    void M()
    {
        string xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx = null;

        var x = xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.Length
            .ToString(xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.ToString());
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_PreferCallChainOverArgumentList2()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx = null;

[|        var x = xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.Length.ToString(xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.ToString())|]
            .ToString();
    }
}
",
@"
class C
{
    void M()
    {
        string xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx = null;

        var x = xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.Length
            .ToString(xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.ToString())
            .ToString();
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_PreferCallChainOverArgumentList3()
    {
        await VerifyDiagnosticAndFixAsync("""
class C
{
    void M()
    {
        string x = null;

        if (
[|                    x.Length.ToString("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx")?.Length == 0)|]
        {
        }
    }
}
""",
"""
class C
{
    void M()
    {
        string x = null;

        if (
                    x.Length.ToString("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx")?
                        .Length == 0)
        {
        }
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_BinaryExpression()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool xxxxxxxxxxxxxxxxxxxxxxxxxx = false;

[|        if (xxxxxxxxxxxxxxxxxxxxxxxxxx && xxxxxxxxxxxxxxxxxxxxxxxxxx && xxxxxxxxxxxxxxxxxxxxxxxxxx && xxxxxxxxxxxxxxxxxxxxxxxxxx && xxxxxxxxxxxxxxxxxxxxxxxxxx)|]
        {
        }
    }
}
",
@"
class C
{
    void M()
    {
        bool xxxxxxxxxxxxxxxxxxxxxxxxxx = false;

        if (xxxxxxxxxxxxxxxxxxxxxxxxxx && xxxxxxxxxxxxxxxxxxxxxxxxxx && xxxxxxxxxxxxxxxxxxxxxxxxxx
            && xxxxxxxxxxxxxxxxxxxxxxxxxx
            && xxxxxxxxxxxxxxxxxxxxxxxxxx)
        {
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_InitializerExpression()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        string sssssssssssssssssssssssssssss = null;
[|        var arr = new string[] { sssssssssssssssssssssssssssss, sssssssssssssssssssssssssssss, sssssssssssssssssssssssssssss };|]
    }
}
",
@"
class C
{
    void M()
    {
        string sssssssssssssssssssssssssssss = null;
        var arr = new string[] {
            sssssssssssssssssssssssssssss,
            sssssssssssssssssssssssssssss,
            sssssssssssssssssssssssssssss };
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_PropertyInitializer()
    {
        await VerifyDiagnosticAndFixAsync("""
class C
{
[|    string P { get; } = "".ToString().ToString().ToString().ToString().ToString().ToString().ToString().ToString().ToString();|]
}
""",
"""
class C
{
    string P { get; }
        = "".ToString().ToString().ToString().ToString().ToString().ToString().ToString().ToString().ToString();
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_FieldInitializer()
    {
        await VerifyDiagnosticAndFixAsync("""
class C
{
[|    private string _f = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";|]
}
""",
"""
class C
{
    private string _f
        = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_LocalDeclaration()
    {
        await VerifyDiagnosticAndFixAsync("""
class C
{
    void M()
    {
[|        var xxxxxxxxxxxxxxxx = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";|]
    }
}
""",
"""
class C
{
    void M()
    {
        var xxxxxxxxxxxxxxxx
            = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_Assignment()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    static string Mxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx()
    {
[|        string x = Mxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx();|]

        return null;
    }
}
",
@"
class C
{
    static string Mxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx()
    {
        string x
            = Mxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx();

        return null;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_Assignment2()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    static string foo()
    {
[|                                                                                                              string x = foo();|]

        return null;
    }
}
",
@"
class C
{
    static string foo()
    {
                                                                                                              string x
                                                                                                                  = foo();

        return null;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_AttributeList()
    {
        await VerifyDiagnosticAndFixAsync("""
using System;

class C
{
[|    [Obsolete("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"), Foo]|]
    void M()
    {
    }
}

class FooAttribute : Attribute
{
}
""",
"""
using System;

class C
{
    [Obsolete("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"),
        Foo]
    void M()
    {
    }
}

class FooAttribute : Attribute
{
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_AttributeArgumentList()
    {
        await VerifyDiagnosticAndFixAsync("""
using System;

class C
{
[|    [Obsolete("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", error: false)]|]
    void M()
    {
    }
}

class FooAttribute : Attribute
{
}
""",
"""
using System;

class C
{
    [Obsolete(
        "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
        error: false)]
    void M()
    {
    }
}

class FooAttribute : Attribute
{
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_ConditionalExpression()
    {
        await VerifyDiagnosticAndFixAsync("""
class C
{
    void M()
    {
        bool f = false;

[|        var x = f ? "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx" : "";|]
    }
}
""",
"""
class C
{
    void M()
    {
        bool f = false;

        var x = f
            ? "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
            : "";
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_PreferConditionalExpressionOverCallChain()
    {
        await VerifyDiagnosticAndFixAsync("""
class C
{
    void M()
    {
        string s = null;
        bool f = false;

[|        var x = f ? 0.ToString("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx") : 1.ToString("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");|]
    }
}
""",
"""
class C
{
    void M()
    {
        string s = null;
        bool f = false;

        var x = f
            ? 0.ToString("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx")
            : 1.ToString("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_ForStatement()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;

class C
{
    void M()
    {
        var xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx = new List<string>();

[|        for (int i = 0; i < xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.Count; i++)|]
        {
        }
    }
}
",
@"
using System.Collections.Generic;

class C
{
    void M()
    {
        var xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx = new List<string>();

        for (
            int i = 0;
            i < xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.Count;
            i++)
        {
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_BaseList()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Collections;
using System.Collections.Generic;

namespace N
{
[|    interface IC : Xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx, IEnumerable, IEnumerable<object>|]
    {
    }

    interface Xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    {
    }
}
",
@"
using System.Collections;
using System.Collections.Generic;

namespace N
{
    interface IC :
        Xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx,
        IEnumerable,
        IEnumerable<object>
    {
    }

    interface Xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    {
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_BaseList_Constraint()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System.Collections;
using System.Collections.Generic;

namespace N
{
[|    interface IC<T> : Xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx, IEnumerable, IEnumerable<T> where T : IComparer|]
    {
    }

    interface Xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    {
    }
}
",
@"
using System.Collections;
using System.Collections.Generic;

namespace N
{
    interface IC<T> :
        Xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx,
        IEnumerable,
        IEnumerable<T>
        where T : IComparer
    {
    }

    interface Xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    {
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_AnonymousObject()
    {
        await VerifyDiagnosticAndFixAsync("""
using System.Collections.Generic;
using System.Linq;

public sealed record User(int Id, string Email, bool Active, bool TwoFactorEnabled);

public static class C
{
    public static int Count(IEnumerable<User> users)
    {
        var rows = users
            .Where(a => a.Active)
[|            .Select(a => new { a.Id, a.Email, Upper = a.Email.ToUpperInvariant(), IsSecured = a.Active, a.TwoFactorEnabled })|]
            .ToList();
        return rows.Count;
    }
}
""",
"""
using System.Collections.Generic;
using System.Linq;

public sealed record User(int Id, string Email, bool Active, bool TwoFactorEnabled);

public static class C
{
    public static int Count(IEnumerable<User> users)
    {
        var rows = users
            .Where(a => a.Active)
            .Select(
                a => new {
                    a.Id,
                    a.Email,
                    Upper = a.Email.ToUpperInvariant(),
                    IsSecured = a.Active,
                    a.TwoFactorEnabled })
            .ToList();
        return rows.Count;
    }
}
""", options: Options.SetConfigOption(ConfigOptionKeys.MaxLineLength, "120"));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_AnonymousObject_PreferOuterAnonymousObject()
    {
        await VerifyDiagnosticAndFixAsync("""
public static class C
{
[|    public static object Order() => new { Id = 1, Lines = new[] { new { Sku = "SKU-0001", Quantity = 2, UnitPrice = 10.5m, Discount = 0m } }, Total = 21m };|]
}
""",
"""
public static class C
{
    public static object Order() => new {
        Id = 1,
        Lines = new[] { new { Sku = "SKU-0001", Quantity = 2, UnitPrice = 10.5m, Discount = 0m } },
        Total = 21m };
}
""", options: Options
            .SetConfigOption(ConfigOptionKeys.MaxLineLength, "120")
            .AddConfigOption(ConfigOptionKeys.ArrowTokenNewLine, "after"));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_ParameterList_OpenParenOnPreviousLine()
    {
        await VerifyDiagnosticAndFixAsync("""
public static class C
{
    public static int Sum(
[|        int firstValue, int secondValue, int thirdValue, int fourthValue, int fifthValue, int sixthValue, int seventhValue)|]
    {
        return firstValue + secondValue;
    }
}
""",
"""
public static class C
{
    public static int Sum(
        int firstValue,
        int secondValue,
        int thirdValue,
        int fourthValue,
        int fifthValue,
        int sixthValue,
        int seventhValue)
    {
        return firstValue + secondValue;
    }
}
""", options: Options.SetConfigOption(ConfigOptionKeys.MaxLineLength, "120"));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_ArgumentList_OpenParenOnPreviousLine()
    {
        await VerifyDiagnosticAndFixAsync("""
using System.Linq;

class C
{
    int M(object a, object b) => 0;

    void M2(string[] items, object aaaaaaaaaaaaaaa)
    {
        M(
[|            aaaaaaaaaaaaaaa, items.Where(f => f.Length > 0).Select(f => f.ToUpperInvariant()).OrderBy(f => f).ToArray());|]
    }
}
""",
"""
using System.Linq;

class C
{
    int M(object a, object b) => 0;

    void M2(string[] items, object aaaaaaaaaaaaaaa)
    {
        M(
            aaaaaaaaaaaaaaa,
            items.Where(f => f.Length > 0).Select(f => f.ToUpperInvariant()).OrderBy(f => f).ToArray());
    }
}
""", options: Options.SetConfigOption(ConfigOptionKeys.MaxLineLength, "120"));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_ObjectInitializer_OpenBraceOnPreviousLine()
    {
        await VerifyDiagnosticAndFixAsync("""
class P
{
    public string Name { get; set; }
    public string Value { get; set; }
}

class C
{
    P M() => new P
    {
[|        Name = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", Value = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",|]
    };
}
""",
"""
class P
{
    public string Name { get; set; }
    public string Value { get; set; }
}

class C
{
    P M() => new P
    {
        Name = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
        Value = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
    };
}
""", options: Options.SetConfigOption(ConfigOptionKeys.MaxLineLength, "120"));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_Assignment_PreferOverSimpleMemberAccess()
    {
        await VerifyDiagnosticAndFixAsync("""
using System.Collections.Generic;

public static class StatusCodes
{
    public const int Status409Conflict = 409;
}

public static class C
{
    public static Dictionary<string, int> Codes() => new()
    {
[|        ["the-request-conflicts-with-the-current-state-of-the-resource-reload-it-and-retry"] = StatusCodes.Status409Conflict,|]
    };
}
""",
"""
using System.Collections.Generic;

public static class StatusCodes
{
    public const int Status409Conflict = 409;
}

public static class C
{
    public static Dictionary<string, int> Codes() => new()
    {
        ["the-request-conflicts-with-the-current-state-of-the-resource-reload-it-and-retry"] =
            StatusCodes.Status409Conflict,
    };
}
""", options: Options
            .SetConfigOption(ConfigOptionKeys.MaxLineLength, "120")
            .AddConfigOption(ConfigOptionKeys.EqualsTokenNewLine, "after"));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_LocalDeclaration_PreferOverSimpleMemberAccess()
    {
        await VerifyDiagnosticAndFixAsync("""
static class Constants
{
    public const string SomeVeryLongConstantNameThatDescribesSomethingImportant = "x";
}

class C
{
    void M()
    {
[|        string someVeryLongLocalVariableNameForTheValue = Constants.SomeVeryLongConstantNameThatDescribesSomethingImportant;|]
    }
}
""",
"""
static class Constants
{
    public const string SomeVeryLongConstantNameThatDescribesSomethingImportant = "x";
}

class C
{
    void M()
    {
        string someVeryLongLocalVariableNameForTheValue =
            Constants.SomeVeryLongConstantNameThatDescribesSomethingImportant;
    }
}
""", options: Options
            .SetConfigOption(ConfigOptionKeys.MaxLineLength, "120")
            .AddConfigOption(ConfigOptionKeys.EqualsTokenNewLine, "after"));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_IsExpression_PreferOverSimpleMemberAccess()
    {
        await VerifyDiagnosticAndFixAsync("""
class P
{
    public object AnotherVeryLongPropertyNameThatIsHereJustToMakeTheLineLongEnoughForSure { get; }
}

class C
{
    P SomeVeryLongPropertyName { get; }

    void M()
    {
[|        if (this.SomeVeryLongPropertyName.AnotherVeryLongPropertyNameThatIsHereJustToMakeTheLineLongEnoughForSure is string)|]
        {
        }
    }
}
""",
"""
class P
{
    public object AnotherVeryLongPropertyNameThatIsHereJustToMakeTheLineLongEnoughForSure { get; }
}

class C
{
    P SomeVeryLongPropertyName { get; }

    void M()
    {
        if (this.SomeVeryLongPropertyName.AnotherVeryLongPropertyNameThatIsHereJustToMakeTheLineLongEnoughForSure
            is string)
        {
        }
    }
}
""", options: Options.SetConfigOption(ConfigOptionKeys.MaxLineLength, "120"));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_Assignment_PreferOverMemberBinding()
    {
        await VerifyDiagnosticAndFixAsync("""
using System.Collections.Generic;

public class Settings
{
    public int Status409ConflictValue { get; }
}

public static class C
{
    public static Dictionary<string, int?> Codes(Settings settings) => new()
    {
[|        ["the-request-conflicts-with-the-current-state-of-the-resource-reload-it-and-retryyyyyy"] = settings?.Status409ConflictValue,|]
    };
}
""",
"""
using System.Collections.Generic;

public class Settings
{
    public int Status409ConflictValue { get; }
}

public static class C
{
    public static Dictionary<string, int?> Codes(Settings settings) => new()
    {
        ["the-request-conflicts-with-the-current-state-of-the-resource-reload-it-and-retryyyyyy"] =
            settings?.Status409ConflictValue,
    };
}
""", options: Options
            .SetConfigOption(ConfigOptionKeys.MaxLineLength, "120")
            .AddConfigOption(ConfigOptionKeys.EqualsTokenNewLine, "after"));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_ParameterList_RestOfLineIsWrappedInNextStep()
    {
        await VerifyDiagnosticAndFixAsync("""
class B
{
    public B(string a, string b)
    {
    }
}

class D : B
{
[|    public D(string firstParameterName, string secondParameterName, string thirdeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee) : base(firstParameterName, secondParameterName)|]
    {
    }
}
""",
"""
class B
{
    public B(string a, string b)
    {
    }
}

class D : B
{
    public D(
        string firstParameterName,
        string secondParameterName,
        string thirdeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee) : base(
            firstParameterName,
            secondParameterName)
    {
    }
}
""", options: Options.SetConfigOption(ConfigOptionKeys.MaxLineLength, "120"));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_ArgumentList_LongStringLiteralAtLimit()
    {
        await VerifyDiagnosticAndFixAsync("""
class C
{
    void WriteLine(string s, int v)
    {
    }

    void M(int count)
    {
[|        WriteLine($"{count} {((count == 1) ? "document" : "documents")} formatted xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", 1);|]
    }
}
""",
"""
class C
{
    void WriteLine(string s, int v)
    {
    }

    void M(int count)
    {
        WriteLine(
            $"{count} {((count == 1) ? "document" : "documents")} formatted xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
            1);
    }
}
""", options: Options.SetConfigOption(ConfigOptionKeys.MaxLineLength, "120"));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_ArgumentList_OpenParenOnPreviousLine_IndentationOfList()
    {
        await VerifyDiagnosticAndFixAsync("""
class C
{
    void M(object a, string b)
    {
        M(
[|            a, "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");|]
    }
}
""",
"""
class C
{
    void M(object a, string b)
    {
        M(
            a,
            "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
    }
}
""", options: Options.SetConfigOption(ConfigOptionKeys.MaxLineLength, "120"));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_AnonymousObject_OpenBraceOnPreviousLine()
    {
        await VerifyDiagnosticAndFixAsync("""
class C
{
    void M()
    {
        var x = new
        {
[|            Name = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", Value = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",|]
        };
    }
}
""",
"""
class C
{
    void M()
    {
        var x = new
        {
            Name = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
            Value = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
        };
    }
}
""", options: Options.SetConfigOption(ConfigOptionKeys.MaxLineLength, "120"));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_SimpleMemberAccess_PreferOverInnerBracketedArgumentList()
    {
        await VerifyDiagnosticAndFixAsync("""
using System.Collections.Generic;

class P
{
    public string VeryLongPropertyNameNumberOneeeeeeeeeeeeeeeeeeeeeeeeeeeeeee { get; }
}

class C
{
    string M(Dictionary<string, P> dictionaryWithALongName)
    {
[|        return dictionaryWithALongName["some-key-that-is-long-xx"].VeryLongPropertyNameNumberOneeeeeeeeeeeeeeeeeeeeeeeeeeeeeee;|]
    }
}
""",
"""
using System.Collections.Generic;

class P
{
    public string VeryLongPropertyNameNumberOneeeeeeeeeeeeeeeeeeeeeeeeeeeeeee { get; }
}

class C
{
    string M(Dictionary<string, P> dictionaryWithALongName)
    {
        return dictionaryWithALongName["some-key-that-is-long-xx"]
            .VeryLongPropertyNameNumberOneeeeeeeeeeeeeeeeeeeeeeeeeeeeeee;
    }
}
""", options: Options.SetConfigOption(ConfigOptionKeys.MaxLineLength, "120"));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_CallChain_ParenthesizedInvocation_PreferOverComparison()
    {
        await VerifyDiagnosticAndFixAsync("""
class IndentationAnalysis
{
    public int IndentSize { get; }
}

class Context
{
    public object GetConfigOptions() => null;
}

class C
{
    static IndentationAnalysis AnalyzeIndentation(object expression, object options) => null;

    static bool M(bool f, IndentationAnalysis indentationAnalysis, object expressionxxxxxxxxxx, Context context)
    {
        return f
[|            && (indentationAnalysis ?? AnalyzeIndentation(expressionxxxxxxxxxx, context.GetConfigOptions())).IndentSize > 0;|]
    }
}
""",
"""
class IndentationAnalysis
{
    public int IndentSize { get; }
}

class Context
{
    public object GetConfigOptions() => null;
}

class C
{
    static IndentationAnalysis AnalyzeIndentation(object expression, object options) => null;

    static bool M(bool f, IndentationAnalysis indentationAnalysis, object expressionxxxxxxxxxx, Context context)
    {
        return f
            && (indentationAnalysis ?? AnalyzeIndentation(expressionxxxxxxxxxx, context.GetConfigOptions()))
                .IndentSize > 0;
    }
}
""", options: Options.SetConfigOption(ConfigOptionKeys.MaxLineLength, "120"));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task Test_ArgumentList_PreferOverInnerCallChain()
    {
        await VerifyDiagnosticAndFixAsync("""
using System.Linq;

class C
{
    int M(object a, object b) => 0;

    void M2(string[] itemsxxxxxxxxxxxxxxxxx, object aaaaaaaaaaaaaaa)
    {
[|        M(aaaaaaaaaaaaaaa, itemsxxxxxxxxxxxxxxxxx.Where(f => f.Length > 0).Select(f => f.ToUpperInvariant()).OrderBy(f => f).ToArray());|]
    }
}
""",
"""
using System.Linq;

class C
{
    int M(object a, object b) => 0;

    void M2(string[] itemsxxxxxxxxxxxxxxxxx, object aaaaaaaaaaaaaaa)
    {
        M(
            aaaaaaaaaaaaaaa,
            itemsxxxxxxxxxxxxxxxxx.Where(f => f.Length > 0).Select(f => f.ToUpperInvariant()).OrderBy(f => f)
                .ToArray());
    }
}
""", options: Options.SetConfigOption(ConfigOptionKeys.MaxLineLength, "120"));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task TestNoFix_ArrayInitializer_OpenBraceOnPreviousLine()
    {
        await VerifyDiagnosticAndNoFixAsync("""
class C
{
    static readonly byte[] Table =
    {
        0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
[|        0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18,|]
        0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
    };
}
""", options: Options.SetConfigOption(ConfigOptionKeys.MaxLineLength, "120"));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task TestNoFix_BracketedArgumentList_TooLongAfterWrapping()
    {
        await VerifyDiagnosticAndNoFixAsync("""
using System.Collections.Generic;

public static class C
{
    public static Dictionary<string, string> Settings() => new()
    {
[|        ["Jwt:SigningKey"] = "a-test-signing-key-that-is-long-enough-for-hmac-sha256-and-then-some-more-characters-0123456789-abcdef-0123456789",|]
    };
}
""", options: Options
            .SetConfigOption(ConfigOptionKeys.MaxLineLength, "120")
            .AddConfigOption(ConfigOptionKeys.EqualsTokenNewLine, "after"));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task TestNoFix_ExpressionBody_TooLongAfterWrapping()
    {
        await VerifyDiagnosticAndNoFixAsync(@"
class C
{
[|    string Foooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo() => null;|]
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task TestNoFix_ExpressionBody_TooLongAfterWrapping2()
    {
        await VerifyDiagnosticAndNoFixAsync(@"
class C
{
[|    string Fooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo() => null;|]
}
", options: Options.AddConfigOption(ConfigOptionKeys.ArrowTokenNewLine, "after"));
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task TestNoFix_ExpressionBody_AlreadyWrapped()
    {
        await VerifyNoDiagnosticAsync(
"""
class C
{
    string M(object p)
        => "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task TestNoFix_ExpressionBody_AlreadyWrapped2()
    {
        await VerifyNoDiagnosticAsync("""
class C
{
    string M(object p) =>
        "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task TestNoDiagnostic_Banner()
    {
        await VerifyNoDiagnosticAsync(@"//xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

class C
{
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task TestNoDiagnostic_DoNotWrapNameof()
    {
        await VerifyDiagnosticAndNoFixAsync(@"
class C
{
    string M(string xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx)
    {
[|        return nameof(xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx);|]
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task TestNoDiagnostic_DocumentationComment()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    /// <summary>
    /// xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    /// </summary>
    void M()
    {
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task TestNoDiagnostic_SingleLineComment()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    
    // xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    void M()
    {
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task TestNoDiagnostic_StringLiteral()
    {
        await VerifyNoDiagnosticAsync("""
class C
{
    void M()
    {
        string s = @"
xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
";

        s =
@"xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
;
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task TestNoDiagnostic_ThisExpression_BaseExpression()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C : B
{

    void M()
    {
        var c = new C();

[|        var x = this.Pxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx;|]

[|        var y = base.Pxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx;|]
    }
}

class B
{
    public C Pxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        { get; }
}
"
,
@"
class C : B
{

    void M()
    {
        var c = new C();

        var x
            = this.Pxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx;

        var y
            = base.Pxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx;
    }
}

class B
{
    public C Pxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        { get; }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task TestNoDiagnostic_EnumField()
    {
        await VerifyDiagnosticAndNoFixAsync(@"
using System.Text.RegularExpressions;

class C
{

    void M()
    {
[|        var x =                                                                                               RegexOptions.None;|]
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task TestNoDiagnostic_Namespace()
    {
        await VerifyDiagnosticAndNoFixAsync(@"
using System.Text.RegularExpressions;

class C
{

    void M()
    {
[|        var x =                                                                             System.Text.RegularExpressions.RegexOptions.None;|]
[|        var y =                                                                                        global::System.Text.RegularExpressions.RegexOptions.None;|]
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task TestNoDiagnostic_LongStringLiteral_Argument()
    {
        await VerifyNoDiagnosticAsync("""
class C
{
    static void M(string x, string y)
    {
        C.M(
            "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
            "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task TestNoDiagnostic_LongStringLiteral_AttributeArgument()
    {
        await VerifyNoDiagnosticAsync("""
using System;

class C
{
    [Obsolete(
        "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx")]
    static void M()
    {
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task TestNoDiagnostic_LongStringLiteral_AttributeArgument2()
    {
        await VerifyNoDiagnosticAsync("""
using System;

class C
{
    [My(
        Value = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx")]
    static void M()
    {
    }
}

class MyAttribute : Attribute
{
    public string Value { get; set; }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task TestNoDiagnostic_LongVerbatimStringLiteral()
    {
        await VerifyNoDiagnosticAsync("""
class C
{
    static void M(string x, int y)
    {
        C.M(
            @"
            
            xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
            0);
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task TestNoDiagnostic_LongInterpolatedString()
    {
        await VerifyNoDiagnosticAsync("""
class C
{
    static void M(string x, string y)
    {
        C.M(
            $"xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
            $"xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task TestNoDiagnostic_LongRawInterpolatedString()
    {
        await VerifyNoDiagnosticAsync(""""
class C
{
    static void M(string x, int y)
    {
        C.M(
            $"""
            
               xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            """,
            0);
    }
}
"""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task TestNoDiagnostic_LongRawStringLiteral()
    {
        await VerifyNoDiagnosticAsync(""""
class C
{
    static void M(string x, int y)
    {
        C.M(
            """
            
               xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            """,
            0);
    }
}
"""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.LineIsTooLong)]
    public async Task TestNoDiagnostic_LongRawStringLiteral2()
    {
        await VerifyNoDiagnosticAsync(""""
class C
{
    void M()
    {
        string x = "x";
        var value = $$"""
            xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            {{x}}
            """;
    }
}
"""");
    }
}
