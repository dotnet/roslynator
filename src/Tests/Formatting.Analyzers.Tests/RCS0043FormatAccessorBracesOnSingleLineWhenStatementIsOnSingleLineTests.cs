// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Formatting.CodeFixes.CSharp;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class RCS0043FormatAccessorBracesOnSingleLineWhenStatementIsOnSingleLineTests : AbstractCSharpDiagnosticVerifier<FormatAccessorBracesAnalyzer, AccessorDeclarationCodeFixProvider>
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.FormatAccessorBracesOnSingleLineWhenExpressionIsOnSingleLine;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FormatAccessorBracesOnSingleLineWhenExpressionIsOnSingleLine)]
        public async Task Test_Getter_Setter()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    private string _p;

    string P
    {
        [|get
        {
            return _p;
        }|]
        [|set
        {
            _p = value;
        }|]
    }
}
", @"
class C
{
    private string _p;

    string P
    {
        get { return _p; }
        set { _p = value; }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FormatAccessorBracesOnSingleLineWhenExpressionIsOnSingleLine)]
        public async Task Test_Getter()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    private string _p;

    string P
    {
        [|get
        {
            return _p;
        }|]
        set { _p = value; }
    }
}
", @"
class C
{
    private string _p;

    string P
    {
        get { return _p; }
        set { _p = value; }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FormatAccessorBracesOnSingleLineWhenExpressionIsOnSingleLine)]
        public async Task Test_Setter()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    private string _p;

    string P
    {
        get { return _p; }
        [|set
        {
            _p = value;
        }|]
    }
}
", @"
class C
{
    private string _p;

    string P
    {
        get { return _p; }
        set { _p = value; }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FormatAccessorBracesOnSingleLineWhenExpressionIsOnSingleLine)]
        public async Task TestNoDiagnostic_FullProperty()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    private string _p;

    string P
    {
        get { return _p; }
        set { _p = value; }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FormatAccessorBracesOnSingleLineWhenExpressionIsOnSingleLine)]
        public async Task TestNoDiagnostic_AutoProperty()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    string P
    {
        get;
        set;
    }
}
");
        }
    }
}
