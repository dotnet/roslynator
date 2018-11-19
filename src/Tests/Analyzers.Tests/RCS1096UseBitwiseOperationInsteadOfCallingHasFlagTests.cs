// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1096UseBitwiseOperationInsteadOfCallingHasFlagTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UseBitwiseOperationInsteadOfCallingHasFlag;

        public override DiagnosticAnalyzer Analyzer { get; } = new InvocationExpressionAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new InvocationExpressionCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseBitwiseOperationInsteadOfCallingHasFlag)]
        public async Task Test_HasFlag()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    void M()
    {
        var options = StringSplitOptions.None;

        if ([|options.HasFlag(StringSplitOptions.RemoveEmptyEntries)|]) { }
    }
}
", @"
using System;

class C
{
    void M()
    {
        var options = StringSplitOptions.None;

        if ((options & StringSplitOptions.RemoveEmptyEntries) != 0) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseBitwiseOperationInsteadOfCallingHasFlag)]
        public async Task Test_NotHasFlag()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    void M()
    {
        var options = StringSplitOptions.None;

        if (![|options.HasFlag(StringSplitOptions.RemoveEmptyEntries)|]) { }
    }
}
", @"
using System;

class C
{
    void M()
    {
        var options = StringSplitOptions.None;

        if ((options & StringSplitOptions.RemoveEmptyEntries) == 0) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseBitwiseOperationInsteadOfCallingHasFlag)]
        public async Task Test_HasFlag_EqualsTrue()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    void M()
    {
        var options = StringSplitOptions.None;

        if ([|options.HasFlag(StringSplitOptions.RemoveEmptyEntries)|] == true) { }
    }
}
", @"
using System;

class C
{
    void M()
    {
        var options = StringSplitOptions.None;

        if ((options & StringSplitOptions.RemoveEmptyEntries) != 0) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseBitwiseOperationInsteadOfCallingHasFlag)]
        public async Task Test_HasFlag_EqualsFalse()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    void M()
    {
        var options = StringSplitOptions.None;

        if ([|options.HasFlag(StringSplitOptions.RemoveEmptyEntries)|] == false) { }
    }
}
", @"
using System;

class C
{
    void M()
    {
        var options = StringSplitOptions.None;

        if ((options & StringSplitOptions.RemoveEmptyEntries) == 0) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseBitwiseOperationInsteadOfCallingHasFlag)]
        public async Task Test_HasFlag_WithTrivia()
        {
            await VerifyDiagnosticAndFixAsync(@"
using System;

class C
{
    void M()
    {
        var options = StringSplitOptions.None;

        if ( /*lt*/ [|options.HasFlag(StringSplitOptions.RemoveEmptyEntries /*tt*/ )|].Equals(true)) { }
    }
}
", @"
using System;

class C
{
    void M()
    {
        var options = StringSplitOptions.None;

        if ( /*lt*/ ((options & StringSplitOptions.RemoveEmptyEntries /*tt*/ ) != 0).Equals(true)) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseBitwiseOperationInsteadOfCallingHasFlag)]
        public async Task TestNoDiagnostic_TypeIsSystemEnum()
        {
            await VerifyNoDiagnosticAsync(@"
using System;

class C
{
    void M()
    {
        var @enum = default(Enum);
        var options = StringSplitOptions.None;

        if (options.HasFlag(@enum)) { }

        if (@enum.HasFlag(options)) { }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseBitwiseOperationInsteadOfCallingHasFlag)]
        public async Task TestNoDiagnostic_ConditionalAccess()
        {
            await VerifyNoDiagnosticAsync(@"
using System;

class C
{
    StringSplitOptions P { get; }

    void M()
    {
        C c = null;

        if (c?.P.HasFlag(StringSplitOptions.RemoveEmptyEntries) == true) { }
    }
}
");
        }
    }
}
