// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Analysis.Documentation;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1141AddParamElementToDocumentationCommentTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.AddParamElementToDocumentationComment;

        public override DiagnosticAnalyzer Analyzer { get; } = new AddParamElementToDocumentationCommentAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new SingleLineDocumentationCommentTriviaCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddParamElementToDocumentationComment)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    ///[| <summary>
    /// 
    /// </summary>
    /// <param name=""p""></param>
    /// <param name=""p3""></param>
    /// <param name=""p5""></param>
|]    void M(object p, object p2, object p3, object p4, object p5)
    {
    }
}
", @"
class C
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name=""p""></param>
    /// <param name=""p2""></param>
    /// <param name=""p3""></param>
    /// <param name=""p4""></param>
    /// <param name=""p5""></param>
    void M(object p, object p2, object p3, object p4, object p5)
    {
    }
}
");
        }
    }
}
