// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Xunit;

#pragma warning disable RCS1090

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0048FormatArgumentListTests : AbstractCSharpCodeRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.FormatArgumentList;

        [Fact]
        public async Task Test_ToMultiLine()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M(string p1, string p2, string p3)
    {
        M(p1[||], p2, p3);
    }
}
", @"
class C
{
    void M(string p1, string p2, string p3)
    {
        M(
            p1,
            p2,
            p3);
    }
}
", RefactoringId);
        }

        [Fact]
        public async Task Test_ToMultiLine2()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M(string p1, string p2, string p3)
    {
        M[|(p1, p2, p3)|];
    }
}
", @"
class C
{
    void M(string p1, string p2, string p3)
    {
        M(
            p1,
            p2,
            p3);
    }
}
", RefactoringId);
        }

        [Fact]
        public async Task Test_ToSingleLine()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M(string p1, string p2, string p3)
    {
        M(
            p1[||],
            p2,
            p3);
    }
}
", @"
class C
{
    void M(string p1, string p2, string p3)
    {
        M(p1, p2, p3);
    }
}
", RefactoringId);
        }

        [Fact]
        public async Task Test_ToSingleLine2()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M(string p1, string p2, string p3)
    {
        M[|(
            p1,
            p2,
            p3)|];
    }
}
", @"
class C
{
    void M(string p1, string p2, string p3)
    {
        M(p1, p2, p3);
    }
}
", RefactoringId);
        }

        [Fact]
        public async Task TestNoRefactoring()
        {
            await VerifyNoRefactoringAsync(@"
class C
{
    void M(string p1, string p2, string p3)
    {
        M(
            [|p1,
            p2, //x
            p3|]);
    }
}
", RefactoringId);
        }
    }
}
