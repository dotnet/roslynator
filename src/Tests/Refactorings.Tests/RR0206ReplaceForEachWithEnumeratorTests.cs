// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0206ReplaceForEachWithEnumeratorTests : AbstractCSharpCodeRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.ReplaceForEachWithEnumerator;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ReplaceForEachWithEnumerator)]
        public async Task TestCodeRefactoring_WithUsing()
        {
            await VerifyRefactoringAsync(
@"
using System.Linq;

class C
{
    void M()
    {
        [||]foreach (int item in Enumerable.Range(0, 1))
        {
            int x = item;
        }
    }

    int M(int value)
    {
        return value;
    }
}
",
@"
using System.Linq;

class C
{
    void M()
    {
        using (var en = Enumerable.Range(0, 1).GetEnumerator())
        {
            while (en.MoveNext())
            {
                int x = en.Current;
            }
        }
    }

    int M(int value)
    {
        return value;
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ReplaceForEachWithEnumerator)]
        public async Task TestCodeRefactoring_WithUsing_EmbeddedStatement()
        {
            await VerifyRefactoringAsync(
@"
using System.Linq;

class C
{
    void M()
    {
        for[||]each (int item in Enumerable.Range(0, 1))
            M(item);
    }

    int M(int value)
    {
        return value;
    }
}
",
@"
using System.Linq;

class C
{
    void M()
    {
        using (var en = Enumerable.Range(0, 1).GetEnumerator())
        {
            while (en.MoveNext())
                M(en.Current);
        }
    }

    int M(int value)
    {
        return value;
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ReplaceForEachWithEnumerator)]
        public async Task TestCodeRefactoring_WithoutUsing()
        {
            await VerifyRefactoringAsync(
@"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SyntaxList<SyntaxNode> nodes;

        [||]foreach (SyntaxNode node in nodes)
        {
            SyntaxNode x = node;
        }
    }
}
",
@"
using Microsoft.CodeAnalysis;

class C
{
    void M()
    {
        SyntaxList<SyntaxNode> nodes;

        var en = nodes.GetEnumerator();
        while (en.MoveNext())
        {
            SyntaxNode x = en.Current;
        }
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ReplaceForEachWithEnumerator)]
        public async Task TestNoCodeRefactoring_InvalidSpan()
        {
            await VerifyNoRefactoringAsync(
@"
using System.Linq;

class C
{
    void M()
    {
        [|foreach|] (int item in Enumerable.Range(0, 1))
        {
            int x = item;
        }

        [|foreach (int item in Enumerable.Range(0, 1))
        {
            int x = item;
        }|]

        [|f|]oreach (int item in Enumerable.Range(0, 1))
        {
            int x = item;
        }
        foreach (int item in Enumerable.Range(0, 1))
        {
            [||]int x = item;
        }
    }
}
", RefactoringId);
        }
    }
}
