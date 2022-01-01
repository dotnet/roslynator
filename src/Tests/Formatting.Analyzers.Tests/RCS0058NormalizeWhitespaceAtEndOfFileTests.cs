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
        public async Task Test_NewLineAtEndOfFile()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
}[||]", @"
class C
{
}
", options: Options.AddConfigOption(ConfigOptionKeys.NewLineAtEndOfFile, true));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task Test_NewLineAtEndOfFile_EmptyFile()
        {
            await VerifyDiagnosticAndFixAsync(@"
[||]",
"", options: Options.AddConfigOption(ConfigOptionKeys.NewLineAtEndOfFile, true));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task Test_NewLineAtEndOfFile_TrailingWhitespace()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
} [||]", @"
class C
{
} 
", options: Options.AddConfigOption(ConfigOptionKeys.NewLineAtEndOfFile, true));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task Test_NewLineAtEndOfFile_TrailingMany()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
} [||]", @"
class C
{
} 
", options: Options.AddConfigOption(ConfigOptionKeys.NewLineAtEndOfFile, true));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task Test_NewLineAtEndOfFile_SingleLineComment()
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
", options: Options.AddConfigOption(ConfigOptionKeys.NewLineAtEndOfFile, true));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task Test_NewLineAtEndOfFile_MultiLineComment()
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
", options: Options.AddConfigOption(ConfigOptionKeys.NewLineAtEndOfFile, true));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task Test_NewLineAtEndOfFile_Directive()
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
", options: Options.AddConfigOption(ConfigOptionKeys.NewLineAtEndOfFile, true));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task Test_NoNewLineAtEndOfFile()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
}
[||]", @"
class C
{
}", options: Options.AddConfigOption(ConfigOptionKeys.NewLineAtEndOfFile, false));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task Test_NoNewLineAtEndOfFile_EmptyFile()
        {
            await VerifyDiagnosticAndFixAsync(@"
[||]",
"", options: Options.AddConfigOption(ConfigOptionKeys.NewLineAtEndOfFile, false));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task Test_NoNewLineAtEndOfFile_TrailingWhitespace()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
}
 [||]", @"
class C
{
}", options: Options.AddConfigOption(ConfigOptionKeys.NewLineAtEndOfFile, false));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task Test_NoNewLineAtEndOfFile_TrailingMany()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
}
 

  [||]", @"
class C
{
}", options: Options.AddConfigOption(ConfigOptionKeys.NewLineAtEndOfFile, false));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task Test_NoNewLineAtEndOfFile_SingleLineComment()
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
//x", options: Options.AddConfigOption(ConfigOptionKeys.NewLineAtEndOfFile, false));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task Test_NoNewLineAtEndOfFile_MultiLineComment()
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
/** x **/", options: Options.AddConfigOption(ConfigOptionKeys.NewLineAtEndOfFile, false));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task Test_NoNewLineAtEndOfFile_Directive()
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
#endif", options: Options.AddConfigOption(ConfigOptionKeys.NewLineAtEndOfFile, false));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task TestNoDiagnostic_NewLineAtEndOfFile()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.NormalizeWhitespaceAtEndOfFile)]
        public async Task TestNoDiagnostic_NoNewLineAtEndOfFile()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
}", options: Options.AddConfigOption(ConfigOptionKeys.NewLineAtEndOfFile, false));
        }
    }
}
