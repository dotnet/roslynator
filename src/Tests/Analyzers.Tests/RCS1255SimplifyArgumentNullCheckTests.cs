// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1255SimplifyArgumentNullCheckTests : AbstractCSharpDiagnosticVerifier<SimplifyArgumentNullCheckAnalyzer, IfStatementCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.SimplifyArgumentNullCheck;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyArgumentNullCheck)]
    public async Task Test_IfStatement_Block_Nameof()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    C(string x)
    {
        [|if|] (x is null)
        {
            throw new ArgumentNullException(nameof(x));
        }
    }
}
", @"
using System;

class C
{
    C(string x)
    {
        ArgumentNullException.ThrowIfNull(x);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyArgumentNullCheck)]
    public async Task Test_IfStatement_Block_Literal()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    C(string x)
    {
        [|if|] (x is null)
        {
            throw new ArgumentNullException(""x"");
        }
    }
}
", @"
using System;

class C
{
    C(string x)
    {
        ArgumentNullException.ThrowIfNull(x);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyArgumentNullCheck)]
    public async Task Test_IfStatement_Embedded_Nameof()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    C(string x)
    {
        [|if|] (x is null)
            throw new ArgumentNullException(nameof(x));
    }
}
", @"
using System;

class C
{
    C(string x)
    {
        ArgumentNullException.ThrowIfNull(x);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyArgumentNullCheck)]
    public async Task TestNoDiagnostic_TwoArguments()
    {
        await VerifyNoDiagnosticAsync(@"
using System;

class C
{
    C(string x)
    {
        if (x is null)
        {
            throw new ArgumentNullException(nameof(x), ""message"");
        }
    }
}
");
    }
}
