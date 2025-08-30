using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests;

public class RR0024CheckExpressionForNullTests : AbstractCSharpRefactoringVerifier
{
    public override string RefactoringId { get; } = RefactoringIdentifiers.CheckExpressionForNull;

    [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.CheckParameterForNull)]
    public async Task Test()
    {
        await VerifyRefactoringAsync("""
class C
{
    void M()
    {
        var [|x|] = "".ToString();
    }
}
""", """
class C
{
    void M()
    {
        var x = "".ToString();
        if (x != null)
        {

        }
    }
}
""", equivalenceKey: EquivalenceKey.Create(RefactoringId));
    }

    [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.CheckParameterForNull)]
    public async Task Test_TopLevelStatement()
    {
        await VerifyRefactoringAsync("""

var [|x|] = "".ToString();
var y = x.ToString();

""", """

var x = "".ToString();

if (x != null)
{

}
var y = x.ToString();

""", equivalenceKey: EquivalenceKey.Create(RefactoringId));
    }
}