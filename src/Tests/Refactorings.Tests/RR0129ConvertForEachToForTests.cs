using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests;

public class RR0129ConvertForEachToForTests : AbstractCSharpRefactoringVerifier
{
    public override string RefactoringId { get; } = RefactoringIdentifiers.ConvertForEachToFor;

    [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ConvertForEachToFor)]
    public async Task Test()
    {
        await VerifyRefactoringAsync(@"
using System.Collections.Generic;

class C
{
    void M()
    {
        List<string> items = new List<string>();

        f[||]oreach (string item in items)
        {
            var x = item;
        }
    }
}
", @"
using System.Collections.Generic;

class C
{
    void M()
    {
        List<string> items = new List<string>();

        for (int i = 0; i < items.Count; i++)
        {
            var x = items[i];
        }
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
    }

    [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ConvertForEachToFor)]
    public async Task TestNoRefactoring_OrderedDictionary()
    {
        await VerifyNoRefactoringAsync(@"
using System.Collections.Generic;

class C
{
    void M()
    {
        OrderedDictionary<int, string> dic = new OrderedDictionary<int, string>();

        f[||]oreach (KeyValuePair<int, string> item in dic)
        {
            KeyValuePair<int, string> x = item;
        }
     }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
    }
}