// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Formatting.CodeFixes.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class UseSpacesInsteadOfTabTests : AbstractCSharpFixVerifier
    {
        private readonly ReplaceTabWithSpacesCodeFixProvider _fixProvider = new ReplaceTabWithSpacesCodeFixProvider();

        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UseSpacesInsteadOfTab;

        public override DiagnosticAnalyzer Analyzer { get; } = new UseSpacesInsteadOfTabAnalyzer();

        public override CodeFixProvider FixProvider
        {
            get { return _fixProvider; }
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseSpacesInsteadOfTab)]
        public async Task Test_FourSpaces()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
[|	|]void M()
[|	|]{
[|		|]M();
[|	|]}
}
", @"
class C
{
    void M()
    {
        M();
    }
}
", equivalenceKey: _fixProvider.FourSpacesEquivalenceKey);
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UseSpacesInsteadOfTab)]
        public async Task TestNoDiagnostic()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        M();
    }
}
");
        }
    }
}
