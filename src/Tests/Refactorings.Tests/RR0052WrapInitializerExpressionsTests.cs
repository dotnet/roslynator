// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0052WrapInitializerExpressionsTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.WrapInitializerExpressions;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.WrapInitializerExpressions)]
        public async Task Test_ToMultiLine()
        {
            await VerifyRefactoringAsync(@"
class C
{
    string P1 { get; set; }
    string P2 { get; set; }

    void M()
    {
        var x = new C() {[||] P1 = null, P2 = null };
    }
}
", @"
class C
{
    string P1 { get; set; }
    string P2 { get; set; }

    void M()
    {
        var x = new C()
        {
            P1 = null,
            P2 = null
        };
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.WrapInitializerExpressions)]
        public async Task Test_WithExpressionToMultiLine()
        {
            await VerifyRefactoringAsync(@"
record C
{
    string P1 { get; set; }
    string P2 { get; set; }

    void M()
    {
        var x = this with {[||] P1 = null, P2 = null };
    }
}

namespace System.Runtime.CompilerServices { internal static class IsExternalInit {} }
", @"
record C
{
    string P1 { get; set; }
    string P2 { get; set; }

    void M()
    {
        var x = this with
        {
            P1 = null,
            P2 = null
        };
    }
}

namespace System.Runtime.CompilerServices { internal static class IsExternalInit {} }
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.WrapInitializerExpressions)]
        public async Task Test_ToSingleLine()
        {
            await VerifyRefactoringAsync(@"
class C
{
    string P1 { get; set; }
    string P2 { get; set; }

    void M()
    {
        var x = new C()
        {
            [||]P1 = null,
            P2 = null
        };
    }
}
", @"
class C
{
    string P1 { get; set; }
    string P2 { get; set; }

    void M()
    {
        var x = new C() { P1 = null, P2 = null };
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.WrapInitializerExpressions)]
        public async Task Test_WithExpressionToSingleLine()
        {
            await VerifyRefactoringAsync(@"
record C
{
    string P1 { get; set; }
    string P2 { get; set; }

    void M()
    {
        var x = this with
        {
            [||]P1 = null,
            P2 = null
        };
    }
}

namespace System.Runtime.CompilerServices { internal static class IsExternalInit {} }
", @"
record C
{
    string P1 { get; set; }
    string P2 { get; set; }

    void M()
    {
        var x = this with { P1 = null, P2 = null };
    }
}

namespace System.Runtime.CompilerServices { internal static class IsExternalInit {} }
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.WrapInitializerExpressions)]
        public async Task TestNoRefactoring_ToSingleLine_ContainsSingleLineComment()
        {
            await VerifyNoRefactoringAsync(@"
class C
{
    string P1 { get; set; }
    string P2 { get; set; }

    void M()
    {
        var x = new C()
        {
            [||]P1 = null, //x
            P2 = null
        };
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }
    }
}
