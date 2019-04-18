// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1134RemoveRedundantStatementTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.RemoveRedundantStatement;

        public override DiagnosticAnalyzer Analyzer { get; } = new RemoveRedundantStatementAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new StatementCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantStatement)]
        public async Task Test_SimpleIf_ReturnNull()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    string M()
    {
        bool f = false;

        if (f)
        {
            [|return null;|]
        }

        return null;
    }
}
", @"
class C
{
    string M()
    {
        bool f = false;

        if (f)
        {
        }

        return null;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantStatement)]
        public async Task Test_SimpleIf_ReturnDefault()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    string M()
    {
        bool f = false;

        if (f)
        {
            [|return default;|]
        }

        return default;
    }
}
", @"
class C
{
    string M()
    {
        bool f = false;

        if (f)
        {
        }

        return default;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantStatement)]
        public async Task Test_SimpleIf_ReturnTrue()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    bool M()
    {
        bool f = false;

        if (f)
        {
            [|return true;|]
        }

        return true;
    }
}
", @"
class C
{
    bool M()
    {
        bool f = false;

        if (f)
        {
        }

        return true;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantStatement)]
        public async Task Test_SimpleIf_ReturnFalse()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    bool M()
    {
        bool f = false;

        if (f)
        {
            [|return false;|]
        }

        return false;
    }
}
", @"
class C
{
    bool M()
    {
        bool f = false;

        if (f)
        {
        }

        return false;
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveRedundantStatement)]
        public async Task TestNoDiagnostic_SimpleIf_ExpressionsAreNotEquivalent()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    bool M()
    {
        bool f = false;

        if (f)
        {
            return true;
        }

        return false;
    }
}
");
        }
    }
}
