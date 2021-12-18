// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0143ConvertReturnStatementToIfTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.ConvertReturnStatementToIf;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ConvertReturnStatementToIf)]
        public async Task Test_ReturnStatement()
        {
            await VerifyRefactoringAsync(@"
class C
{
    bool M(bool f)
    {
        [||]return f;
    }
}
", @"
class C
{
    bool M(bool f)
    {
        if (f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ConvertReturnStatementToIf)]
        public async Task Test_ReturnStatement_SelectEntireStatement()
        {
            await VerifyRefactoringAsync(@"
class C
{
    bool M(bool f)
    {
        [|return f;|]
    }
}
", @"
class C
{
    bool M(bool f)
    {
        if (f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ConvertReturnStatementToIf)]
        public async Task Test_YieldReturnStatement()
        {
            await VerifyRefactoringAsync(@"
using System.Collections.Generic;

class C
{
    IEnumerable<bool> M(bool f)
    {
        [||]yield return f;
    }
}
", @"
using System.Collections.Generic;

class C
{
    IEnumerable<bool> M(bool f)
    {
        if (f)
        {
            yield return true;
        }
        else
        {
            yield return false;
        }
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ConvertReturnStatementToIf)]
        public async Task Test_YieldReturnStatement_SelectEntireStatement()
        {
            await VerifyRefactoringAsync(@"
using System.Collections.Generic;

class C
{
    IEnumerable<bool> M(bool f)
    {
        [|yield return f;|]
    }
}
", @"
using System.Collections.Generic;

class C
{
    IEnumerable<bool> M(bool f)
    {
        if (f)
        {
            yield return true;
        }
        else
        {
            yield return false;
        }
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ConvertReturnStatementToIf)]
        public async Task TestNoRefactoring_NotBooleanExpression()
        {
            await VerifyNoRefactoringAsync(@"
class C
{
    string M(string s)
    {
        [||]return s;
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ConvertReturnStatementToIf)]
        public async Task TestNoRefactoring_TrueLiteralExpression()
        {
            await VerifyNoRefactoringAsync(@"
class C
{
    bool M()
    {
        [||]return true;
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ConvertReturnStatementToIf)]
        public async Task TestNoRefactoring_FalseLiteralExpression()
        {
            await VerifyNoRefactoringAsync(@"
class C
{
    bool M()
    {
        [||]return false;
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }
    }
}
