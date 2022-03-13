// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0103RemoveMemberDeclarationTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.RemoveMemberDeclaration;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.RemoveMemberDeclaration)]
        public async Task Test_Class()
        {
            await VerifyRefactoringAsync(@"
namespace N
{
    class C
    [||]{
    }

    class C2
    {
    }
}
", @"
namespace N
{
    class C2
    {
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.RemoveMemberDeclaration)]
        public async Task Test_Class2()
        {
            await VerifyRefactoringAsync(@"
namespace N
{
    class C
    {
    }

    class C2
    {
    [||]}
}
", @"
namespace N
{
    class C
    {
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.RemoveMemberDeclaration)]
        public async Task Test_Class_FileScopedNamespace()
        {
            await VerifyRefactoringAsync(@"
namespace N;

class C
[||]{
}

class C2
{
}
", @"
namespace N;

class C2
{
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }
    }
}
