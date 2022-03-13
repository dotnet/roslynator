// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0194SplitLocalDeclarationAndAssignmentTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.SplitLocalDeclarationAndAssignment;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.SplitLocalDeclarationAndAssignment)]
        public async Task TestRefactoring()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M()
    {
        string s [||]= new string(' ', 1);
    }
}
", @"
class C
{
    void M()
    {
        string s;
        s = new string(' ', 1);
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }
    }
}
