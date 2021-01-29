// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace Roslynator.CodeAnalysis.CSharp.Tests
{
    public class RCS9010SpecifyExportCodeRefactoringProviderAttributeNameTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.SpecifyExportCodeRefactoringProviderAttributeName;

        protected override DiagnosticAnalyzer Analyzer { get; } = new NamedTypeSymbolAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new AttributeCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SpecifyExportCodeRefactoringProviderAttributeName)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;

[[|ExportCodeRefactoringProvider|](LanguageNames.CSharp)]
abstract class C : CodeRefactoringProvider
{
}
", @"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;

[ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(C))]
abstract class C : CodeRefactoringProvider
{
}
");
        }
    }
}
