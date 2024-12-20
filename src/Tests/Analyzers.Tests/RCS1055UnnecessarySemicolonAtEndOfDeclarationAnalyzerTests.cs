// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests;

public class RCS1055UnnecessarySemicolonAtEndOfDeclarationAnalyzerTests : AbstractCSharpDiagnosticVerifier<UnnecessarySemicolonAtEndOfDeclarationAnalyzer, MemberDeclarationCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.UnnecessarySemicolonAtEndOfDeclaration;

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UnnecessarySemicolonAtEndOfDeclaration)]
    public async Task TestNoDiagnostic_Class()
    {
        await VerifyNoDiagnosticAsync(@"
class C;
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UnnecessarySemicolonAtEndOfDeclaration)]
    public async Task TestNoDiagnostic_Struct()
    {
        await VerifyNoDiagnosticAsync(@"
struct C;
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UnnecessarySemicolonAtEndOfDeclaration)]
    public async Task TestNoDiagnostic_Interface()
    {
        await VerifyNoDiagnosticAsync(@"
interface C;
");
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIds.UnnecessarySemicolonAtEndOfDeclaration)]
    public async Task TestNoDiagnostic_Enum()
    {
        await VerifyNoDiagnosticAsync(@"
enum E;
");
    }
}
