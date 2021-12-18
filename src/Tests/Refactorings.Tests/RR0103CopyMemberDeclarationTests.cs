// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0103CopyMemberDeclarationTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.CopyMemberDeclaration;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.CopyMemberDeclaration)]
        public async Task Test_CopyMemberBefore()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M()
    {
    }

    void N()
    [||]{
    }

    void O()
    {
    }
}
", @"
class C
{
    void M()
    {
    }

    void N2()
    {
    }

    void N()
    {
    }

    void O()
    {
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.CopyMemberDeclaration)]
        public async Task Test_CopyMemberAfter()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M()
    {
    }

    void N()
    {
    [||]}

    void O()
    {
    }
}
", @"
class C
{
    void M()
    {
    }

    void N()
    {
    }

    void N2()
    {
    }

    void O()
    {
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.CopyMemberDeclaration)]
        public async Task Test_CopyFirstMemberBefore()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M()
    [||]{
    }

    void N()
    {
    }
}
", @"
class C
{
    void M2()
    {
    }

    void M()
    {
    }

    void N()
    {
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.CopyMemberDeclaration)]
        public async Task Test_CopyFirstMemberAfter()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M()
    {
    [||]}

    void N()
    {
    }
}
", @"
class C
{
    void M()
    {
    }

    void M2()
    {
    }

    void N()
    {
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.CopyMemberDeclaration)]
        public async Task Test_CopyLastMemberBefore()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M()
    {
    }

    void N()
    [||]{
    }
}
", @"
class C
{
    void M()
    {
    }

    void N2()
    {
    }

    void N()
    {
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.CopyMemberDeclaration)]
        public async Task Test_CopyLastMemberAfter()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M()
    {
    }

    void N()
    {
    [||]}
}
", @"
class C
{
    void M()
    {
    }

    void N()
    {
    }

    void N2()
    {
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.CopyMemberDeclaration)]
        public async Task Test_ClassInsideFileScopedNamespace()
        {
            await VerifyRefactoringAsync(@"
namespace N;

class C
{
[||]}
", @"
namespace N;

class C
{
}

class C2
{
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.CopyMemberDeclaration)]
        public async Task Test_Class()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M()
    {
        C c = default(C);
    }
[||]}

class C2
{
    void M()
    {
        C c = default(C);
    }
}
", @"
class C
{
    void M()
    {
        C c = default(C);
    }
}

class C3
{
    void M()
    {
        C3 c = default(C3);
    }
}

class C2
{
    void M()
    {
        C c = default(C);
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.CopyMemberDeclaration)]
        public async Task Test_Constructor()
        {
            await VerifyRefactoringAsync(@"
class C
{
    public C()
    {
    [||]}
}
", @"
class C
{
    public C()
    {
    }

    public C()
    {
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId), options: Options.AddAllowedCompilerDiagnosticId("CS0111"));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.CopyMemberDeclaration)]
        public async Task Test_Indexer()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M()
    {
    }

    public string this[int index]
    [||]{
        get { return null; }
        set { }
    }
}
", @"
class C
{
    void M()
    {
    }

    public string this[int index]
    {
        get { return null; }
        set { }
    }

    public string this[int index]
    {
        get { return null; }
        set { }
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId), options: Options.AddAllowedCompilerDiagnosticId("CS0111"));
        }
    }
}
