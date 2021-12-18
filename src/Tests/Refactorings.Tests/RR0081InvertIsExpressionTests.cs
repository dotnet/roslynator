// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0081InvertIsExpressionTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.InvertIsExpression;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.NotifyWhenPropertyChanges)]
        public async Task Test_IsExpression()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M()
    {
        var x = new object();

        if (x [||]is string)
        {
        }
    }
}
", @"
class C
{
    void M()
    {
        var x = new object();

        if (x is not string)
        {
        }
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.NotifyWhenPropertyChanges)]
        public async Task Test_IsNotExpression()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M()
    {
        var x = new object();

        if (x [||]is not string)
        {
        }
    }
}
", @"
class C
{
    void M()
    {
        var x = new object();

        if (x is string)
        {
        }
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }
    }
}
