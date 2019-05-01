// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Roslynator.CSharp.Tests;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1181ConvertCommentToDocumentationCommentTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.ConvertCommentToDocumentationComment;

        public override DiagnosticAnalyzer Analyzer { get; } = new ConvertCommentToDocumentationCommentAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new MemberDeclarationCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConvertCommentToDocumentationComment)]
        public async Task Test_SingleComment()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    [|// x|]
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
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConvertCommentToDocumentationComment)]
        public async Task Test_MultipleComments()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    [|// x
    // x|]
    void M()
    {
    }
}
", @"
class C
{
    /// <summary>
    /// x
    /// x
    /// </summary>
    void M()
    {
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConvertCommentToDocumentationComment)]
        public async Task Test_TrailingComment()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M() [|// x|]
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
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConvertCommentToDocumentationComment)]
        public async Task TestNoDiagnostic_HasDocumentationComment()
        {
            var options = (CSharpCodeVerificationOptions)Options;

            await VerifyNoDiagnosticAsync(@"
class C
{
    /// <summary>
    /// x
    /// </summary>
    void M()
    {
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConvertCommentToDocumentationComment)]
        public async Task TestNoDiagnostic_HasDocumentationCommentAndTrailingComment()
        {
            var options = (CSharpCodeVerificationOptions)Options;

            await VerifyNoDiagnosticAsync(@"
class C
{
    /// <summary>
    /// x
    /// </summary>
    void M() // x
    {
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.ConvertCommentToDocumentationComment)]
        public async Task TestNoDiagnostic_HasDocumentationComment_DocumentationModeIsEqualToNone()
        {
            var options = (CSharpCodeVerificationOptions)Options;

            await VerifyNoDiagnosticAsync(@"
class C
{
    /// <summary>
    /// x
    /// </summary>
    void M()
    {
    }
}
", options: options.WithParseOptions(options.ParseOptions.WithDocumentationMode(DocumentationMode.None)));
        }
    }
}
