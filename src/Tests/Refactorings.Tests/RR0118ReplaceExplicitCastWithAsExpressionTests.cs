// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0118ReplaceExplicitCastWithAsExpressionTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.ReplaceExplicitCastWithAsExpression;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ReplaceExplicitCastWithAsExpression)]
        public async Task Test()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M()
    {
        object x = null;

        var y = (C)[||]x;
    }
}
", @"
class C
{
    void M()
    {
        object x = null;

        var y = x as C;
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ReplaceExplicitCastWithAsExpression)]
        public async Task TestNoRefactoring_ValueType()
        {
            await VerifyNoRefactoringAsync(@"
class C
{
    void M()
    {
        object i = 0;
        int j = (int)[||]i;
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }
    }
}
