// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0124ReplaceEqualityOperatorWithStringEqualsTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.ReplaceEqualityOperatorWithStringEquals;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ReplaceEqualityOperatorWithStringEquals)]
        public async Task Test()
        {
            await VerifyRefactoringAsync(@"
using System;

class C
{
    void M()
    {
        string s1 = null;
        string s2 = null;

        if (s1 =[||]= s2) { }
    }
}
", @"
using System;

class C
{
    void M()
    {
        string s1 = null;
        string s2 = null;

        if (string.Equals(s1, s2, StringComparison.CurrentCulture)) { }
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }
    }
}
