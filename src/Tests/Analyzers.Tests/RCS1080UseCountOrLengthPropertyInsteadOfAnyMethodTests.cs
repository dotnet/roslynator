// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1080UseCountOrLengthPropertyInsteadOfAnyMethodTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UseCountOrLengthPropertyInsteadOfAnyMethod;

        public override DiagnosticAnalyzer Analyzer { get; } = new InvocationExpressionAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new InvocationExpressionCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseCountOrLengthPropertyInsteadOfAnyMethod)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        List<object> items = null;

        bool any = items.[|Any()|] //x
            ;
    }
}
", @"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        List<object> items = null;

        bool any = items.Count > 0 //x
            ;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseCountOrLengthPropertyInsteadOfAnyMethod)]
        public async Task Test_LogicalNot()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        List<object> items = null;

        bool any = !items.[|Any()|] //x
            ;
    }
}
", @"
using System.Collections.Generic;
using System.Linq;

class C
{
    void M()
    {
        List<object> items = null;

        bool any = items.Count == 0 //x
            ;
    }
}
");
        }
    }
}
