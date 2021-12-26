// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Formatting.CodeFixes.CSharp;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class RCS0058NormalizeWhitespaceAtEndOfFileTests : AbstractCSharpDiagnosticVerifier<NormalizeWhitespaceAtEndOfFileAnalyzer, NormalizeWhitespaceAtEndOfFileCodeFixProvider>
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.NormalizeWhitespaceAtEndOfFile;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task Test_NewlineAtEndOfFile()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
}[||]", @"
class C
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task Test_NewlineAtEndOfFile_EmptyFile()
        {
            await VerifyDiagnosticAndFixAsync(@"
[||]",
"");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task Test_NewlineAtEndOfFile_TrailingWhitespace()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
} [||]", @"
class C
{
} 
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task Test_NewlineAtEndOfFile_TrailingMany()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
} [||]", @"
class C
{
} 
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task Test_NewlineAtEndOfFile_SingleLineComment()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
}
//x[||]", @"
class C
{
}
//x
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task Test_NewlineAtEndOfFile_MultiLineComment()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
}
/** x **/[||]", @"
class C
{
}
/** x **/
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task Test_NewlineAtEndOfFile_Directive()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
}
#if DEBUG
#endif[||]", @"
class C
{
}
#if DEBUG
#endif
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task Test_NoNewlineAtEndOfFile()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
}
[||]", @"
class C
{
}", options: Options.EnableConfigOption(GlobalOptions.PreferNoNewLineAtEndOfFile.Key));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task Test_NoNewlineAtEndOfFile_EmptyFile()
        {
            await VerifyDiagnosticAndFixAsync(@"
[||]",
"", options: Options.EnableConfigOption(GlobalOptions.PreferNoNewLineAtEndOfFile.Key));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task Test_NoNewlineAtEndOfFile_TrailingWhitespace()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
}
 [||]", @"
class C
{
}", options: Options.EnableConfigOption(GlobalOptions.PreferNoNewLineAtEndOfFile.Key));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task Test_NoNewlineAtEndOfFile_TrailingMany()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
}
 

  [||]", @"
class C
{
}", options: Options.EnableConfigOption(GlobalOptions.PreferNoNewLineAtEndOfFile.Key));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task Test_NoNewlineAtEndOfFile_SingleLineComment()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
}
//x
[||]", @"
class C
{
}
//x", options: Options.EnableConfigOption(GlobalOptions.PreferNoNewLineAtEndOfFile.Key));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task Test_NoNewlineAtEndOfFile_MultiLineComment()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
}
/** x **/
[||]", @"
class C
{
}
/** x **/", options: Options.EnableConfigOption(GlobalOptions.PreferNoNewLineAtEndOfFile.Key));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task Test_NoNewlineAtEndOfFile_Directive()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
}
#if DEBUG
#endif
[||]", @"
class C
{
}
#if DEBUG
#endif", options: Options.EnableConfigOption(GlobalOptions.PreferNoNewLineAtEndOfFile.Key));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task TestNoDiagnostic_NewlineAtEndOfFile()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task TestNoDiagnostic_NoNewlineAtEndOfFile()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
}", options: Options.EnableConfigOption(GlobalOptions.PreferNoNewLineAtEndOfFile.Key));
        }
    }
}
