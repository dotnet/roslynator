// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1211RemoveUnnecessaryElseTests : AbstractCSharpDiagnosticVerifier<RemoveUnnecessaryElseAnalyzer, RemoveUnnecessaryElseCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.RemoveUnnecessaryElse;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnnecessaryElse)]
    public async Task Test_UnnecessaryElse_Removed()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    int M(bool flag)
    {
        if(flag)
        {
            return 1;
        }
        [|else|]
        {
            return 0;
        }
    }
}
", @"
class C
{
    int M(bool flag)
    {
        if(flag)
        {
            return 1;
        }

        return 0;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnnecessaryElse)]
    public async Task Test_UnnecessaryElseIf_Removed()
    {
        await VerifyDiagnosticAndFixAsync(@"
class C
{
    int M(bool flag, bool flag2)
    {
        if (flag)
        {
            return 1;
        }
        [|else|] if (flag2)
        {
            return 0;
        }

        return 2;
    }
}
", @"
class C
{
    int M(bool flag, bool flag2)
    {
        if (flag)
        {
            return 1;
        }

        if (flag2)
        {
            return 0;
        }

        return 2;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnnecessaryElse)]
    public async Task TestNoDiagnostic_OverlappingLocalVariables()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    int M(bool flag)
    {
        if(flag)
        {
            var z = 1;
            return z;
        }
        else
        {
            var z = 0;
            return z;
        }
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveUnnecessaryElse)]
    public async Task TestNoDiagnostic_OverlappingLocalVariablesWithSwitch()
    {
        await VerifyNoDiagnosticAsync(@"
class C
{
    int M(int i, bool flag)
    {
        switch(i)
        {
            case 0:
                if(flag)
                {
                    var z = 1;
                    return z;
                }   
                break;
            case 1:
                if(flag)
                {
                    var y = 1;
                    return y;
                }
                else
                {
                    var z = 1;
                    return z;
                }
        }
        return 2;
    }
}
");
    }
}
