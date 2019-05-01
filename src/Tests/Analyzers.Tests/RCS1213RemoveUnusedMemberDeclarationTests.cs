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
    public class RCS1213RemoveUnusedMemberDeclarationTests : AbstractCSharpFixVerifier
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
        public async Task Test_Method_IfElsePreprocessorDirectives()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    private void [|M|]()
    {
#if DEBUG
#else
#endif
    }
}
", @"
class C
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnusedMemberDeclaration)]
        public async Task Test_Method_PragmaPreprocessorDirectives()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
        private void [|M|]()
        {
        }

#pragma warning disable IDE0001
        private const int [|K|] = 0;
}
", @"
class C
{

#pragma warning disable IDE0001
        }
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnusedMemberDeclaration)]
        public async Task Test_Method_RegionPreprocessorDirectives()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    #region R
    private void [|M|]()
    {
    #endregion R
    }
}
", @"
class C
{
    #region R
    
#endregion R
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

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnusedMemberDeclaration)]
        public async Task TestNoDiagnostic_UnbalancedPreprocessorDirectives()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
#if DEBUG
    void M()
#else
    void M()
#endif
    {
        M();
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnusedMemberDeclaration)]
        public async Task TestNoDiagnostic_LateBound()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        dynamic x = 1;
        Foo(x);

        x = 1.1;
        Foo(x);
    }

    void Foo(int _) => M();

    void Foo(double _) => M();
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnusedMemberDeclaration)]
        public async Task TestNoDiagnostic_StructLayoutAttribute()
        {
            await VerifyNoDiagnosticAsync(@"
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
struct S
{
    private int F;
}
");
        }
    }
}
