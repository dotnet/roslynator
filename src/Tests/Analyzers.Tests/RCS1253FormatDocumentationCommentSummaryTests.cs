// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.CodeFixes;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1253FormatDocumentationCommentSummaryTests : AbstractCSharpDiagnosticVerifier<FormatDocumentationCommentSummaryAnalyzer, SingleLineDocumentationCommentTriviaCodeFixProvider>
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.FormatDocumentationCommentSummary;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FormatDocumentationCommentSummary)]
        public async Task Test_ToMultiLine()
        {
            await VerifyDiagnosticAndFixAsync(@"
/// [|<summary>a<code>b</code>c</summary>|]
class C
{
}
", @"
/// <summary>
/// a<code>b</code>c
/// </summary>
class C
{
}
", options: Options.AddConfigOption(ConfigOptionKeys.DocCommentSummaryStyle, ConfigOptionValues.DocCommentSummaryStyle_MultiLine));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FormatDocumentationCommentSummary)]
        public async Task Test_EmptySummary_ToMultiLine()
        {
            await VerifyDiagnosticAndFixAsync(@"
/// [|<summary></summary>|]
class C
{
}
", @"
/// <summary>
/// 
/// </summary>
class C
{
}
", options: Options.AddConfigOption(ConfigOptionKeys.DocCommentSummaryStyle, ConfigOptionValues.DocCommentSummaryStyle_MultiLine));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FormatDocumentationCommentSummary)]
        public async Task Test_Tab_ToMultiLine()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
	/// [|<summary>x</summary>|]
	void M()
	{
	}
}
", @"
class C
{
	/// <summary>
	/// x
	/// </summary>
	void M()
	{
	}
}
", options: Options.AddConfigOption(ConfigOptionKeys.DocCommentSummaryStyle, ConfigOptionValues.DocCommentSummaryStyle_MultiLine));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FormatDocumentationCommentSummary)]
        public async Task Test_ToSingleLine()
        {
            await VerifyDiagnosticAndFixAsync(@"
/// [|<summary>
/// a<code>b</code>c
/// </summary>|]
class C
{
}
", @"
/// <summary>a<code>b</code>c</summary>
class C
{
}
", options: Options.AddConfigOption(ConfigOptionKeys.DocCommentSummaryStyle, ConfigOptionValues.DocCommentSummaryStyle_SingleLine));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FormatDocumentationCommentSummary)]
        public async Task Test_EmptySummary_ToSingleLine()
        {
            await VerifyDiagnosticAndFixAsync(@"
/// [|<summary>
/// 
/// </summary>|]
class C
{
}
", @"
/// <summary></summary>
class C
{
}
", options: Options.AddConfigOption(ConfigOptionKeys.DocCommentSummaryStyle, ConfigOptionValues.DocCommentSummaryStyle_SingleLine));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FormatDocumentationCommentSummary)]
        public async Task Test_Tab_ToSingleLine()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
	/// [|<summary>
	/// x
	/// </summary>|]
	void M()
	{
	}
}
", @"
class C
{
	/// <summary>x</summary>
	void M()
	{
	}
}
", options: Options.AddConfigOption(ConfigOptionKeys.DocCommentSummaryStyle, ConfigOptionValues.DocCommentSummaryStyle_SingleLine));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FormatDocumentationCommentSummary)]
        public async Task TestNoDiagnostic_MultiLine()
        {
            await VerifyNoDiagnosticAsync(@"
/// <summary>
/// x
/// </summary>
class C
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FormatDocumentationCommentSummary)]
        public async Task TestNoDiagnostic_EmptySummary_ToMultiLine()
        {
            await VerifyNoDiagnosticAsync(@"
/// <summary>
/// </summary>
class C
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FormatDocumentationCommentSummary)]
        public async Task TestNoDiagnostic_SingleLine()
        {
            await VerifyNoDiagnosticAsync(@"
/// <summary>x</summary>
class C
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FormatDocumentationCommentSummary)]
        public async Task TestNoDiagnostic_ToSingleLine()
        {
            await VerifyNoDiagnosticAsync(@"
/// <summary>
/// x
/// x
/// </summary>
class C
{
}
", options: Options.AddConfigOption(ConfigOptionKeys.DocCommentSummaryStyle, ConfigOptionValues.DocCommentSummaryStyle_SingleLine));
        }
    }
}
