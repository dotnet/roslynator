// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1207UseMethodGroupInsteadOfAnonymousFunctionTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UseMethodGroupInsteadOfAnonymousFunction;

        public override DiagnosticAnalyzer Analyzer { get; } = new UseMethodGroupInsteadOfAnonymousFunctionAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new AnonymousFunctionCodeFixProvider();

        [Theory, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseMethodGroupInsteadOfAnonymousFunction)]
        [InlineData("f => M(f)", "M")]
        [InlineData("f => { M(f); }", "M")]
        [InlineData("(f) => M(f)", "M")]
        [InlineData("(f) => { M(f); }", "M")]
        [InlineData("delegate (string f) { M(f); }", "M")]
        [InlineData("f => f.M()", "M")]
        [InlineData("f => { f.M(); }", "M")]
        [InlineData("(f) => f.M()", "M")]
        [InlineData("(f) => { f.M(); }", "M")]
        [InlineData("delegate (string f) { f.M(); }", "M")]
        public async Task Test_VoidAnonymousFunction_AsParameter(string fromData, string toData)
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Collections.Generic;

static class C
{
    static void M()
    {
        var items = new List<string>();

        items.ForEach([||]);
    }
    
    static void M(this string value) { }
}
", fromData, toData);
        }

        [Theory, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseMethodGroupInsteadOfAnonymousFunction)]
        [InlineData("f => M(f)", "M")]
        [InlineData("f => { return M(f); }", "M")]
        [InlineData("delegate (string f) { return M(f); }", "M")]
        [InlineData("f => f.M()", "M")]
        [InlineData("f => { return f.M();}", "M")]
        [InlineData("delegate (string f) { return f.M(); }", "M")]
        public async Task Test_AnonymousFunction_AsParameter(string fromData, string toData)
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Collections.Generic;
using System.Linq;

static class C
{
    static void M()
    {
        var items = new List<string>();

        IEnumerable<object> x = items.Select([||]);
    }
    
    static string M(this string value) => value;
}
", fromData, toData);
        }

        [Theory, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseMethodGroupInsteadOfAnonymousFunction)]
        [InlineData("(f, i) => M(f, i)", "M")]
        [InlineData("(f, i) => { return M(f, i); }", "M")]
        [InlineData("delegate (string f, int i) { return M(f, i); }", "M")]
        [InlineData("(f, i) => f.M(i)", "M")]
        [InlineData("(f, i) => { return f.M(i); }", "M")]
        [InlineData("delegate (string f, int i) { return f.M(i); }", "M")]
        public async Task Test_AnonymousFunctionWithTwoParameters_AsParameter(string fromData, string toData)
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Collections.Generic;
using System.Linq;

static class C
{
    static void M()
    {
        var items = new List<string>();

        IEnumerable<object> x = items.Select([||]);
    }
    
    static string M(this string value, int index) => value;
}
", fromData, toData);
        }

        [Theory, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseMethodGroupInsteadOfAnonymousFunction)]
        [InlineData("f => M(f)", "M")]
        [InlineData("f => { return M(f); }", "M")]
        [InlineData("delegate (string f) { return M(f); }", "M")]
        [InlineData("f => f.M()", "M")]
        [InlineData("f => { return f.M(); }", "M")]
        [InlineData("delegate (string f) { return f.M(); }", "M")]
        public async Task Test_AnonymousFunction_Assignment(string fromData, string toData)
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

static class C
{
    static void M2()
    {
        Func<string, object> func = [||];
    }
    
    static string M(this string value) => value;
}
", fromData, toData);
        }

        [Theory, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseMethodGroupInsteadOfAnonymousFunction)]
        [InlineData("(f, i) => M(f, i)", "M")]
        [InlineData("(f, i) => { return M(f, i); }", "M")]
        [InlineData("delegate (string f, int i) { return M(f, i); }", "M")]
        [InlineData("(f, i) => f.M(i)", "M")]
        [InlineData("(f, i) => { return f.M(i); }", "M")]
        [InlineData("delegate (string f, int i) { return f.M(i); }", "M")]
        public async Task Test_AnonymousFunctionWithTwoParameters_Assignment(string fromData, string toData)
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

