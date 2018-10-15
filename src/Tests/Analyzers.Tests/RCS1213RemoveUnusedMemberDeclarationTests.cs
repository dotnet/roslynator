// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;
using Roslynator.CSharp.Analysis.UnusedMember;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1213RemoveUnusedMemberDeclarationTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.RemoveUnusedMemberDeclaration;

        public override DiagnosticAnalyzer Analyzer { get; } = new UnusedMemberAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new UnusedMemberCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnusedMemberDeclaration)]
        public async Task Test_Method()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    private string [|M|]() => null;
}
", @"
class C
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnusedMemberDeclaration)]
        public async Task Test_Property()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    private string [|P|] { get; }
}
", @"
class C
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnusedMemberDeclaration)]
        public async Task Test_Method_Recursive()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void [|M|]()
    {
        M();
    }
}
", @"
class C
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnusedMemberDeclaration)]
        public async Task TestNoDiagnostic_Property_AttributeArgument_NameOf()
        {
            await VerifyNoDiagnosticAsync(@"
using System;

class C
{
    private string P { get; }

    [Foo(nameof(P))]
    public string P2 { get; }
}

class FooAttribute : Attribute
{
    public FooAttribute(string s)
    {
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnusedMemberDeclaration)]
        public async Task TestNoDiagnostic_Method_AttributeArgument_NameOf()
        {
            await VerifyNoDiagnosticAsync(@"
using System;

class C
{
    private string M() => null;

    [Foo(nameof(M))]
    public string M2() => null;
}

class FooAttribute : Attribute
{
    public FooAttribute(string s)
    {
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnusedMemberDeclaration)]
        public async Task TestNoDiagnostic_Method_Argument_NameOf()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    string M() => null;

    public string M2(string s = nameof(M)) => s;
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnusedMemberDeclaration)]
        public async Task TestNoDiagnostic_Indexer_Argument_NameOf()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    string M { get; }

    string this[int index, string s = nameof(M)] => s;
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnusedMemberDeclaration)]
        public async Task TestNoDiagnostic_ExtensionMethod()
        {
            await VerifyNoDiagnosticAsync(@"
static class C
{
    public static bool M(this string s)
    {
        return s.M2();
    }

    private static bool M2(this string s)
    {
        return s == null;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnusedMemberDeclaration)]
        public async Task TestNoDiagnostic_DelegateAsReturnType()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    private delegate void D(object p);

    private D M()
    {
        return default;
    }

    public void M2() => M();
}
");
        }
    }
}
