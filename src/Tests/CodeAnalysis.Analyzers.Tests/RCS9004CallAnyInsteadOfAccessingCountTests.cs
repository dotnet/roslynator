// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace Roslynator.CodeAnalysis.CSharp.Tests
{
    public class RCS9004CallAnyInsteadOfAccessingCountTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.CallAnyInsteadOfAccessingCount;

        protected override DiagnosticAnalyzer Analyzer { get; } = new SimpleMemberAccessExpressionAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new BinaryExpressionCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.CallAnyInsteadOfAccessingCount)]
        public async Task Test_SyntaxList_Equals()
        {
            await VerifyDiagnosticAndFixAsync(@"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SyntaxList<SyntaxNode> list = default;

        if (list.[|Count == 0|]) { }
    }
}
", @"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SyntaxList<SyntaxNode> list = default;

        if (!list.Any()) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.CallAnyInsteadOfAccessingCount)]
        public async Task Test_SyntaxList_Equals2()
        {
            await VerifyDiagnosticAndFixAsync(@"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SyntaxList<SyntaxNode> list = default;

        if ([|0 == list.Count|]) { }
    }
}
", @"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SyntaxList<SyntaxNode> list = default;

        if (!list.Any()) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.CallAnyInsteadOfAccessingCount)]
        public async Task Test_SyntaxList_NotEquals()
        {
            await VerifyDiagnosticAndFixAsync(@"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SyntaxList<SyntaxNode> list = default;

        if (list.[|Count != 0|]) { }
    }
}
", @"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SyntaxList<SyntaxNode> list = default;

        if (list.Any()) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.CallAnyInsteadOfAccessingCount)]
        public async Task Test_SyntaxList_GreaterThan()
        {
            await VerifyDiagnosticAndFixAsync(@"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SyntaxList<SyntaxNode> list = default;

        if (list.[|Count > 0|]) { }
    }
}
", @"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SyntaxList<SyntaxNode> list = default;

        if (list.Any()) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.CallAnyInsteadOfAccessingCount)]
        public async Task Test_SeparatedSyntaxList_GreaterThan()
        {
            await VerifyDiagnosticAndFixAsync(@"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SeparatedSyntaxList<SyntaxNode> list = default;

        if (list.[|Count > 0|]) { }
    }
}
", @"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SeparatedSyntaxList<SyntaxNode> list = default;

        if (list.Any()) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.CallAnyInsteadOfAccessingCount)]
        public async Task Test_SyntaxTokenList_GreaterThan()
        {
            await VerifyDiagnosticAndFixAsync(@"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SyntaxTokenList list = default;

        if (list.[|Count > 0|]) { }
    }
}
", @"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SyntaxTokenList list = default;

        if (list.Any()) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.CallAnyInsteadOfAccessingCount)]
        public async Task Test_SyntaxTriviaList_GreaterThan()
        {
            await VerifyDiagnosticAndFixAsync(@"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SyntaxTriviaList list = default;

        if (list.[|Count > 0|]) { }
    }
}
", @"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SyntaxTriviaList list = default;

        if (list.Any()) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.CallAnyInsteadOfAccessingCount)]
        public async Task Test_ChildSyntaxList_GreaterThan()
        {
            await VerifyDiagnosticAndFixAsync(@"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        ChildSyntaxList list = default;

        if (list.[|Count > 0|]) { }
    }
}
", @"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        ChildSyntaxList list = default;

        if (list.Any()) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.CallAnyInsteadOfAccessingCount)]
        public async Task Test_SyntaxNodeOrTokenList_GreaterThan()
        {
            await VerifyDiagnosticAndFixAsync(@"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SyntaxNodeOrTokenList list = default;

        if (list.[|Count > 0|]) { }
    }
}
", @"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SyntaxNodeOrTokenList list = default;

        if (list.Any()) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.CallAnyInsteadOfAccessingCount)]
        public async Task TestNoDiagnostic_LessThanExpression()
        {
            await VerifyNoDiagnosticAsync(@"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SyntaxList<SyntaxNode> list = default;

        if (list.Count < 0) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.CallAnyInsteadOfAccessingCount)]
        public async Task TestNoDiagnostic_NotNumericLiteralExpression()
        {
            await VerifyNoDiagnosticAsync(@"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SyntaxList<SyntaxNode> list = default;
        int count = 0;

        if (list.Count > count) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.CallAnyInsteadOfAccessingCount)]
        public async Task TestNoDiagnostic_NotZeroNumericLiteralExpression()
        {
            await VerifyNoDiagnosticAsync(@"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SyntaxList<SyntaxNode> list = default;

        if (list.Count > 1) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.CallAnyInsteadOfAccessingCount)]
        public async Task TestNoDiagnostic_NotSyntaxList()
        {
            await VerifyNoDiagnosticAsync(@"
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        List<SyntaxNode> list = default;

        if (list.Count > 0) { }
    }
}
");
        }
    }
}
