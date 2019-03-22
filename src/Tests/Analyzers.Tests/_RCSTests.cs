// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Tests;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCSTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.AddBracesWhenExpressionSpansOverMultipleLines;

        public override DiagnosticAnalyzer Analyzer { get; }

        public override CodeFixProvider FixProvider { get; }

        //[Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBracesWhenExpressionSpansOverMultipleLines)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class C
{
    void M()
    {
    }
}
", @"
");
        }

        //[Theory, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBracesWhenExpressionSpansOverMultipleLines)]
        //[InlineData("", "")]
        public async Task Test2(string fromData, string toData)
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class C
{
    void M()
    {
    }
}
", fromData, toData);
        }

        //[Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBracesWhenExpressionSpansOverMultipleLines)]
        public async Task TestNoDiagnostic()
        {
            await VerifyNoDiagnosticAsync(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class C
{
    void M()
    {
    }
}
");
        }

        //[Theory, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddBracesWhenExpressionSpansOverMultipleLines)]
        //[InlineData("")]
        public async Task TestNoDiagnostic2(string fromData)
        {
            await VerifyNoDiagnosticAsync(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class C
{
    void M()
    {
    }
}
", fromData);
        }
    }
}
