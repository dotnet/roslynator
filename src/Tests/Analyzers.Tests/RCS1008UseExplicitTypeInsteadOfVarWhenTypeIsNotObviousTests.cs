// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1008UseExplicitTypeInsteadOfVarWhenTypeIsNotObviousTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UseExplicitTypeInsteadOfVarWhenTypeIsNotObvious;

        public override DiagnosticAnalyzer Analyzer { get; } = new UseExplicitTypeInsteadOfVarWhenTypeIsNotObviousAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new UseExplicitTypeInsteadOfVarCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitTypeInsteadOfVarWhenTypeIsNotObvious)]
        public async Task Test_LocalVariable()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        [|var|] a = ""a"";
        [|var|] s = a;
    }
}
", @"
class C
{
    void M()
    {
        string a = ""a"";
        string s = a;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitTypeInsteadOfVarWhenTypeIsNotObvious)]
        public async Task Test_DeclarationExpression()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    void M()
    {
        string value = null;
        if (DateTime.TryParse(value, out [|var|] result)) { }
    }
}
", @"
using System;

class C
{
    void M()
    {
        string value = null;
        if (DateTime.TryParse(value, out DateTime result)) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitTypeInsteadOfVarWhenTypeIsNotObvious)]
        public async Task Test_Tuple()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Collections.Generic;

class C
{
    (IEnumerable<DateTime> e1, string e2) M()
    {
        [|var|] x = M();

        return default((IEnumerable<DateTime>, string));
    }
}
", @"
using System;
using System.Collections.Generic;

class C
{
    (IEnumerable<DateTime> e1, string e2) M()
    {
        (IEnumerable<DateTime> e1, string e2) x = M();

        return default((IEnumerable<DateTime>, string));
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseExplicitTypeInsteadOfVarWhenTypeIsNotObvious)]
        internal async Task TestNoDiagnostic()
        {
            await VerifyNoDiagnosticAsync(@"
using System;

class C
{
    void M()
    {
        string a = ""a"";

        string s = a;

        string value = null;
        if (DateTime.TryParse(s, out DateTime result))
        {
        }
    }
}
");
        }
    }
}
