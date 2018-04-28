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
            VerifyRefactoring(@"
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
        public static void TestCodeRefactoring_BoolField()
        {
            VerifyRefactoring(@"
class C
{
    public const bool KB = true;

    void M()
    {
        bool b = <<<KB>>>;
    }
}
", @"
class C
{
    public const bool KB = true;

    void M()
    {
        bool b = true;
    }
}
", CodeRefactoringProvider, RefactoringId);
        }

        [Fact]
        public static void TestCodeRefactoring_CharField()
        {
            VerifyRefactoring(@"
class C
{
    public const char KC = '\n';

    void M()
    {
        char c = <<<KC>>>;
    }
}
", @"
class C
{
    public const char KC = '\n';

    void M()
    {
        char c = '\n';
    }
}
", CodeRefactoringProvider, RefactoringId);
        }

        [Fact]
        public static void TestCodeRefactoring_IntField()
        {
            VerifyRefactoring(@"
class C
{
    public const int KI = int.MaxValue;

    void M()
    {
        int i = <<<KI>>>;
    }
}
", @"
class C
{
    public const int KI = int.MaxValue;

    void M()
    {
        int i = 2147483647;
    }
}
", CodeRefactoringProvider, RefactoringId);
        }

        [Fact]
        public static void TestCodeRefactoring_LongField()
        {
            VerifyRefactoring(@"
class C
{
    public const long KL = 1;

    void M()
    {
        long l = <<<KL>>>;
    }
}
", @"
class C
{
    public const long KL = 1;

    void M()
    {
        long l = 1;
    }
}
", CodeRefactoringProvider, RefactoringId);
        }

        [Fact]
        public static void TestCodeRefactoring_MultipleDocuments()
        {
            VerifyRefactoring(@"
namespace A.B
{
    class C
    {
        public const string K = C2.K2;

        void M(string s)
        {
            s = <<<K>>>;
        }
    }
}
",
new string[] { @"
namespace A.B
{
    class C2
    {
        public const string K2 = C3.K3;
    }
}
", @"
namespace A.B
{
    class C3
    {
        public const string K3 = @""x"";
    }
}
", }, @"
namespace A.B
{
    class C
    {
        public const string K = C2.K2;

        void M(string s)
        {
            s = @""x"";
        }
    }
}
", CodeRefactoringProvider, RefactoringId);
        }

        [Fact]
        public static void TestCodeRefactoring_Local()
        {
            VerifyRefactoring(@"
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
            VerifyNoRefactoring(@"
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
