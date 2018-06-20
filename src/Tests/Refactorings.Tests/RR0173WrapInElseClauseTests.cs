// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0173WrapInElseClauseTests : AbstractCSharpCodeRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.WrapInElseClause;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.WrapInElseClause)]
        public async Task Test_IfWithBlock()
        {
            await VerifyRefactoringAsync(@"
class C
{
    public string M(bool f)
    {
        if (f)
        {
            return null;
        }

[|        return null;|]
    }
}
", @"
class C
{
    public string M(bool f)
    {
        if (f)
        {
            return null;
        }
        else
        {
            return null;
        }
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.WrapInElseClause)]
        public async Task Test_If_WithEmbeddedStatement()
        {
            await VerifyRefactoringAsync(@"
class C
{
    public string M(bool f)
    {
        if (f)
            return null;

[|        return null;|]
    }
}
", @"
class C
{
    public string M(bool f)
    {
        if (f)
            return null;
        else
            return null;
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.WrapInElseClause)]
        public async Task Test_IfWithBlock_MultipleStatements()
        {
            await VerifyRefactoringAsync(@"
class C
{
    public string M(bool f)
    {
        if (f)
        {
            M(f);
            return null;
        }

[|        M(f);
        return null;|]
    }
}
", @"
class C
{
    public string M(bool f)
    {
        if (f)
        {
            M(f);
            return null;
        }
        else
        {
            M(f);
            return null;
        }
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.WrapInElseClause)]
        public async Task Test_IfElseIf()
        {
            await VerifyRefactoringAsync(@"
using System;

class C
{
    public string M(bool f)
    {
        var items = new string[0];

        foreach (string item in items)
        {
            if (f)
            {
                break;
            }
            else if (f)
            {
                continue;
            }
            else if (f)
            {
                return null;
            }
            else if (f)
            {
                throw new InvalidOperationException();
            }

[|            return null;|]
        }

        return null;
    }
}
", @"
using System;

class C
{
    public string M(bool f)
    {
        var items = new string[0];

        foreach (string item in items)
        {
            if (f)
            {
                break;
            }
            else if (f)
            {
                continue;
            }
            else if (f)
            {
                return null;
            }
            else if (f)
            {
                throw new InvalidOperationException();
            }
            else
            {
                return null;
            }
        }

        return null;
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.WrapInElseClause)]
        public async Task TestNoRefactoring_IfWithoutJumpStatement()
        {
            await VerifyNoRefactoringAsync(@"
class C
{
    public string M(bool f)
    {
        if (f)
        {
            M(f);
        }

[|        return null;|]
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.WrapInElseClause)]
        public async Task TestNoRefactoring_IfElse()
        {
            await VerifyNoRefactoringAsync(@"
class C
{
    public string M2(bool f)
    {
        if (f)
        {
            return null;
        }
        else
        {
        }

[|        return null;|]
    }
}
", equivalenceKey: RefactoringId);
        }
    }
}
