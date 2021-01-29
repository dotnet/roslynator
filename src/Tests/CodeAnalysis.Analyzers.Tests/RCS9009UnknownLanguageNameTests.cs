// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace Roslynator.CodeAnalysis.CSharp.Tests
{
    public class RCS9009UnknownLanguageNameTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.UnknownLanguageName;

        protected override DiagnosticAnalyzer Analyzer { get; } = new NamedTypeSymbolAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new AttributeArgumentCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnknownLanguageName)]
        public async Task Test_DiagnosticAnalyzer()
        {
            await VerifyDiagnosticAndFixAsync(@"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer([|""Foo""|])]
abstract class C : DiagnosticAnalyzer
{
}
", @"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
abstract class C : DiagnosticAnalyzer
{
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticIdentifiers.UnknownLanguageName, "C#"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnknownLanguageName)]
        public async Task Test_DiagnosticAnalyzer_ParamArray()
        {
            await VerifyDiagnosticAndFixAsync(@"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.FSharp, [|""Foo""|], LanguageNames.VisualBasic, [|""Bar""|])]
abstract class C : DiagnosticAnalyzer
{
}
", @"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.FSharp, LanguageNames.CSharp, LanguageNames.VisualBasic, LanguageNames.CSharp)]
abstract class C : DiagnosticAnalyzer
{
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticIdentifiers.UnknownLanguageName, "C#"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnknownLanguageName)]
        public async Task Test_CodeFixProvider()
        {
            await VerifyDiagnosticAndFixAsync(@"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

[ExportCodeFixProvider([|""Foo""|])]
abstract class C : CodeFixProvider
{
}
", @"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp)]
abstract class C : CodeFixProvider
{
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticIdentifiers.UnknownLanguageName, "C#"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnknownLanguageName)]
        public async Task Test_CodeRefactoringProvider()
        {
            await VerifyDiagnosticAndFixAsync(@"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;

[ExportCodeRefactoringProvider([|""Foo""|])]
abstract class C : CodeRefactoringProvider
{
}
", @"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;

[ExportCodeRefactoringProvider(LanguageNames.CSharp)]
abstract class C : CodeRefactoringProvider
{
}
", equivalenceKey: EquivalenceKey.Create(DiagnosticIdentifiers.UnknownLanguageName, "C#"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.UnknownLanguageName)]
        public async Task TestNoDiagnostic()
        {
            await VerifyNoDiagnosticAsync(@"
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
abstract class C : DiagnosticAnalyzer
{
}
");
        }
    }
}
