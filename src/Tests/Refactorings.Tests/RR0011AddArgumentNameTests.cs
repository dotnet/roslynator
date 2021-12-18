// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0011AddArgumentNameTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.AddArgumentName;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddArgumentName)]
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
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }
    }
}
