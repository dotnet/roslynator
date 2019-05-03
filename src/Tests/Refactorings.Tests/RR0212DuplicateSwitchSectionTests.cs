// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Tests;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0212DuplicateSwitchSectionTests : AbstractCSharpRefactoringVerifier
    {
        private readonly CodeVerificationOptions _options;

        public RR0212DuplicateSwitchSectionTests()
        {
            _options = base.Options.AddAllowedCompilerDiagnosticId("CS0152");
        }

        public override string RefactoringId { get; } = RefactoringIdentifiers.DuplicateSwitchSection;

        public override CodeVerificationOptions Options => _options;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.DuplicateSwitchSection)]
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
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.DuplicateSwitchSection)]
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
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.DuplicateSwitchSection)]
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
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.DuplicateSwitchSection)]
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
", equivalenceKey: RefactoringId);
        }
    }
}
