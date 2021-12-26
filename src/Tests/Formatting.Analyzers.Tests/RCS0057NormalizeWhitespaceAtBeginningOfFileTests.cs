// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Formatting.CodeFixes.CSharp;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class RCS0057NormalizeWhitespaceAtBeginningOfFileTests : AbstractCSharpDiagnosticVerifier<NormalizeWhitespaceAtBeginningOfFileAnalyzer, CompilationUnitCodeFixProvider>
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.NormalizeWhitespaceAtBeginningOfFile;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtBeginningOfFile)]
        public async Task Test()
        {
            await VerifyDiagnosticAndFixAsync(@"[||]
class C
{
}", @"class C
{
}");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtBeginningOfFile)]
        public async Task Test_TrailingWhitespace()
        {
            await VerifyDiagnosticAndFixAsync(@"[||] 
class C
{
}", @"class C
{
}");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtBeginningOfFile)]
        public async Task Test_TrailingMany()
        {
            await VerifyDiagnosticAndFixAsync(@"[||] 

  
class C
{
}", @"class C
{
}");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtBeginningOfFile)]
        public async Task Test_SingleLineComment()
        {
            await VerifyDiagnosticAndFixAsync(@"[||]
//x
class C
{
}", @"//x
class C
{
}");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtBeginningOfFile)]
        public async Task Test_MultiLineComment()
        {
            await VerifyDiagnosticAndFixAsync(@"[||]
/** x **/
class C
{
}", @"/** x **/
class C
{
}");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtBeginningOfFile)]
        public async Task Test_Directive()
        {
            await VerifyDiagnosticAndFixAsync(@"[||]
#region
class C
{
}
#endregion", @"#region
class C
{
}
#endregion");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtBeginningOfFile)]
        public async Task Test_WhitespaceOnly()
        {
            await VerifyDiagnosticAndFixAsync(@"[||] class C
{
}", @"class C
{
}");
        }
    }
}
