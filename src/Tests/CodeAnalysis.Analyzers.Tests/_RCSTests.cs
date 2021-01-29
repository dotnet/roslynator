// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Testing;
using Xunit;

namespace Roslynator.CodeAnalysis.CSharp.Tests
{
    public class RCSTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UsePropertySyntaxNodeSpanStart;

        protected override DiagnosticAnalyzer Analyzer { get; }

        public override CodeFixProvider FixProvider { get; }

        //[Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UsePropertySyntaxNodeSpanStart)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

class C
{
    void M()
    {
    }
}
", @"
");
        }

        //[Theory, Trait(Traits.Analyzer, DiagnosticIdentifiers.UsePropertySyntaxNodeSpanStart)]
        //[InlineData("", "")]
        public async Task Test2(string source, string expected)
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

class C
{
    void M()
    {
    }
}
", source, expected);
        }

        //[Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UsePropertySyntaxNodeSpanStart)]
        public async Task TestNoDiagnostic()
        {
            await VerifyNoDiagnosticAsync(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

class C
{
    void M()
    {
    }
}
");
        }

        //[Theory, Trait(Traits.Analyzer, DiagnosticIdentifiers.UsePropertySyntaxNodeSpanStart)]
        //[InlineData("")]
        public async Task TestNoDiagnostic2(string source)
        {
            await VerifyNoDiagnosticAsync(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

class C
{
    void M()
    {
    }
}
", source);
        }
    }
}
