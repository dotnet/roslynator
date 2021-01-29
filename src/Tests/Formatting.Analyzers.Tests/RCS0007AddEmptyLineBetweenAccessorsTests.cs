// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Formatting.CodeFixes.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class RCS0007AddEmptyLineBetweenAccessorsTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.AddEmptyLineBetweenAccessors;

        protected override DiagnosticAnalyzer Analyzer { get; } = new AddOrRemoveEmptyLineBetweenAccessorsAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new SyntaxTriviaCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineBetweenAccessors)]
        public async Task Test_FirstIsMultiline()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    string _p;

    string P
    {
        get
        {
            return _p;
        }[||]
        set { _p = value; }
    }
}
", @"
class C
{
    string _p;

    string P
    {
        get
        {
            return _p;
        }

        set { _p = value; }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineBetweenAccessors)]
        public async Task Test_SecondIsMultiline()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    string _p;

    string P
    {
        get { return _p; }[||]
        set
        {
            _p = value;
        }
    }
}
", @"
class C
{
    string _p;

    string P
    {
        get { return _p; }

        set
        {
            _p = value;
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineBetweenAccessors)]
        public async Task Test_BothAreMultiline()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    string _p;

    string P
    {
        get
        {
            return _p;
        }[||]
        set
        {
            _p = value;
        }
    }
}
", @"
class C
{
    string _p;

    string P
    {
        get
        {
            return _p;
        }

        set
        {
            _p = value;
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddEmptyLineBetweenAccessors)]
        public async Task TestNoDiagnostic_SingleLine()
        {
            await VerifyNoDiagnosticAsync(@"
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
    }
}
