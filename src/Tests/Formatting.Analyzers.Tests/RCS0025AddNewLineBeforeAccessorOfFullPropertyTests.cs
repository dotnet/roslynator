// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Formatting.CodeFixes.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class AddNewLineBeforeAccessorOfFullPropertyTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.AddNewLineBeforeAccessorOfFullProperty;

        public override DiagnosticAnalyzer Analyzer { get; } = new AccessorListAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new AccessorDeclarationCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBeforeAccessorOfFullProperty)]
        public async Task Test_Property()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    private string _p;

    public string P { [||]get { return _p; } set { _p = value; } }
}
", @"
class C
{
    private string _p;

    public string P
    {
        get { return _p; }
        set { _p = value; }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBeforeAccessorOfFullProperty)]
        public async Task Test_Property_Setter()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    private string _p;

    public string P
    {
        get { return _p; } [||]set { _p = value; }
    }
}
", @"
class C
{
    private string _p;

    public string P
    {
        get { return _p; }
        set { _p = value; }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBeforeAccessorOfFullProperty)]
        public async Task Test_Property_ExpressionBody()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    private string _p;

    public string P { [||]get => _p; set => _p = value; }
}
", @"
class C
{
    private string _p;

    public string P
    {
        get => _p;
        set => _p = value;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBeforeAccessorOfFullProperty)]
        public async Task Test_Property_ExpressionBody_Setter()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    private string _p;

    public string P
    {
        get => _p; [||]set => _p = value;
    }
}
", @"
class C
{
    private string _p;

    public string P
    {
        get => _p;
        set => _p = value;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBeforeAccessorOfFullProperty)]
        public async Task Test_ReadOnlyProperty()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    private string _p;

    public string P { [||]get { return _p; } }
}
", @"
class C
{
    private string _p;

    public string P
    {
        get { return _p; }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBeforeAccessorOfFullProperty)]
        public async Task Test_ReadOnlyProperty_ExpressionBody()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    private string _p;

    public string P { [||]get => _p; }
}
", @"
class C
{
    private string _p;

    public string P
    {
        get => _p;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineBeforeAccessorOfFullProperty)]
        public async Task TestNoDiagnostic()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    private string _p;

    public string P
    {
        get { return _p; }
        set { _p = value; }
    }
}
");
        }
    }
}
