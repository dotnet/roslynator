// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.Analysis.UsePatternMatching;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1220UsePatternMatchingInsteadOfIsAndCastTests : AbstractCSharpDiagnosticVerifier<UsePatternMatchingInsteadOfIsAndCastAnalyzer, UsePatternMatchingInsteadOfIsAndCastCodeFixProvider>
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.UsePatternMatchingInsteadOfIsAndCast;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UsePatternMatchingInsteadOfIsAndCast)]
        public async Task Test_LogicalAndExpression()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    public void M()
    {
        object x = null;
        string s = null;


        if ([|x is string && ((string)x) == s|]) { }
    }
}
", @"
class C
{
    public void M()
    {
        object x = null;
        string s = null;


        if (x is string x2 && (x2) == s) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UsePatternMatchingInsteadOfIsAndCast)]
        public async Task Test_LogicalAndExpression2()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    public void M()
    {
        object x = null;

        if ([|x is string && ((string)x).Equals((string)x)|]) { }
    }
}
", @"
class C
{
    public void M()
    {
        object x = null;

        if (x is string x2 && (x2).Equals(x2)) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UsePatternMatchingInsteadOfIsAndCast)]
        public async Task Test_LogicalAndExpression3()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    private readonly object _f = false;

    public void M()
    {
        object x = null;
        string s = null;

        if ([|_f is string && (string)(_f) == s|]) { }
    }
}
", @"
class C
{
    private readonly object _f = false;

    public void M()
    {
        object x = null;
        string s = null;

        if (_f is string x2 && x2 == s) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UsePatternMatchingInsteadOfIsAndCast)]
        public async Task Test_LogicalAndExpression4()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    private readonly object _f = false;

    public void M()
    {
        object x = null;
        string s = null;

        if ([|this._f is string && (string)this._f == s|]) { }
    }
}
", @"
class C
{
    private readonly object _f = false;

    public void M()
    {
        object x = null;
        string s = null;

        if (this._f is string x2 && x2 == s) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UsePatternMatchingInsteadOfIsAndCast)]
        public async Task Test_LogicalAndExpression5()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    private readonly object _f = false;

    public void M()
    {
        object x = null;
        string s = null;

        if ([|_f is string && (string)(this._f) == s|]) { }
    }
}
", @"
class C
{
    private readonly object _f = false;

    public void M()
    {
        object x = null;
        string s = null;

        if (_f is string x2 && x2 == s) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UsePatternMatchingInsteadOfIsAndCast)]
        public async Task Test_LogicalAndExpression6()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    private readonly object _f = false;

    public void M()
    {
        object x = null;
        string s = null;

        if ([|this._f is string && (string)_f == s|]) { }
    }
}
", @"
class C
{
    private readonly object _f = false;

    public void M()
    {
        object x = null;
        string s = null;

        if (this._f is string x2 && x2 == s) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UsePatternMatchingInsteadOfIsAndCast)]
        public async Task Test_LogicalAndExpression7()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    private readonly object _f = false;

    public void M()
    {
        object x = null;

        if ([|this._f is string && ((string)_f).Equals((string)this._f)|]) { }
    }
}
", @"
class C
{
    private readonly object _f = false;

    public void M()
    {
        object x = null;

        if (this._f is string x2 && (x2).Equals(x2)) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UsePatternMatchingInsteadOfIsAndCast)]
        public async Task Test_LogicalAndExpression_Enum()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    private readonly object _f = false;

    public void M()
    {
        Enum e = null;
        object x = null;

        if ([|this._f is Enum && ((Enum)_f).Equals((Enum)this._f)|]) { }
    }
}
", @"
using System;

class C
{
    private readonly object _f = false;

    public void M()
    {
        Enum e = null;
        object x = null;

        if (this._f is Enum @enum && (@enum).Equals(@enum)) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UsePatternMatchingInsteadOfIsAndCast)]
        public async Task Test_IfStatement1()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    public void M()
    {
        object x = null;
        string s = null;

        if ([|x is string|])
        {
            if (((string)x) == s) { }
        }
    }
}
", @"
class C
{
    public void M()
    {
        object x = null;
        string s = null;

        if (x is string x2)
        {
            if ((x2) == s) { }
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UsePatternMatchingInsteadOfIsAndCast)]
        public async Task Test_IfStatement2()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    public void M()
    {
        object x = null;

        if ([|x is string|])
        {
            if (((string)x).Equals((string)x)) { }
        }
    }
}
", @"
class C
{
    public void M()
    {
        object x = null;

        if (x is string x2)
        {
            if ((x2).Equals(x2)) { }
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UsePatternMatchingInsteadOfIsAndCast)]
        public async Task Test_IfStatement3()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    private readonly object _f = false;

    public void M()
    {
        object x = null;
        string s = null;

        if ([|_f is string|])
        {
            if ((string)_f == s) { }
        }
    }
}
", @"
class C
{
    private readonly object _f = false;

