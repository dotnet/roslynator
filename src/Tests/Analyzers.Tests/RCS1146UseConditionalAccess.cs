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
    public static class RCS1146UseConditionalAccess
    {
        private static DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UseConditionalAccess;

        private static DiagnosticAnalyzer Analyzer { get; } = new UseConditionalAccessAnalyzer();

        private static CodeFixProvider CodeFixProvider { get; } = new UseConditionalAccessCodeFixProvider();

        [Fact]
        public static void TestDiagnosticWithCodeFix_IfStatement()
        {
            VerifyDiagnosticAndFix(@"
public class C
{
    public void M()
    {
        C x = null;

        <<<if (x != null)
            x.M();>>>

        <<<if (x != null)
        {
            x.M();
        }>>>
    }
}

public struct S
{
    public void M()
    {
        S? x = null;

        <<<if (x != null)
            x.Value.M();>>>

        <<<if (x != null)
        {
            x.Value.M();
        }>>>
    }
}
", @"
public class C
{
    public void M()
    {
        C x = null;

        x?.M();

        x?.M();
    }
}

public struct S
{
    public void M()
    {
        S? x = null;

        x?.M();

        x?.M();
    }
}
", DiagnosticDescriptors.UseConditionalAccess, Analyzer, CodeFixProvider);
        }

        [Fact]
        public static void TestDiagnosticWithCodeFix_LogicalAndExpression_ReferenceType()
        {
            VerifyDiagnosticAndFix(@"
using System.Collections.Generic;

public class Foo
{
    private const string NonNullConst = ""x"";

    public string Value { get; }

    public void M()
    {
        bool f = false;

        Foo x = null;

        if (<<<x != null && x.Equals(x)>>>) { }

        if (<<<null != x && x.Equals(x)>>>) { }

        if (<<<x != null && (x.Equals(x))>>>) { }

        if (<<<x != null && x.Equals(x)>>> && f) { }

        if (f && <<<x != null && x.Equals(x)>>>) { }

        if (<<<x != null && x.Value.Length > 1>>>) { }

        if (<<<x != null && !x.Equals(x)>>>) { }

        if (<<<x != null && (!x.Equals(x))>>>) { }

        if (<<<x != null && x.Value == ""x"">>>) { }

        if (<<<x != null && x.Value == NonNullConst>>>) { }

        if (<<<x != null && x.Value != null>>>) { }

        if (<<<x != null && x.Value is object>>>) { }

        if (<<<x != null && x.Value is object _>>>) { }

        if (x != null && <<<x.ToString() != null && x.ToString().ToString() != null>>>) { }

        if (f &&
     /*lt*/ <<<x != null &&
            x.Equals(""x"")>>> /*tt*/
            && f) { }

        Dictionary<int, string> dic = null;

        if (<<<dic != null && dic[0].Equals(""x"")>>>) { }

        if (<<<dic != null && dic[0].Length > 1>>>) { }

        if (<<<dic != null && !dic[0].Equals(""x"")>>>) { }
    }
}
", @"
using System.Collections.Generic;

public class Foo
{
    private const string NonNullConst = ""x"";

    public string Value { get; }

    public void M()
    {
        bool f = false;

        Foo x = null;

        if (x?.Equals(x) == true) { }

        if (x?.Equals(x) == true) { }

        if (x?.Equals(x) == true) { }

        if (x?.Equals(x) == true && f) { }

        if (f && x?.Equals(x) == true) { }

        if (x?.Value.Length > 1) { }

        if (x?.Equals(x) == false) { }

        if (x?.Equals(x) == false) { }

        if (x?.Value == ""x"") { }

        if (x?.Value == NonNullConst) { }

        if (x?.Value != null) { }

        if (x?.Value is object) { }

        if (x?.Value is object _) { }

        if (x?.ToString()?.ToString() != null) { }

        if (f &&
     /*lt*/ x?.Equals(""x"") == true /*tt*/
            && f) { }

        Dictionary<int, string> dic = null;

        if (dic?[0].Equals(""x"") == true) { }

        if (dic?[0].Length > 1) { }

        if (dic?[0].Equals(""x"") == false) { }
    }
}
", Descriptor, Analyzer, CodeFixProvider);
        }

        [Fact]
        public static void TestDiagnosticWithCodeFix_LogicalAndExpression_NullableType()
        {
            VerifyDiagnosticAndFix(@"
public struct Foo
{
    private const string NonNullConst = ""x"";

    public string V { get; }

    public void M()
    {
        Foo? x = null;

        if (<<<x != null && x.Value.Equals(x)>>>) { }

        if (<<<x != null && x.Value.V.Length > 1>>>) { }

        if (<<<x != null && !x.Value.Equals(x)>>>) { }

        if (<<<x != null && x.Value.V == ""x"">>>) { }

        if (<<<x != null && x.Value.V == NonNullConst>>>) { }

        if (<<<x != null && x.Value.V != null>>>) { }

        if (<<<x != null && x.Value.V is object>>>) { }

        if (<<<x != null && x.Value.V is object _>>>) { }

        if (x != null && <<<x.Value.ToString() != null && x.Value.ToString().ToString() != null>>>) { }
    }
}
", @"
public struct Foo
{
    private const string NonNullConst = ""x"";

    public string V { get; }

    public void M()
    {
        Foo? x = null;

        if (x?.Equals(x) == true) { }

        if (x?.V.Length > 1) { }

        if (x?.Equals(x) == false) { }

        if (x?.V == ""x"") { }

        if (x?.V == NonNullConst) { }

        if (x?.V != null) { }

        if (x?.V is object) { }

        if (x?.V is object _) { }

        if (x?.ToString()?.ToString() != null) { }
    }
}
", Descriptor, Analyzer, CodeFixProvider);
        }

        [Fact]
        public static void TestNoDiagnostic_LogicalAndExpression_ReferenceType()
        {
            VerifyNoDiagnostic(@"
public class Foo
{
    private const string NullConst = null;
    private const string NonNullConst = ""x"";

    public string Value { get; }

    public void M()
    {
        bool f = false;

        string s = null;

        Foo x = null;

        if (x != null && x.Value == null && f) { }

        if (x != null && x.Value == NullConst && f) { }

        if (x != null && x.Value == s && f) { }

        if (x != null && x.Value != ""x"" && f) { }

        if (x != null && x.Value != NonNullConst && f) { }

        if (x != null && x.Value != s && f) { }

        if (x != null && (x.Value != null) is object _) { }
    }
}
", Descriptor, Analyzer);
        }

        [Fact]
        public static void TestNoDiagnostic_LogicalAndExpression_ValueType()
        {
            VerifyNoDiagnostic(@"
public struct Foo
{
    public int Value { get; }

    public void M()
    {
        var x = new Foo();

        if (x != null && x.Value > 0) { }
    }

    public static bool operator ==(Foo left, Foo right) => left.Equals(right);

    public static bool operator !=(Foo left, Foo right) => !(left == right);
}
", Descriptor, Analyzer);
        }

        [Fact]
        public static void TestNoDiagnostic_LogicalAndExpression_NullableType()
        {
            VerifyNoDiagnostic(@"
public struct Foo
{
    public void M()
    {
        bool? f = null;

        if (f != null && f.Value) { }

        if (f != null && f.Value && f.Value) { }

        if (f != null && (f.Value)) { }

        if (f != null && !f.Value) { }

        if (f != null && !f.Value && !f.Value) { }

        if (f != null && (!f.Value)) { }

        Foo? x = null;

        var value = default(Foo);

        if (x != null && x.Value == null) { }

        if (x != null && x.Value == value) { }

        if (x != null && x.Value != null) { }

        if (x != null && x.Value != null) { }

        if (x != null && x.Value != value) { }

        if (x != null && x.HasValue.Equals(true)) { }

        if (x != null && (x.Value == null) is object _) { }
    }

    public static bool operator ==(Foo left, Foo right) => left.Equals(right);

    public static bool operator !=(Foo left, Foo right) => !(left == right);
}
", Descriptor, Analyzer);
        }

        [Fact]
        public static void TestNoDiagnostic_LogicalAndExpression_OutParameter()
        {
            VerifyNoDiagnostic(@"
using System.Collections.Generic;

public class C
{
    public void M()
    {
        Dictionary<int, string> dic = null;

        string value;
        if (dic != null && dic.TryGetValue(0, out value))
        {
        }

        if (dic != null && dic.TryGetValue(0, out string value2))
        {
        }
    }
}
", Descriptor, Analyzer);
        }

        [Fact]
        public static void TestNoDiagnostic_LogicalAndExpression_ExpressionTree()
        {
            VerifyNoDiagnostic(@"
using System;
using System.Linq.Expressions;

public class C
{
    public void M<T>(Expression<Func<T>> expression)
    {
        string s = null;

        M(() => s != null && s.GetHashCode() == 0);
    }
}
", Descriptor, Analyzer);
        }
    }
}
