// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Formatting.CodeFixes.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class AddNewLineAfterOpeningBraceOfTypeDeclarationTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.AddNewLineAfterOpeningBraceOfTypeDeclaration;

        public override DiagnosticAnalyzer Analyzer { get; } = new AddNewLineAfterOpeningBraceOfTypeDeclarationAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new MemberDeclarationCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineAfterOpeningBraceOfTypeDeclaration)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{[||]}
", @"
class C
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineAfterOpeningBraceOfTypeDeclaration)]
        public async Task Test_WithWhitespace()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{[||] }
", @"
class C
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddNewLineAfterOpeningBraceOfTypeDeclaration)]
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