    public void M()
    {
        object x = null;
        string s = null;

        if (_f is string x2)
        {
            if (x2 == s) { }
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UsePatternMatchingInsteadOfIsAndCast)]
        public async Task Test_IfStatement4()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    private readonly object _f = false;

    public void M()
    {
        object x = null;
        string s = null;

        if ([|this._f is string|])
        {
            if ((string)this._f == s) { }
        }
    }
}
", @"
class C
{
    private readonly object _f = false;

    public void M()
    {
        object x = null;
        string s = null;

        if (this._f is string x2)
        {
            if (x2 == s) { }
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UsePatternMatchingInsteadOfIsAndCast)]
        public async Task Test_IfStatement5()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    private readonly object _f = false;

    public void M()
    {
        object x = null;
        string s = null;

        if ([|_f is string|])
        {
            if ((string)this._f == s) { }
        }
    }
}
", @"
class C
{
    private readonly object _f = false;

    public void M()
    {
        object x = null;
        string s = null;

        if (_f is string x2)
        {
            if (x2 == s) { }
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UsePatternMatchingInsteadOfIsAndCast)]
        public async Task Test_IfStatement6()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    private readonly object _f = false;

    public void M()
    {
        object x = null;
        string s = null;


        if ([|this._f is string|])
        {
            if ((string)_f == s) { }
        }
    }
}
", @"
class C
{
    private readonly object _f = false;

    public void M()
    {
        object x = null;
        string s = null;


        if (this._f is string x2)
        {
            if (x2 == s) { }
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UsePatternMatchingInsteadOfIsAndCast)]
        public async Task Test_IfStatement7()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    private readonly object _f = false;

    public void M()
    {
        object x = null;

        if ([|this._f is string|])
        {
            if (((string)_f).Equals((string)this._f)) { }
        }
    }
}
", @"
class C
{
    private readonly object _f = false;

    public void M()
    {
        object x = null;

        if (this._f is string x2)
        {
            if ((x2).Equals(x2)) { }
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UsePatternMatchingInsteadOfIsAndCast)]
        public async Task Test_IfStatement8()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System.Dynamic;
using System.Collections.Generic;

class C
{
    bool M(dynamic @object, string name)
    {
        if ([|@object is ExpandoObject|])
            return ((IDictionary<string, object>)@object).ContainsKey(name);

        return false;
    }
}
", @"
using System.Dynamic;
using System.Collections.Generic;

class C
{
    bool M(dynamic @object, string name)
    {
        if (@object is ExpandoObject expandoObject)
            return ((IDictionary<string, object>)expandoObject).ContainsKey(name);

        return false;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UsePatternMatchingInsteadOfIsAndCast)]
        public async Task Test_IfStatement_Enum()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    private readonly object _f = false;

    public void M()
    {
        if ([|this._f is Enum|])
        {
            if (((Enum)_f).Equals((Enum)this._f)) { }
        }
    }
}
", @"
using System;

class C
{
    private readonly object _f = false;

    public void M()
    {
        if (this._f is Enum @enum)
        {
            if ((@enum).Equals(@enum)) { }
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UsePatternMatchingInsteadOfIsAndCast)]
        public async Task TestNoDiagnostic_LogicalAndExpression()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    public void M()
    {
        object x = null;
        object x2 = null;
        string s = null;

        if (x is string && ReferenceEquals(((string)x), x)) { }

        if (x is string && ReferenceEquals(((string)x2), s)) { }

        if (x is string && ReferenceEquals(x, s)) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UsePatternMatchingInsteadOfIsAndCast)]
        public async Task TestNoDiagnostic_IfStatement()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    public void M()
    {
        object x = null;
        object x2 = null;
        string s = null;

        if (x is string)
        {
            if (ReferenceEquals(((string)x), x)) { }
        }

        if (x is string)
        {
            if (((string)x2) == s) { }
        }

        if (x is string)
        {
            if (ReferenceEquals(x, s)) { }
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UsePatternMatchingInsteadOfIsAndCast)]
        public async Task TestNoDiagnostic_LogicalAnd_ExpressionTree()
        {
            await VerifyNoDiagnosticAsync(@"
using System;
using System.Linq.Expressions;

class C
{
    public void M<T>(Expression<Func<T>> expression)
    {
        object x = null;
        string s = null;

        M(() => x is string && ((string)x) == s);
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UsePatternMatchingInsteadOfIsAndCast)]
        public async Task TestNoDiagnostic_NullableType()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M(int? p)
    {
        object x = null;

        if (x is int?)
        {
            M((int?)x);
        }

        if (x is int? && ((int?)x) == 0) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UsePatternMatchingInsteadOfIsAndCast)]
        public async Task TestNoDiagnostic_LanguageVersion()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    public void M()
    {
        object x = null;
        string s = null;

        if (x is string && ((string)x) == s) { }
    }
}
", options: WellKnownCSharpTestOptions.Default_CSharp6);
        }
    }
}
