// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0217DeconstructForeachVariableTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.DeconstructForeachVariable;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.DeconstructForeachVariable)]
        public async Task Test_EmptyObjectInitializer()
        {
            await VerifyRefactoringAsync(@"
using System.Collections.Generic;

class C
{
    void M()
    {
        var dic = new Dictionary<object, object>();

        foreach ([||]var kvp in dic)
        {
            var k = kvp.Key;
            var v = kvp.Value.ToString();
        }
    }
}
", @"
using System.Collections.Generic;

class C
{
    void M()
    {
        var dic = new Dictionary<object, object>();

        foreach (var (key, value) in dic)
        {
            var k = key;
            var v = value.ToString();
        }
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }
    }
}
