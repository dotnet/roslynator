// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CodeAnalysis.CSharp.Tests;

public class RCS9010SpecifyExportCodeRefactoringProviderAttributeNameTests : AbstractCSharpDiagnosticVerifier<NamedTypeSymbolAnalyzer, AttributeCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = CodeAnalysisDiagnosticRules.SpecifyExportCodeRefactoringProviderAttributeName;

    [Fact, Trait(Traits.Analyzer, CodeAnalysisDiagnosticIdentifiers.SpecifyExportCodeRefactoringProviderAttributeName)]
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
