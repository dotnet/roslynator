// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Formatting.CodeFixes.CSharp;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests
{
    public class RCS0054FixFormattingOfCallChainTests : AbstractCSharpDiagnosticVerifier<FixFormattingOfCallChainAnalyzer, FixFormattingOfCallChainCodeFixProvider>
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.FixFormattingOfCallChain;

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixFormattingOfCallChain)]
        public async Task Test_WrongIndentation()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    C M() 
    {
        var x = new C();

        return [|x.M().M()
        .M()|];
    }
}
", @"
class C
{
    C M() 
    {
        var x = new C();

        return x.M().M()
            .M();
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixFormattingOfCallChain)]
        public async Task Test_WrongIndentation_NullConditionalOperator()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    C M() 
    {
        var x = new C();

        return [|x.M().M()
        ?.M()|];
    }
}
", @"
class C
{
    C M() 
    {
        var x = new C();

        return x.M().M()
            ?.M();
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixFormattingOfCallChain)]
        public async Task Test_Invocation_NoIndentation()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    C M() 
    {
        var x = new C();

        return [|x.M()
.M()|];
    }
}
", @"
class C
{
    C M() 
    {
        var x = new C();

        return x.M()
            .M();
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixFormattingOfCallChain)]
        public async Task Test_Invocation_NoIndentation_EmptyLineBetween()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    C M() 
    {
        var x = new C();

        return [|x.M()
.M()|];
    }
}
", @"
class C
{
    C M() 
    {
        var x = new C();

        return x.M()
            .M();
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixFormattingOfCallChain)]
        public async Task Test_Invocation_WrapAndIndent()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    C M() 
    {
        var x = new C();

        return [|x.M()
            .M()?.M()|];
    }
}
", @"
class C
{
    C M() 
    {
        var x = new C();

        return x.M()
            .M()?
            .M();
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixFormattingOfCallChain)]
        public async Task Test_Invocation_WrapAndIndent_NullConditionalOperator_NewLineBefore()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    C M() 
    {
        var x = new C();

        return [|x.M()
            .M()?.M()|];
    }
}
", @"
class C
{
    C M() 
    {
        var x = new C();

        return x.M()
            .M()
            ?.M();
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.NullConditionalOperatorNewLine, "before"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixFormattingOfCallChain)]
        public async Task Test_Invocation_WrapAndIndent_NullConditionalOperator_NewLineAfter()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    C M() 
    {
        var x = new C();

        return [|x.M()
            .M()?.M()|];
    }
}
", @"
class C
{
    C M() 
    {
        var x = new C();

        return x.M()
            .M()?
            .M();
    }
}
", options: Options.AddConfigOption(ConfigOptionKeys.NullConditionalOperatorNewLine, "after"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixFormattingOfCallChain)]
        public async Task Test_Invocation_EmptyLineBetween()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    C M() 
    {
        var x = new C();

        return [|x.M()
    
.M()|];
    }
}
", @"
class C
{
    C M() 
    {
        var x = new C();

        return x.M()
            .M();
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixFormattingOfCallChain)]
        public async Task Test_Invocation_CommentBetween()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    C M() 
    {
        var x = new C();

        return [|x.M()
        // x
        .M()|];
    }
}
", @"
class C
{
    C M() 
    {
        var x = new C();

        return x.M()
        // x
            .M();
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixFormattingOfCallChain)]
        public async Task Test_Invocation_IndentationsDiffer()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    C M(string s) 
    {
        var x = new C();

        return [|x.M("""")
        .M(new string(
            ' ',
            1))
    .M(new string(
        ' ',
        1))
            .M(new string(
                ' ',
                1))|];
    }
}
", @"
class C
{
    C M(string s) 
    {
        var x = new C();

        return x.M("""")
            .M(new string(
                ' ',
                1))
            .M(new string(
                ' ',
                1))
            .M(new string(
                ' ',
                1));
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixFormattingOfCallChain)]
        public async Task Test_TopLevelStatement()
        {
            await VerifyNoDiagnosticAsync(@"
using System;

Console
    .WriteLine();

partial class Program
{
    void Main(string[] args)
    {
    }
}
", options: Options.WithCompilationOptions(Options.CompilationOptions.WithOutputKind(OutputKind.ConsoleApplication)));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixFormattingOfCallChain)]
        public async Task TestNoDiagnostic()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    C M() 
    {
        var x = new C();

        return x.M().M();
    }
}
");
        }
    }
}
