// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0011AddParameterNameToArgumentTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.AddParameterNameToArgument;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddParameterNameToArgument)]
        public async Task Test_MultilineArgumentListInArrayInitializer()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M()
    {
        var arr = new[]
        {
            new string(
[|                ' ',
                1|])
        };
    }
}
", @"
class C
{
    void M()
    {
        var arr = new[]
        {
            new string(
                c: ' ',
                count: 1)
        };
    }
}
", equivalenceKey: RefactoringId);
        }
    }
}
