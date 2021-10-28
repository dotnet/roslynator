// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    public class CS7036ThereIsNoArgumentGivenThatCorrespondsToRequiredFormalParameter : AbstractCSharpCompilerDiagnosticFixVerifier<ObjectCreationExpressionCodeFixProvider>
    {
        public override string DiagnosticId { get; } = CompilerDiagnosticIdentifiers.CS7036_ThereIsNoArgumentGivenThatCorrespondsToRequiredFormalParameter;

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS7036_ThereIsNoArgumentGivenThatCorrespondsToRequiredFormalParameter)]
        public async Task Test()
        {
            await VerifyFixAsync(@"
class C2
{
    void M()
    {
        var c = new C()
        {
            P1 = string.Empty,
            P2 = ""x"",
        };
    }
}

class C
{
    public C(string p1, string p2)
    {
        P1 = p1;
        P2 = p2;
    }

    public string P1 { get; private set; }
    public string P2 { get; private set; }
}
", @"
class C2
{
    void M()
    {
        var c = new C(string.Empty, ""x"");
    }
}

class C
{
    public C(string p1, string p2)
    {
        P1 = p1;
        P2 = p2;
    }

    public string P1 { get; private set; }
    public string P2 { get; private set; }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS7036_ThereIsNoArgumentGivenThatCorrespondsToRequiredFormalParameter)]
        public async Task Test_NoArgumentList()
        {
            await VerifyFixAsync(@"
class C2
{
    void M()
    {
        var c = new C
        {
            P1 = string.Empty,
            P2 = ""x"",
        };
    }
}

class C
{
    public C(string p1, string p2)
    {
        P1 = p1;
        P2 = p2;
    }

    public string P1 { get; private set; }
    public string P2 { get; private set; }
}
", @"
class C2
{
    void M()
    {
        var c = new C(string.Empty, ""x"");
    }
}

class C
{
    public C(string p1, string p2)
    {
        P1 = p1;
        P2 = p2;
    }

    public string P1 { get; private set; }
    public string P2 { get; private set; }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }

        [Fact, Trait(Traits.CodeFix, CompilerDiagnosticIdentifiers.CS7036_ThereIsNoArgumentGivenThatCorrespondsToRequiredFormalParameter)]
        public async Task Test_NotAllProperties()
        {
            await VerifyFixAsync(@"
class C2
{
    void M()
    {
        var c = new C()
        {
            P1 = string.Empty,
            P3 = string.Empty,
            P2 = ""x"",
        };
    }
}

class C
{
    public C(string p1, string p2)
    {
        P1 = p1;
        P2 = p2;
    }

    public string P1 { get; private set; }
    public string P2 { get; private set; }
    public string P3 { get; set; }
}
", @"
class C2
{
    void M()
    {
        var c = new C(string.Empty, ""x"")
        {

            P3 = string.Empty,

        };
    }
}

class C
{
    public C(string p1, string p2)
    {
        P1 = p1;
        P2 = p2;
    }

    public string P1 { get; private set; }
    public string P2 { get; private set; }
    public string P3 { get; set; }
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticId));
        }
    }
}
