// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0014AddUsingDirectiveTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.AddUsingDirective;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddUsingDirective)]
        public async Task Test()
        {
            await VerifyRefactoringAsync(@"
using A.B;

namespace N
{
    class C
    {
        void M()
        {
            A.B.[||]C.D d = null;
        }
    }
}

namespace A.B.C
{
    class D
    {
    }
}
", @"
using A.B;
using A.B.C;

namespace N
{
    class C
    {
        void M()
        {
            D d = null;
        }
    }
}

namespace A.B.C
{
    class D
    {
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddUsingDirective)]
        public async Task Test_FileScopedNamespace()
        {
            await VerifyRefactoringAsync(@"
using A.B;

namespace N;

class C
{
    void M()
    {
        A.B[||].C.D d = null;
    }
}
", @"
using A.B;
using A.B.C;

namespace N;

class C
{
    void M()
    {
        D d = null;
    }
}
", additionalFiles: new[] { @"
namespace A.B.C
{
    class D
    {
    }
}
" }, equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddUsingDirective)]
        public async Task TestNoRefactoring_InsideFileScopedNamespace()
        {
            await VerifyNoRefactoringAsync(@"
namespace A.[||]B;
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddUsingDirective)]
        public async Task TestNoRefactoring_InsideUsingDirective()
        {
            await VerifyNoRefactoringAsync(@"
using A.[||]B;

namespace A.B
{
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddUsingDirective)]
        public async Task TestNoRefactoring_UsingInScope()
        {
            await VerifyNoRefactoringAsync(@"
using A.B;

namespace N
{
    class C
    {
        void M()
        {
            A.[||]B.C c = null;
        }
    }
}

namespace A.B
{
    class C
    {
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }
    }
}
