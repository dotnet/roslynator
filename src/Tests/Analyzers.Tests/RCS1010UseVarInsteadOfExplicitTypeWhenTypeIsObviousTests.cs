// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1010UseVarInsteadOfExplicitTypeWhenTypeIsObviousTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UseVarInsteadOfExplicitTypeWhenTypeIsObvious;

        public override DiagnosticAnalyzer Analyzer { get; } = new UseVarInsteadOfExplicitTypeWhenTypeIsObviousAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new UseVarInsteadOfExplicitTypeCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarInsteadOfExplicitTypeWhenTypeIsObvious)]
        public async Task Test_ObjectCreation()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;

class C
{
    void M()
    {
        [|List<object>|] x = new List<object>();
    }
}
", @"
using System.Collections.Generic;

class C
{
    void M()
    {
        var x = new List<object>();
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarInsteadOfExplicitTypeWhenTypeIsObvious)]
        public async Task Test_DefaultExpression_TupleExpression()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        [|(object x, string y)|] = default((object, string));
    }
}
", @"
class C
{
    void M()
    {
        var (x, y) = default((object, string));
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarInsteadOfExplicitTypeWhenTypeIsObvious)]
        public async Task Test_DefaultExpression_TupleExpression_var()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        [|(object x, var y)|] = default((object, string));
    }
}
", @"
class C
{
    void M()
    {
        var (x, y) = default((object, string));
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarInsteadOfExplicitTypeWhenTypeIsObvious)]
        public async Task TestNoDiagnostic()
        {
            await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;
using System.Text.RegularExpressions;

class C
{
    void M()
    {
        object o = """";
        const string c = """";
        string value1, value2;
        dynamic x = new object();
        dynamic x2 = c;

        foreach (Match match in Regex.Matches(""input"", ""pattern""))
        {
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarInsteadOfExplicitTypeWhenTypeIsObvious)]
        public async Task TestNoDiagnostic_ForEach_DeclarationExpression()
        {
            await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;

class C
{
    IEnumerable<(object x, string y)> M()
    {
        foreach (var (x, y) in M())
        {
        }

        return default;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseVarInsteadOfExplicitTypeWhenTypeIsObvious)]
        public async Task TestNoDiagnostic_ForEach_TupleExpression()
        {
            await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;

class C
{
    IEnumerable<(object x, string y)> M()
    {
        foreach ((object x, string y) in M())
        {
        }

        return default;
    }
}
");
        }
    }
}
