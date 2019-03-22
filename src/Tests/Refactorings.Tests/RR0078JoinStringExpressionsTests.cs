// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0078JoinStringExpressionsTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.JoinStringExpressions;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.JoinStringExpressions)]
        public async Task Test()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M(string s)
    {
        s = s + [|""a"" + @""b""|];
    }
}
", @"
class C
{
    void M(string s)
    {
        s = s + ""ab"";
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.JoinStringExpressions)]
        public async Task Test_ToInterpolatedString()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M(string s1, string s2, string s3)
    {
        s1 = [|s1 + ""a"" + s2 +  @""b"" + s3|];
    }
}
", @"
class C
{
    void M(string s1, string s2, string s3)
    {
        s1 = $""{s1}a{s2}b{s3}"";
    }
}
", equivalenceKey: RefactoringId);
        }
    }
}
