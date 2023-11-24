// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests;

public class RR0098RemoveCommentTests : AbstractCSharpRefactoringVerifier
{
    public override string RefactoringId { get; } = RefactoringIdentifiers.RemoveComment;

    [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.RemoveComment)]
    public async Task Test()
    {
        await VerifyRefactoringAsync(@"
#pragma warning disable RCS1018 // [||]Add accessibility modifiers.

class C
{
    void M()
    {
    }
}
", @"
#pragma warning disable RCS1018

class C
{
    void M()
    {
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
    }

    [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.RemoveComment)]
    public async Task Test_CommentBetweenDeclaration()
    {
        await VerifyRefactoringAsync(@"
class C
{
    void M1()
    {
    }

    //[||]void M2()
    //{
    //}

    void M3() { }
}
", @"
class C
{
    void M1()
    {
    }

    void M3() { }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
    }

    [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.RemoveComment)]
    public async Task Test_CommentBetweenDeclaration2()
    {
        await VerifyRefactoringAsync(@"
class C
{
    void M1()
    {
    }

    //[||]void M2()
    //{
    //}
    void M3() { }
}
", @"
class C
{
    void M1()
    {
    }

    void M3() { }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
    }

    [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.RemoveComment)]
    public async Task Test_BlankLineBetweenComments()
    {
        await VerifyRefactoringAsync(@"
class C
{
    void M1()
    {
    }

    //void M2()

    //[||]{

    //}

    void M3() { }
}
", @"
class C
{
    void M1()
    {
    }

    void M3() { }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
    }
}
