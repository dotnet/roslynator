// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Analysis.MakeMemberReadOnly;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1170UseReadOnlyAutoPropertyTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UseReadOnlyAutoProperty;

        public override DiagnosticAnalyzer Analyzer { get; } = new UseReadOnlyAutoPropertyAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new MemberDeclarationCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseReadOnlyAutoProperty)]
        public async Task Test_AssignedInInstanceConstructor()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    public C()
    {
        P = this;
    }

    public C P { get; [|private set;|] }
}
", @"
class C
{
    public C()
    {
        P = this;
    }

    public C P { get; }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseReadOnlyAutoProperty)]
        public async Task TestNoDiagnostic_VariablePropertyAssignedInConstructor()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    public C(C c)
    {
        c.P1 = this;
        c.P1.P2 = this;
    }

    public C P1 { get; private set; }
    public C P2 { get; private set; }
}
");
        }
    }
}
