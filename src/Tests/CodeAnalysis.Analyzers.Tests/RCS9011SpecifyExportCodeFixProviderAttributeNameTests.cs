// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CodeAnalysis.CSharp.Tests;

public class RCS9011SpecifyExportCodeFixProviderAttributeNameTests : AbstractCSharpDiagnosticVerifier<NamedTypeSymbolAnalyzer, AttributeCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = CodeAnalysisDiagnosticRules.SpecifyExportCodeFixProviderAttributeName;

    [Fact, Trait(Traits.Analyzer, CodeAnalysisDiagnosticIds.SpecifyExportCodeFixProviderAttributeName)]
    public async Task Test()
    {
        await VerifyDiagnosticAndFixAsync(@"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

[[|ExportCodeFixProvider|](LanguageNames.CSharp)]
abstract class C : CodeFixProvider
{
}
", @"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(C))]
abstract class C : CodeFixProvider
{
}
");
    }
}
