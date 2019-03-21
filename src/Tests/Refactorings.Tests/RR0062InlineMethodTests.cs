// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0062InlineMethodTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.InlineMethod;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.InlineMethod)]
        public async Task Test()
        {
            await VerifyRefactoringAsync(@"
namespace N
{
    class C
    {
        void M()
        {
            [||]M2();
        }

        void M2()
        {
            var x = typeof(N.B);
        }

        object B => null;
    }

    static class B
    {
    }
}", @"
namespace N
{
    class C
    {
        void M()
        {
            var x = typeof(N.B);
        }

        void M2()
        {
            var x = typeof(N.B);
        }

        object B => null;
    }

    static class B
    {
    }
}", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.InlineMethod)]
        public async Task TestNoRefactoring()
        {
            await VerifyNoRefactoringAsync(@"
class C
{
    void M()
    {
        var c = this;

        C a = c?.[||]A();
    }

    C A()
    {
        return B;
    }

    C B
    {
        get { return null; }
    }
}
", equivalenceKey: RefactoringId);
        }
    }
}
