// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CodeRefactorings;
using Roslynator.CSharp.Refactorings;
using Xunit;
using static Roslynator.Tests.CSharp.CSharpCodeRefactoringVerifier;

namespace Roslynator.Refactorings.Tests
{
    public static class RR0166UseConditionalExpressionInsteadOfIfTests
    {
        private const string RefactoringId = RefactoringIdentifiers.UseConditionalExpressionInsteadOfIf;

        private static CodeRefactoringProvider CodeRefactoringProvider { get; } = new RoslynatorCodeRefactoringProvider();

        [Theory]
        [InlineData("if (f) { z = x; } else { z = y; }", "z = (f) ? x : y;")]
        [InlineData("if (f) z = x; else z = y;", "z = (f) ? x : y;")]
        public static void TestRefactoring_IfElseToAssignmentWithConditionalExpression(string fixableCode, string fixedCode)
        {
            VerifyRefactoring(@"
class C
{
    void M(bool f, string x, string y, string z)
    {
        <<<>>>
    }
}
", fixableCode, fixedCode, CodeRefactoringProvider, RefactoringId);
        }

        [Fact]
        public static void TestRefactoring_AssignmentAndIfElseToAssignmentWithConditionalExpression()
        {
            VerifyRefactoring(@"
class C
{
    void M(bool f, string x, string y, string z)
    {
<<<        z = null;
        if (f)
        {
            z = x;
        }
        else
        {
            z = y;
        }>>>
    }
}
",
@"
class C
{
    void M(bool f, string x, string y, string z)
    {
        z = (f) ? x : y;
    }
}
", CodeRefactoringProvider, RefactoringId);
        }

        [Fact]
        public static void TestRefactoring_LocalDeclarationAndIfElseToAssignmentWithConditionalExpression()
        {
            VerifyRefactoring(@"
class C
{
    void M(bool f, string x, string y)
    {
<<<        string z = null;
        if (f)
        {
            z = x;
        }
        else
        {
            z = y;
        }>>>
    }
}
",
@"
class C
{
    void M(bool f, string x, string y)
    {
        string z = (f) ? x : y;
    }
}
", CodeRefactoringProvider, RefactoringId);
        }

        [Theory]
        [InlineData("if (f) { return x; } else { return y; }", "return (f) ? x : y;")]
        [InlineData("if (f) return x; else return y;", "return (f) ? x : y;")]
        [InlineData("if (f) { return x; } return y;", "return (f) ? x : y;")]
        [InlineData("if (f) return x; return y;", "return (f) ? x : y;")]
        public static void TestRefactoring_IfToReturnWithConditionalExpression(string fixableCode, string fixedCode)
        {
            VerifyRefactoring(@"
class C
{
    string M(bool f, string x, string y, string z)
    {
        <<<>>>
    }
}
", fixableCode, fixedCode, CodeRefactoringProvider, RefactoringId);
        }

        [Theory]
        [InlineData("if (f) { yield return x; } else { yield return y; }", "yield return (f) ? x : y;")]
        [InlineData("if (f) yield return x; else yield return y;", "yield return (f) ? x : y;")]
        public static void TestRefactoring_IfElseToYieldReturnWithConditionalExpression(string fixableCode, string fixedCode)
        {
            VerifyRefactoring(@"
using System.Collections.Generic;

class C
{
    IEnumerable<string> M(bool f, string x, string y, string z)
    {
        <<<>>>
    }
}
", fixableCode, fixedCode, CodeRefactoringProvider, RefactoringId);
        }

        [Fact]
        public static void TestNoRefactoring_IfElseToAssignmentWithConditionalExpression()
        {
            VerifyNoRefactoring(
@"
class C
{
    void M(bool f)
    {
        int? ni;
        <<<>>>if (f)
        {
            ni = null;
        }
        else
        {
            ni = 1;
        }
    }
}
", CodeRefactoringProvider, RefactoringId);
        }

        [Fact]
        public static void TestNoRefactoring_LocalDeclarationAndIfElseAssignmentWithConditionalExpression()
        {
            VerifyNoRefactoring(
@"
class C
{
    void M(bool f)
    {
<<<        int? ni;
        if (f)
        {
            ni = null;
        }
        else
        {
            ni = 1;
        }>>>
    }
}
", CodeRefactoringProvider, RefactoringId);
        }

        [Fact]
        public static void TestNoRefactoring_AssignmentAndIfElseToAssignmentWithConditionalExpression()
        {
            VerifyNoRefactoring(@"
class C
{
    void M(bool f)
    {
        int? ni;
<<<        ni = null;
        if (f)
        {
            ni = null;
        }
        else
        {
            ni = 1;
        }>>>
    }
}
", CodeRefactoringProvider, RefactoringId);
        }

        [Fact]
        public static void TestNoRefactoring_IfElseToYieldReturnWithConditionalExpression()
        {
            VerifyNoRefactoring(@"
using System.Collections.Generic;

class C
{
    IEnumerable<int?> M(bool f)
    {
<<<        if (f)
        {
            yield return null;
        }
        else
        {
            yield return 1;
        }>>>
    }
}
", CodeRefactoringProvider, RefactoringId);
        }

        [Fact]
        public static void TestNoRefactoring_IfElseToReturnWithConditionalExpression()
        {
            VerifyNoRefactoring(@"
class C
{
    int? M(bool f)
    {
<<<        if (f)
        {
            return null;
        }
        else
        {
            return 1;
        }>>>
    }
}
", CodeRefactoringProvider, RefactoringId);
        }

        [Fact]
        public static void TestNoRefactoring_IfReturnToReturnWithConditionalExpression()
        {
            VerifyNoRefactoring(@"
class C
{
    int? M(bool f)
    {
<<<        if (f)
        {
            return null;
        }

        return 1;>>>
    }
}
", CodeRefactoringProvider, RefactoringId);
        }
    }
}
