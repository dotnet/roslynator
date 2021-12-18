// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0021ChangeMethodReturnTypeToVoidTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.ChangeMethodReturnTypeToVoid;

        public override CSharpTestOptions Options
        {
            get { return base.Options.AddAllowedCompilerDiagnosticId(CompilerDiagnosticIdentifiers.CS0161_NotAllCodePathsReturnValue); }
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ChangeMethodReturnTypeToVoid)]
        public async Task Test_Method()
        {
            await VerifyRefactoringAsync(@"
class C
{
    string M()
    {[||]
        M();
    }
}
", @"
class C
{
    void M()
    {
        M();
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ChangeMethodReturnTypeToVoid)]
        public async Task Test_Method_IEnumerableOfT()
        {
            await VerifyRefactoringAsync(@"
class C
{
    System.Collections.Generic.IEnumerable<string> M()
    {[||]
        M();
    }
}
", @"
class C
{
    void M()
    {
        M();
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ChangeMethodReturnTypeToVoid)]
        public async Task Test_LocalFunction()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M()
    {
        string LF()
        {[||]
            LF();
        }
    }
}
", @"
class C
{
    void M()
    {
        void LF()
        {
            LF();
        }
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ChangeMethodReturnTypeToVoid)]
        public async Task TestNoRefactoring_MethodWithReturn()
        {
            await VerifyNoRefactoringAsync(@"
class C
{
    string M()
    {[||]
        return M();
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ChangeMethodReturnTypeToVoid)]
        public async Task TestNoRefactoring_VoidMethod()
        {
            await VerifyNoRefactoringAsync(@"
class C
{
    void M()
    {[||]
        M();
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ChangeMethodReturnTypeToVoid)]
        public async Task TestNoRefactoring_Iterator()
        {
            await VerifyNoRefactoringAsync(@"
using System.Collections.Generic;

class C
{
    IEnumerable<string> M()
    {[||]
        yield return null;
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ChangeMethodReturnTypeToVoid)]
        public async Task TestNoRefactoring_Iterator_YieldBreak()
        {
            await VerifyNoRefactoringAsync(@"
using System.Collections.Generic;

class C
{
    IEnumerable<string> M()
    {[||]
        yield break;
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ChangeMethodReturnTypeToVoid)]
        public async Task TestNoRefactoring_ReturnsTask()
        {
            await VerifyNoRefactoringAsync(@"
using System.Threading.Tasks;

class C
{
    async Task M()
    {[||]
        string x = await Task.FromResult(default(string));
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ChangeMethodReturnTypeToVoid)]
        public async Task TestNoRefactoring_Throws()
        {
            await VerifyNoRefactoringAsync(@"
using System;

class C
{
    string M()
    {[||]
        throw new InvalidOperationException();
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ChangeMethodReturnTypeToVoid)]
        public async Task TestNoRefactoring_InterfaceImplementation()
        {
            await VerifyNoRefactoringAsync(@"
interface IFoo
{
    string M();
}

class C : IFoo
{
    public string M()
    {[||]
        M();
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ChangeMethodReturnTypeToVoid)]
        public async Task TestNoRefactoring_Override()
        {
            await VerifyNoRefactoringAsync(@"
class B
{
    public virtual string M() => null;
}

class C : B
{
    public override string M()
    {[||]
        object x = null;
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }
    }
}
