// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1139AddSummaryElementToDocumentationCommentTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.AddSummaryElementToDocumentationComment;

        public override DiagnosticAnalyzer Analyzer { get; } = new SingleLineDocumentationCommentTriviaAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new DocumentationCommentCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddSummaryElementToDocumentationComment)]
        public async Task Test_Class()
        {
            await VerifyDiagnosticAndFixAsync(@"
///[| <typeparam name=""T""></typeparam>
|]class C<T>
{
}
", @"
/// <summary>
/// 
/// </summary>
/// <typeparam name=""T""></typeparam>
class C<T>
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddSummaryElementToDocumentationComment)]
        public async Task TestNoDiagnostic_PartialClassWithContentElement()
        {
            await VerifyNoDiagnosticAsync(@"
/// <summary>
/// x
/// </summary>
partial class C
{
}

/// <content>
/// x
/// </content>
partial class C
{
}
");
        }
    }
}
