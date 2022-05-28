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
        public async Task Test_Dictionary()
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

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.DeconstructForeachVariable)]
        public async Task Test_Dictionary_TopLevelStatement()
        {
            await VerifyRefactoringAsync(@"
using System.Collections.Generic;

var dic = new Dictionary<object, object>();

foreach ([||]var kvp in dic)
{
    var k = kvp.Key;
    var v = kvp.Value.ToString();
}
", @"
using System.Collections.Generic;

var dic = new Dictionary<object, object>();

foreach (var (key, value) in dic)
{
    var k = key;
    var v = value.ToString();
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.DeconstructForeachVariable)]
        public async Task Test_Tuple()
        {
            await VerifyRefactoringAsync(@"
using System.Collections.Generic;

class C
{
    void M()
    {
        var items = new List<(object, string)>();

        foreach ([||]var item in items)
        {
            var k = item.Item1;
            var v = item.Item2.ToString();
        }
    }
}
", @"
using System.Collections.Generic;

class C
{
    void M()
    {
        var items = new List<(object, string)>();

        foreach (var (item1, item2) in items)
        {
            var k = item1;
            var v = item2.ToString();
        }
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.DeconstructForeachVariable)]
        public async Task Test_TupleWithNamedFields()
        {
            await VerifyRefactoringAsync(@"
using System.Collections.Generic;

class C
{
    void M()
    {
        var p1 = false;
        var items = new List<(object p1, string p2)>();

        foreach ([||]var item in items)
        {
            var k = item.p1;
            var v = item.p2.ToString();
        }
    }
}
", @"
using System.Collections.Generic;

class C
{
    void M()
    {
        var p1 = false;
        var items = new List<(object p1, string p2)>();

        foreach (var (p12, p2) in items)
        {
            var k = p12;
            var v = p2.ToString();
        }
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }
    }
}
