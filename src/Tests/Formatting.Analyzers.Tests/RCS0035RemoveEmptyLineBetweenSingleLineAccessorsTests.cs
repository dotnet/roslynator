// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Formatting.CodeFixes.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class RemoveEmptyLineBetweenSingleLineAccessorsTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.RemoveEmptyLineBetweenSingleLineAccessors;

        public override DiagnosticAnalyzer Analyzer { get; } = new EmptyLineBetweenAccessorsAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new SyntaxTriviaCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveEmptyLineBetweenSingleLineAccessors)]
        public async Task Test_Property()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    string _p;

    string P
    {
        get { return _p; }
[||]
        set { _p = value; }
    }
}
", @"
class C
{
    string _p;

    string P
    {
        get { return _p; }
        set { _p = value; }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveEmptyLineBetweenSingleLineAccessors)]
        public async Task Test_Property_MoreEmptyLines()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    string _p;

    string P
    {
        get { return _p; }
[||]

        set { _p = value; }
    }
}
", @"
class C
{
    string _p;

    string P
    {
        get { return _p; }
        set { _p = value; }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveEmptyLineBetweenSingleLineAccessors)]
        public async Task Test_Event()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    event EventHandler E
    {
        add { }
[||]
        remove { }
    }
}
", @"
using System;

class C
{
    event EventHandler E
    {
        add { }
        remove { }
    }
}
");
        }
    }
}
