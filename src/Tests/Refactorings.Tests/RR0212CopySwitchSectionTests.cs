// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0212CopySwitchSectionTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.CopySwitchSection;

        public override CSharpTestOptions Options
        {
            get { return base.Options.AddAllowedCompilerDiagnosticId("CS0152"); }
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.CopySwitchSection)]
        public async Task Test_OnCloseBrace()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M()
    {
        string s = null;

        switch (s)
        {
            case ""a"":
                {
                    M();
                    break;
                }[||]
            default:
                break;
        }
    }
}
", @"
class C
{
    void M()
    {
        string s = null;

        switch (s)
        {
            case ""a"":
                {
                    M();
                    break;
                }
            case ""a"":
                {
                    M();
                    break;
                }
            default:
                break;
        }
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.CopySwitchSection)]
        public async Task Test_OnEmptyLine()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M()
    {
        string s = null;

        switch (s)
        {
            case ""a"":
                M();
                break;
[||]
            default:
                break;
        }
    }
}
", @"
class C
{
    void M()
    {
        string s = null;

        switch (s)
        {
            case ""a"":
                M();
                break;

            case ""a"":
                M();
                break;

            default:
                break;
        }
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.CopySwitchSection)]
        public async Task Test_OnWhitespaceLine()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M()
    {
        string s = null;

        switch (s)
        {
            case ""a"":
                M();
                break;
[||]    
            default:
                break;
        }
    }
}
", @"
class C
{
    void M()
    {
        string s = null;

        switch (s)
        {
            case ""a"":
                M();
                break;

            case ""a"":
                M();
                break;

            default:
                break;
        }
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.CopySwitchSection)]
        public async Task Test_OnEmptyLineAfterLastSection()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M()
    {
        string s = null;

        switch (s)
        {
            case ""a"":
                M();
                break;
[||]
        }
    }
}
", @"
class C
{
    void M()
    {
        string s = null;

        switch (s)
        {
            case ""a"":
                M();
                break;

            case ""a"":
                M();
                break;
        }
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }
    }
}
