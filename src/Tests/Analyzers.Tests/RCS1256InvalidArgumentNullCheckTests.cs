// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1256InvalidArgumentNullCheckTests : AbstractCSharpDiagnosticVerifier<InvalidArgumentNullCheckAnalyzer, InvalidArgumentNullCheckCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.InvalidArgumentNullCheck;

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.InvalidArgumentNullCheck)]
    public async Task Test_Method_OptionalParameter()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    void M(string p = null)
    {
        [|if|] (p == null)
        {
            throw new ArgumentNullException(nameof(p));
        }

        M(p);
    }
}
", @"
using System;

class C
{
    void M(string p = null)
    {

        M(p);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.InvalidArgumentNullCheck)]
    public async Task Test_Method_NullableReferenceTypeParameter()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;

#nullable enable

class C
{
    void M(string? p)
    {
        [|if|] (p == null)
        {
            throw new ArgumentNullException(nameof(p));
        }

        M(p);
    }
}
", @"
using System;

#nullable enable

class C
{
    void M(string? p)
    {

        M(p);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.InvalidArgumentNullCheck)]
    public async Task Test_Method_OptionalParameter_ThrowIfNull()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    void M(string p = null)
    {
        [|ArgumentNullException.ThrowIfNull(p);|]

        M(p);
    }
}
", @"
using System;

class C
{
    void M(string p = null)
    {

        M(p);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.InvalidArgumentNullCheck)]
    public async Task Test_Method_NullableReferenceTypeParameter_ThrowIfNull()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;

#nullable enable

class C
{
    void M(string? p)
    {
        [|ArgumentNullException.ThrowIfNull(p);|]

        M(p);
    }
}
", @"
using System;

#nullable enable

class C
{
    void M(string? p)
    {

        M(p);
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.InvalidArgumentNullCheck)]
    public async Task Test_Constructor_OptionalParameter()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    C(string p = null)
    {
        [|if|] (p == null)
        {
            throw new ArgumentNullException(nameof(p));
        }

        string x = null;
    }
}
", @"
using System;

class C
{
    C(string p = null)
    {

        string x = null;
    }
}
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.InvalidArgumentNullCheck)]
    public async Task Test_Constructor_NullableReferenceTypeParameter()
    {
        await VerifyDiagnosticAndFixAsync(@"
using System;

#nullable enable

class C
{
    C(string? p)
    {
        [|if|] (p == null)
        {
            throw new ArgumentNullException(nameof(p));
        }

        string x = """";
    }
}
", @"
using System;

#nullable enable

class C
{
    C(string? p)
    {

        string x = """";
    }
}
");
    }
}
