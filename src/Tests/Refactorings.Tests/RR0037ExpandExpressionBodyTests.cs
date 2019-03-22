// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0037ExpandExpressionBodyTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.ExpandExpressionBody;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.UseExpressionBodiedMember)]
        public async Task Test_MultipleMembers()
        {
            await VerifyRefactoringAsync(@"
class C
{
[|    public C() => M();

    ~C() => M();

    string M() => default;

    public string P => default;

    public string this[int index] => default;

    public static explicit operator C(string value) => default;

    public static explicit operator string(C value) => default;

    public static C operator !(C value) => default;|]
}", @"
class C
{
    public C()
    {
        M();
    }

    ~C()
    {
        M();
    }

    string M()
    {
        return default;
    }

    public string P
    {
        get { return default; }
    }

    public string this[int index]
    {
        get { return default; }
    }

    public static explicit operator C(string value)
    {
        return default;
    }

    public static explicit operator string(C value)
    {
        return default;
    }

    public static C operator !(C value)
    {
        return default;
    }
}", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ExpandExpressionBody)]
        public async Task TestNoRefactoring_MultipleMembers()
        {
            await VerifyNoRefactoringAsync(@"
class C
{
[|    string M()
    {
        return default;
    }

    string M2() => default;|]
}
", equivalenceKey: RefactoringId);
        }
    }
}
