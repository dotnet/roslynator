// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1031RemoveUnnecessaryBracesInSwitchSectionTests : AbstractCSharpDiagnosticVerifier<RemoveUnnecessaryBracesInSwitchSectionAnalyzer, BlockCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.RemoveUnnecessaryBracesInSwitchSection;

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.RemoveUnnecessaryBracesInSwitchSection)]
    public async Task Test_Section()
    {
        await VerifyDiagnosticAndFixAsync("""
class C
{
    void M()
    {
        string s = null;

        switch (s)
        {
            case "":
                [|{|]
                    M();
                    break;
                }
            default:
                break;
        }
    }
}
""", """
class C
{
    void M()
    {
        string s = null;

        switch (s)
        {
            case "":
                M();
                break;

            default:
                break;
        }
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.RemoveUnnecessaryBracesInSwitchSection)]
    public async Task Test_SectionWithComments()
    {
        await VerifyDiagnosticAndFixAsync("""
class C
{
    void M()
    {
        string s = null;

        switch (s)
        {
            case "":
                // a
                [|{|] // b
                    M();
                    break; // c
                    // d
                } // e
            default:
                break;
        }
    }
}
""", """
class C
{
    void M()
    {
        string s = null;

        switch (s)
        {
            case "":
                // a
                // b
                M();
                break; // c
                       // d
                       // e

            default:
                break;
        }
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.RemoveUnnecessaryBracesInSwitchSection)]
    public async Task Test_LastSection()
    {
        await VerifyDiagnosticAndFixAsync("""
class C
{
    void M()
    {
        string s = null;

        switch (s)
        {
            case "":
                [|{|]
                    M();
                    break;
                }
        }
    }
}
""", """
class C
{
    void M()
    {
        string s = null;

        switch (s)
        {
            case "":
                M();
                break;
        }
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.RemoveUnnecessaryBracesInSwitchSection)]
    public async Task Test_WithLocalVariablesThatDoNotOverlap()
    {
        await VerifyDiagnosticAndFixAsync("""
using System;

class C
{
    void M()
    {
        string s = null;

        switch (s)
        {
            case "":
                [|{|]
                    var x = 1;
                    break;
                }
            default:
                [|{|]
                    var y = 1;
                    break;
                }
        }
    }
}
""", """
using System;

class C
{
    void M()
    {
        string s = null;

        switch (s)
        {
            case "":
                var x = 1;
                break;

            default:
                var y = 1;
                break;
        }
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.RemoveUnnecessaryBracesInSwitchSection)]
    public async Task TestNoDiagnostic_SectionWithoutBlock()
    {
        await VerifyNoDiagnosticAsync("""
class C
{
    void M()
    {
        string s = null;

        switch (s)
        {
            case "":
                break;
        }
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.RemoveUnnecessaryBracesInSwitchSection)]
    public async Task TestNoDiagnostic_UsingLocalVariable()
    {
        await VerifyNoDiagnosticAsync("""
using System;

class C
{
    void M()
    {
        string s = null;

        switch (s)
        {
            case "":
                {
                    using IDisposable disposable = default;
                    break;
                }
        }
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.RemoveUnnecessaryBracesInSwitchSection)]
    public async Task TestNoDiagnostic_WhenOverlappingLocalVariableDeclaration()
    {
        await VerifyNoDiagnosticAsync("""
using System;

class C
{
    void M()
    {
        string s = null;

        switch (s)
        {
            case "":
                {
                    var x = 1;
                    break;
                }
            default:
                {
                    var x = 1;
                    break;
                }
        }
    }
}
""");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.RemoveUnnecessaryBracesInSwitchSection)]
    public async Task TestNoDiagnostic_WhenOverlappingLocalVariableWithPatternMatchDeclaration()
    {
        await VerifyNoDiagnosticAsync(@"
using System;

class C
{
    void M()
    {
        object o = null;

        switch (o)
        {
            case string s:
                var x = 1;
                break;
            default:
                {
                    var s = 1;
                    break;
                }
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.RemoveUnnecessaryBracesInSwitchSection)]
    public async Task TestNoDiagnostic_WhenOverlappingLocalVariableWithRecursivePatternMatchDeclaration()
    {
        await VerifyNoDiagnosticAsync(@"
using System;

class C
{
    class Wrapper
    {
        public string S;
    }
    void M()
    {
        object o = null;

        switch (o)
        {
            case Wrapper { S: var s }:
                var x = 1;
                break;
            default:
                {
                    var s = 1;
                    break;
                }
        }
    }
}
");
    }
}
