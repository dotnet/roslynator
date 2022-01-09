// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Formatting.CodeFixes.CSharp;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class RCS0023FormatTypeDeclarationBracesTests : AbstractCSharpDiagnosticVerifier<FormatTypeDeclarationBracesAnalyzer, MemberDeclarationCodeFixProvider>
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.FormatTypeDeclarationBraces;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FormatTypeDeclarationBraces)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
[|{|]}
", @"
class C
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FormatTypeDeclarationBraces)]
        public async Task Test_WithWhitespace()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
[|{|] }
", @"
class C
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FormatTypeDeclarationBraces)]
        public async Task TestNoDiagnostic_EmptyLine()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{

}
");
        }
    }
}
