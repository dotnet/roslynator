// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0139ReplaceNullLiteralWithDefaultExpressionTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.ReplaceNullLiteralWithDefaultExpression;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ReplaceNullLiteralWithDefaultExpression)]
        public async Task Test_Argument()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M(C p)
    {
        M([||]null);
    }
}
", @"
class C
{
    void M(C p)
    {
        M(default(C));
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ReplaceNullLiteralWithDefaultExpression)]
        public async Task Test_ReturnExpression()
        {
            await VerifyRefactoringAsync(@"
class C
{
    object M()
    {
        return [||]null;
    }
}
", @"
class C
{
    object M()
    {
        return default(object);
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ReplaceNullLiteralWithDefaultExpression)]
        public async Task Test_LocalDeclaration()
        {
            await VerifyRefactoringAsync(@"
#nullable enable

class C
{
    void M()
    {
        string? s = [||]null;
    }
}
", @"
#nullable enable

class C
{
    void M()
    {
        string? s = default(string);
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ReplaceNullLiteralWithDefaultExpression)]
        public async Task TestNoRefactoring_ParameterDefaultValue()
        {
            await VerifyNoRefactoringAsync(@"
class C
{
    void M(C p = [||]null)
    {
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }
    }
}