static class C
{
    static void M2()
    {
        Func<string, int, object> func = [||];
    }
    
    static string M(this string value, int index) => value;
}
", fromData, toData);
        }

        [Theory, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseMethodGroupInsteadOfAnonymousFunction)]
        [InlineData("() => Foo.M()", "Foo.M")]
        [InlineData("delegate () { return Foo.M(); }", "Foo.M")]
        public async Task Test_StaticMethod_Assignment(string fromData, string toData)
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Collections.Generic;

static class Foo
{
    static void M2()
    {
        var items = new List<string>();

        Func<string> func = [||];
    }
    
    static string M() => null;

}
", fromData, toData);
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseMethodGroupInsteadOfAnonymousFunction)]
        public async Task TestNoDiagnostic_NullReferenceException()
        {
            await VerifyNoDiagnosticAsync(@"
using System;

class Foo
{
    void M()
    {
        Func<string> func = null;

        Foo x = null;

        func = () => x.M2();

        func = delegate () { return x.M2(); };
    }

    private string M2() => null;
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseMethodGroupInsteadOfAnonymousFunction)]
        public async Task TestNoDiagnostic_FuncToAction()
        {
            await VerifyNoDiagnosticAsync(@"
using System;

class Foo
{
    void M()
    {
        Action action = () => GetHashCode();
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseMethodGroupInsteadOfAnonymousFunction)]
        public async Task TestNoDiagnostic_FuncToAction2()
        {
            await VerifyNoDiagnosticAsync(@"
using System;

class Foo
{
    private void M(Action action)
    {
        action();

        M(() => GetHashCode());
    }
}
");
        }

        //TODO: Reports diagnostic before C# 7.3
#pragma warning disable xUnit1013
        public async Task TestNoDiagnostic_ReportsDiagnosticBeforeCSharp73()
#pragma warning restore xUnit1013
        {
            await VerifyNoDiagnosticAsync(@"
using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        IEnumerable<int> items = Enumerable.Empty<string>().SelectMany(f => M2(f));
    }

    private static ImmutableArray<int> M2(string s) => throw new NotImplementedException();
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseMethodGroupInsteadOfAnonymousFunction)]
        public async Task TestNoDiagnostic_ReducedExtensionFromOtherClassInvokedOnLambdaParameter()
        {
            await VerifyNoDiagnosticAsync(@"
using System;
using System.Collections.Generic;
using System.Linq;

class C
{
    void M(Func<IEnumerable<string>, string> func)
    {
        M(f => f.First());
        M((f) => f.First());
        M(delegate (IEnumerable<string> p) { return p.First(); });
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseMethodGroupInsteadOfAnonymousFunction)]
        public async Task TestNoDiagnostic_ConditionalAccess()
        {
            await VerifyNoDiagnosticAsync(@"
using System;
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
    }

    private static void M<TSource, TResult>(IEnumerable<TSource> items, Func<TSource, TResult> selector)
    {
        IEnumerable<TResult> x = null;

        x = items?.Select(e => selector(e));
        x = items?.Where(f => f != null).Select(e => selector(e));
        x = items?.Where(f => f != null).Select(e => selector(e)).Distinct();
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseMethodGroupInsteadOfAnonymousFunction)]
        public async Task TestNoDiagnostic_DelegateInvoke()
        {
            await VerifyNoDiagnosticAsync(@"
using System;

class C
{
    delegate bool D(string s);

    void M(Func<string, bool> func)
    {
        D d = null;

        M(f => d(f));
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseMethodGroupInsteadOfAnonymousFunction)]
        public async Task TestNoDiagnostic_InParameter()
        {
            await VerifyNoDiagnosticAsync(@"
using System;

class C
{
    void M(Func<string, string> func)
    {
        M(f => M2(f));
    }

    string M2(in string p)
    {
        return p;
    }
}
");
        }
    }
}
