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
    public class RCS1142AddTypeParamElementToDocumentationCommentTests : AbstractCSharpCodeFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.AddTypeParamElementToDocumentationComment;

        public override DiagnosticAnalyzer Analyzer { get; } = new AddTypeParamElementToDocumentationCommentAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new SingleLineDocumentationCommentTriviaCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.AddTypeParamElementToDocumentationComment)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"
///[| <summary>
/// 
/// </summary>
/// <typeparam name=""T2""></typeparam>
|]class C<T1, T2, T3>
{
}
", @"
/// <summary>
/// 
/// </summary>
/// <typeparam name=""T1""></typeparam>
/// <typeparam name=""T2""></typeparam>
/// <typeparam name=""T3""></typeparam>
class C<T1, T2, T3>
{
}
");
        }
    }
}
