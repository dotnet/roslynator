// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests;

public class RR0100RemovePreprocessorDirectiveTests : AbstractCSharpRefactoringVerifier
{
    public override string RefactoringId { get; } = RefactoringIdentifiers.RemovePreprocessorDirective;

    [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.RemovePreprocessorDirective)]
    public async Task RemovePreprocessorDirectiveIncludingContent_If()
    {
        await VerifyRefactoringAsync(@"
class C
{
#if [||]DEBUG // if
    void M()
    {
    }
#endif // endif
}
", @"
class C
{
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId) + ".IncludingContent");
    }

    [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.RemovePreprocessorDirective)]
    public async Task RemovePreprocessorDirectiveIncludingContent_EndIf()
    {
        await VerifyRefactoringAsync(@"
class C
{
#if DEBUG // if
    void M()
    {
    }
#[||]endif // endif
}
", @"
class C
{
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId) + ".IncludingContent");
    }

    [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.RemovePreprocessorDirective)]
    public async Task RemovePreprocessorDirectiveIncludingContent_Else()
    {
        await VerifyRefactoringAsync(@"
class C
{
#if DEBUG // if
    void M()
    {
    }
#[||]else
    void M2()
    {
    }
#endif // endif
}
", @"
class C
{
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId) + ".IncludingContent");
    }

    [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.RemovePreprocessorDirective)]
    public async Task RemovePreprocessorDirectiveIncludingContent_Elif()
    {
        await VerifyRefactoringAsync(@"
class C
{
#if DEBUG // if
    void M()
    {
    }
#[||]elif FOO
    void M2()
    {
    }
#else
    void M3()
    {
    }
#endif // endif
}
", @"
class C
{
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId) + ".IncludingContent");
    }

    [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.RemovePreprocessorDirective)]
    public async Task RemovePreprocessorDirectiveIncludingContent_Region()
    {
        await VerifyRefactoringAsync(@"
class C
{
#[||]region // region
    void M()
    {
    }
#endregion // endregion
}
", @"
class C
{
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId) + ".IncludingContent");
    }

    [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.RemovePreprocessorDirective)]
    public async Task RemovePreprocessorDirectiveIncludingContent_EndRegion()
    {
        await VerifyRefactoringAsync(@"
class C
{
#region // region
    void M()
    {
    }
#[||]endregion // endregion
}
", @"
class C
{
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId) + ".IncludingContent");
    }
}
