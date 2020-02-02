// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1037RemoveTrailingWhitespaceTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.RemoveTrailingWhitespace;

        public override DiagnosticAnalyzer Analyzer { get; } = new WhitespaceAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new WhitespaceTriviaCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveTrailingWhitespace)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"[| |]
class C[|  |]
{
[|
   |]
}
", @"
class C
{

}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.RemoveTrailingWhitespace)]
        public async Task Test_DocumentationComment()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    /// <summary>[| |]
    /// x[|  |]
    /// </summary>[|   |]
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
    }
}
