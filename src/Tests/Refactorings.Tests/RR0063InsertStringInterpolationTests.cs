// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0063InsertStringInterpolationTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.InsertStringInterpolation;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.InsertStringInterpolation)]
        public async Task Test_EmptyStringLiteral()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M()
    {
        var x = ""[||]"";
    }
}
", @"
class C
{
    void M()
    {
        var x = $""{}"";
    }
}
",
equivalenceKey: EquivalenceKey.Create(RefactoringId), options: Options.AddAllowedCompilerDiagnosticId("CS1733"));
        }


        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.InsertStringInterpolation)]
        public async Task Test_EmptyInterpolatedString()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M()
    {
        var x = $""[||]"";
    }
}
", @"
class C
{
    void M()
    {
        var x = $""{}"";
    }
}
",
equivalenceKey: EquivalenceKey.Create(RefactoringId), options: Options.AddAllowedCompilerDiagnosticId("CS1733"));
        }
    }
}
