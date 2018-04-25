// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CodeRefactorings;
using Roslynator.CSharp.Refactorings;
using Xunit;
using static Roslynator.Tests.CSharp.CSharpCodeRefactoringVerifier;

namespace Roslynator.Refactorings.Tests
{
    public static class RR0127InlineConstantValueTests
    {
        private const string RefactoringId = RefactoringIdentifiers.InlineConstantValue;

        private static CodeRefactoringProvider CodeRefactoringProvider { get; } = new RoslynatorCodeRefactoringProvider();

        [Fact]
        public static void TestCodeRefactoring_Field()
        {
            VerifyRefactoring(
@"
namespace A.B
{
    class C
    {
        public const string K = @""x"";
        public const string K2 = K;
        public const string K3 = K2;
        public const string K4 = null;

        void M(string s)
        {
            s = <<<K>>>;
            s = <<<K3>>>;
            s = <<<C.K>>>;
            s = <<<A.B.C.K>>>;
            s = <<<K4>>>;
        }
    }
}
", @"
namespace A.B
{
    class C
    {
        public const string K = @""x"";
        public const string K2 = K;
        public const string K3 = K2;
        public const string K4 = null;

        void M(string s)
        {
            s = @""x"";
            s = @""x"";
            s = @""x"";
            s = @""x"";
            s = null;
        }
    }
}
", CodeRefactoringProvider, RefactoringId);
        }

        [Fact]
        public static void TestCodeRefactoring_Field2()
        {
            VerifyRefactoring(
@"
class C
{
    public const bool KB = true;
    public const char KC = '\n';
    public const int KI = int.MaxValue;
    public const long KL = 1;

    void M(string s)
    {
        bool b = <<<KB>>>;
        char c = <<<KC>>>;
        int i = <<<KI>>>;
        long l = <<<KL>>>;
    }
}
", @"
class C
{
    public const bool KB = true;
    public const char KC = '\n';
    public const int KI = int.MaxValue;
    public const long KL = 1;

    void M(string s)
    {
        bool b = true;
        char c = '\n';
        int i = 2147483647;
        long l = 1;
    }
}
", CodeRefactoringProvider, RefactoringId);
        }

        [Fact]
        public static void TestCodeRefactoring_Local()
        {
            VerifyRefactoring(
@"
class C
{
    string M(string s)
    {
        const string k = @""x"";
        const string k2 = k;
        const string k3 = k2;

        s += <<<k>>>;
        s += <<<k3>>>;

        return k3;
    }
}
", @"
class C
{
    string M(string s)
    {
        const string k = @""x"";
        const string k2 = k;
        const string k3 = k2;

        s += @""x"";
        s += @""x"";

        return k3;
    }
}
", CodeRefactoringProvider, RefactoringId);
        }

        [Fact]
        public static void TestNoCodeRefactoring()
        {
            VerifyNoRefactoring(
@"
using System;

class C
{
    public readonly string F = null;

    void M(string s)
    {
        s = <<<""x"">>>;
        s = <<<""x"" + ""x"">>>;
        s = <<<F>>>;
        s = <<<string.Empty>>>;
        var options = <<<StringSplitOptions.None>>>;
    }
}
", CodeRefactoringProvider, RefactoringId);
        }
    }
}
