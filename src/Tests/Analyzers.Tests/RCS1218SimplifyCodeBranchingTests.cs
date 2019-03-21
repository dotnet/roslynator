// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.CodeFixes;
using Xunit;

namespace Roslynator.CSharp.Analysis.Tests
{
    public class RCS1218SimplifyCodeBranchingTests : AbstractCSharpFixVerifier
    {
        public override DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors.SimplifyCodeBranching;

        public override DiagnosticAnalyzer Analyzer { get; } = new SimplifyCodeBranchingAnalyzer();

        public override CodeFixProvider FixProvider { get; } = new SimplifyCodeBranchingCodeFixProvider();

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task Test_IfElse_WithBraces()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(bool f1 = false, bool f2 = false)
    {
        [|if (f1)
        {
        }
        else
        {
            M();
        }|]
    }
}
", @"
class C
{
    void M(bool f1 = false, bool f2 = false)
    {
        if (!f1)
        {
            M();
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task Test_IfElse_WithoutBraces()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(bool f1 = false, bool f2 = false)
    {
        [|if (f1)
        {
        }
        else
            M();|]
    }
}
", @"
class C
{
    void M(bool f1 = false, bool f2 = false)
    {
        if (!f1)
            M();
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task Test_IfElseIf_WithBraces()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(bool f1 = false, bool f2 = false)
    {
        [|if (f1)
        {
        }
        else if (f2)
        {
            M();
        }|]
    }
}
", @"
class C
{
    void M(bool f1 = false, bool f2 = false)
    {
        if (!f1 && f2)
        {
            M();
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task Test_IfElseIf_WithoutBraces()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(bool f1 = false, bool f2 = false)
    {
        [|if (f1)
        {
        }
        else if (f2)
            M();|]
    }
}
", @"
class C
{
    void M(bool f1 = false, bool f2 = false)
    {
        if (!f1 && f2)
            M();
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task Test_While_IfElseWithBraces()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(bool f1 = false, bool f2 = false)
    {
        while (true)
        {
            [|if (f1)
            {
                M();
            }
            else
            {
                break;
            }|]
        }
    }
}
", @"
class C
{
    void M(bool f1 = false, bool f2 = false)
    {
        while (f1)
        {
            M();
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task Test_While_IfElseWithoutBraces()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(bool f1 = false, bool f2 = false)
    {
        while (true)
        {
            [|if (f1)
                M();
            else
                break;|]
        }
    }
}
", @"
class C
{
    void M(bool f1 = false, bool f2 = false)
    {
        while (f1)
        {
            M();
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task Test_While_IfElseWithMultipleStatements()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(bool f1 = false, bool f2 = false)
    {
        while (true)
        {
            [|if (f1)
            {
                M();
                M();
            }
            else
            {
                break;
            }|]
        }
    }
}
", @"
class C
{
    void M(bool f1 = false, bool f2 = false)
    {
        while (f1)
        {
            M();
            M();
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task Test_While_EmbeddedIfElseWithSingleStatement()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(bool f1 = false, bool f2 = false)
    {
        while (true)
            [|if (f1)
            {
                M();
            }
            else
            {
                break;
            }|]
    }
}
", @"
class C
{
    void M(bool f1 = false, bool f2 = false)
    {
        while (f1)
        {
            M();
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task Test_While_EmbeddedIfElseWithMultipleStatements()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(bool f1 = false, bool f2 = false)
    {
        while (true)
            [|if (f1)
            {
                M();
                M();
            }
            else
            {
                break;
            }|]
    }
}
", @"
class C
{
    void M(bool f1 = false, bool f2 = false)
    {
        while (f1)
        {
            M();
            M();
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task Test_While_IfWithBraces_LastStatement()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool f = false;

        while (true)
        {
            M();

            [|if (f)
            {
                break;
            }|]
        }
    }
}
", @"
class C
{
    void M()
    {
        bool f = false;

        do
        {
            M();
        }
        while (!f);
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task Test_While_IfWithoutBraces_LastStatement()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(bool f1 = false)
    {
        while (true)
        {
            M();

            [|if (f1)
                break;|]
        }
    }
}
", @"
class C
{
    void M(bool f1 = false)
    {
        do
        {
            M();
        }
        while (!f1);
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task Test_While_IfWithBraces_FirstStatement()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(bool f1 = false, bool f2 = false)
    {
        while (true)
        {
            [|if (f1)
            {
                break;
            }|]
            M();
        }
    }
}
", @"
class C
{
    void M(bool f1 = false, bool f2 = false)
    {
        while (!f1)
        {
            M();
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task Test_While_IfWithoutBraces_FirstStatement()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(bool f1 = false, bool f2 = false)
    {
        while (true)
        {
            [|if (f1)
                break;|]
            M();
        }
    }
}
", @"
class C
{
    void M(bool f1 = false, bool f2 = false)
    {
        while (!f1)
        {
            M();
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task Test_Do_IfWithBraces_LastStatement()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool f = false;

        do
        {
            M();

            [|if (f)
            {
                break;
            }|]
        }
        while (true);
    }
}
", @"
class C
{
    void M()
    {
        bool f = false;

        do
        {
            M();
        }
        while (!f);
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task Test_Do_IfWithoutBraces_LastStatement()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(bool f1 = false)
    {
        do
        {
            M();

            [|if (f1)
                break;|]
        }
        while (true);
    }
}
", @"
class C
{
    void M(bool f1 = false)
    {
        do
        {
            M();
        }
        while (!f1);
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task Test_Do_IfWithBraces_FirstStatement()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(bool f1 = false, bool f2 = false)
    {
        do
        {
            [|if (f1)
            {
                break;
            }|]
            M();
        }
        while (true);
    }
}
", @"
class C
{
    void M(bool f1 = false, bool f2 = false)
    {
        while (!f1)
        {
            M();
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task Test_Do_IfWithoutBraces_FirstStatement()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M(bool f1 = false, bool f2 = false)
    {
        do
        {
            [|if (f1)
                break;|]
            M();
        }
        while (true);
    }
}
", @"
class C
{
    void M(bool f1 = false, bool f2 = false)
    {
        while (!f1)
        {
            M();
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task Test_IfThatContainsOnlyDo()
        {
            await VerifyDiagnosticAndFixAsync(@"
class C
{
    void M()
    {
        bool f = false;

        [|if (f)
        {
            do
            {
                M();
            }
            while (f);
        }|]
    }
}
", @"
class C
{
    void M()
    {
        bool f = false;

        while (f)
        {
            M();
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task TestNoDiagnostic()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M(bool f1 = false, bool f2 = false)
    {
        if (f1)
        {
        }
        else
        {
        }

        if (f1)
        {
        }
        else if (f2)
        {
        }

        if (f1)
        {
            M();
        }
        else
        {
            M();
        }

        if (f1)
        {
            M();
        }
        else if (f2)
        {
            M();
        }

        if (f1)
        {
        }
        else if (f2)
        {
            M();
        }
        else
        {
        }

        if ()
        {
        }
        else if (f2)
        {
            M();
        }

        if (f1)
        {
        }
        else if ()
        {
            M();
        }
    }
}
", options: Options.AddAllowedCompilerDiagnosticId("CS1525"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task TestNoDiagnostic_While()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M(bool f1 = false, bool f2 = false)
    {
        while (true)
        {
            if (f1)
            {
                break;
            }
        }

        while (f1)
        {
            if (f2)
            {
                break;
            }
        }

        while (true)
        {
            if (f1)
            {
                break;
            }
            else
            {
            }
        }

        while ()
        {
            if (f1)
            {
                break;
            }
        }

        while (true)
        {
            if ()
            {
                break;
            }
        }

        while (f1)
        {
            M();

            if (f2)
            {
                break;
            }
        }

        while (f1)
        {
            M();

            if (f2)
            {
                return;
            }
        }

        while ()
        {
            M();

            if (f1)
            {
                break;
            }
        }

        while (f1)
        {
            M();

            if ()
            {
                break;
            }
        }

        while (f1)
        {
            M();

            if (f2)
            {
                return;
            }
        }

        while ()
        {
            M();

            if (f1)
            {
                break;
            }
        }

        while (f1)
        {
            M();

            if ()
            {
                break;
            }
        }
    }
}
", options: Options.AddAllowedCompilerDiagnosticId("CS1525"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task TestNoDiagnostic_While_ConditionContainsLocalDefinedInLoopBody_LocalDeclaration()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        while (true)
        {
            bool f = false;

            if (f)
            {
                break;
            }
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task TestNoDiagnostic_While_ConditionContainsLocalDefinedInLoopBody_IsPatternExpression()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        while (true)
        {
            object x = null;
            if (!(x is bool f))
            {
                f = false;
            }


            if (f)
            {
                break;
            }
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task TestNoDiagnostic_While_ConditionContainsLocalDefinedInLoopBody_OutVariable()
        {
            await VerifyNoDiagnosticAsync(@"
using System;

class C
{
    void M()
    {
        while (true)
        {
            if (TryGet(out bool f))
            {
            }

            if (f)
            {
                break;
            }
        }
    }

    private bool TryGet(out bool f)
    {
        throw new NotImplementedException();
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task TestNoDiagnostic_While_ConditionContainsLocalDefinedInLoopBody_DeconstructionVariable()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        while (true)
        {
            (bool f, bool f2) = (false, false);

            if (f)
            {
                break;
            }
        }
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task TestNoDiagnostic_Do()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M(bool f1 = false, bool f2 = false)
    {
        do
        {
            M();

            if (f2)
            {
                break;
            }

        } while (f1);

        do
        {
            M();

            if (f2)
            {
                return;
            }
        }
        while (f1);

        do
        {
            M();

            if (f1)
            {
                break;
            }
        }
        while ();

        do
        {
            M();

            if ()
            {
                break;
            }
        }
        while (f1);

        do
        {
            M();

            if (f2)
            {
                break;
            }

        } while (f1);

        do
        {
            M();

            if (f2)
            {
                return;
            }

        } while (f1);

        do
        {
            M();

            if (f1)
            {
                break;
            }

        } while ();

        do
        {
            M();

            if ()
            {
                break;
            }

        } while (f1);
    }
}
", options: Options.AddAllowedCompilerDiagnosticId("CS1525"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task TestNoDiagnostic_Do_ConditionContainsLocalDefinedInLoopBody_LocalDeclaration()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        do
        {
            bool f = false;

            if (f)
            {
                break;
            }
        }
        while (true);
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task TestNoDiagnostic_Do_ConditionContainsLocalDefinedInLoopBody_IsPatternExpression()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        do
        {
            object x = null;
            if (!(x is bool f))
            {
                f = false;
            }


            if (f)
            {
                break;
            }
        }
        while (true);
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task TestNoDiagnostic_Do_ConditionContainsLocalDefinedInLoopBody_OutVariable()
        {
            await VerifyNoDiagnosticAsync(@"
using System;

class C
{
    void M()
    {
        do
        {
            if (TryGet(out bool f))
            {
            }

            if (f)
            {
                break;
            }
        }
        while (true);
    }

    private bool TryGet(out bool f)
    {
        throw new NotImplementedException();
    }
}
");
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task TestNoDiagnostic_Do_ConditionContainsLocalDefinedInLoopBody_DeconstructionVariable()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        do
        {
            (bool f, bool f2) = (false, false);

            if (f)
            {
                break;
            }
        }
        while (true);
    }
}
", options: Options.AddAllowedCompilerDiagnosticId("CS1525"));
        }

        [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.SimplifyCodeBranching)]
        public async Task TestNoDiagnostic_IfThatContainsOnlyDo_ConditionsAreNotEquivalent()
        {
            await VerifyNoDiagnosticAsync(@"
class C
{
    void M()
    {
        bool f = false;
        bool f2 = false;

        if (f)
        {
            do
            {
                M();
            }
            while (f2);
        }
    }
}
");
        }
    }
}
