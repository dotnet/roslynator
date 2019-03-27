// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1043RemovePartialModifierFromTypeWithSinglePartTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.RemovePartialModifierFromTypeWithSinglePart;

        public override DiagnosticAnalyzer Analyzer { get; } = new RemovePartialModifierFromTypeWithSinglePartAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new RemovePartialModifierFromTypeWithSinglePartCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemovePartialModifierFromTypeWithSinglePart)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
public [|partial|] class Foo
{
}
", @"
public class Foo
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemovePartialModifierFromTypeWithSinglePart)]
        public async Task TestNoDiagnostic_NoPartialModifier()
        {
            await VerifyNoDiagnosticAsync(@"
public class Foo
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemovePartialModifierFromTypeWithSinglePart)]
        public async Task TestNoDiagnostic_PartialModifierWithMultipleParts()
        {
            await VerifyNoDiagnosticAsync(@"
public partial class Foo
{
}

public partial class Foo
{
}
");
        }
    }
}