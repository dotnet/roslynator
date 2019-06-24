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

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.InlineMethod + ".Remove")]
        public async Task TestInlineAndRemove()
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

        object B => null;
    }

    static class B
    {
    }
}", equivalenceKey: EquivalenceKey.Join(RefactoringId, "Remove"));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.InlineMethod + ".Remove")]
        public async Task TestInlineAllAndRemove()
        {
            await VerifyRefactoringAsync(@"
namespace N
{
    class C
    {
        void M()
        {
            [||]M3();
        }

        void M2()
        {
            M3();
        }

        void M3()
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
}", equivalenceKey: EquivalenceKey.Join(RefactoringId, "Remove"));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.InlineMethod + ".Remove")]
        public async Task TestInlineAllAndWithoutRemove()
        {
            await VerifyRefactoringAsync(@"
namespace N
{
    class C
    {
        void M()
        {
            [||]M4();
        }

        void M2()
        {
            M4();
        }

        void M3()
        {
            System.Action a = this.M4;
        }

        void M4()
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

        void M3()
        {
            System.Action a = this.M4;
        }

        void M4()
        {
            var x = typeof(N.B);
        }

        object B => null;
    }

    static class B
    {
    }
}", equivalenceKey: EquivalenceKey.Join(RefactoringId, "Remove"));
        }
    }
}
