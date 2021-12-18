// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0178AddGenericParameterToDeclarationTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.AddGenericParameterToDeclaration;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddGenericParameterToDeclaration)]
        public async Task Test()
        {
            await VerifyRefactoringAsync(@"
class C[||]
{
}
", @"
class C<T>
{
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddGenericParameterToDeclaration)]
        public async Task TestNoRefactoring_SpanOnNextLine()
        {
            await VerifyNoRefactoringAsync(@"
class C
[||]{
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }
    }
}
