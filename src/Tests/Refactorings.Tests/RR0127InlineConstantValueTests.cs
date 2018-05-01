// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Xunit;

#pragma warning disable RCS1090

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0127InlineConstantValueTests : AbstractCSharpCodeRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.InlineConstantValue;

        [Fact]
        public async Task Test_Field()
        {
            await VerifyRefactoringAsync(@"
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
            s = [|K|];
            s = [|K3|];
            s = [|C.K|];
            s = [|A.B.C.K|];
            s = [|K4|];
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
", RefactoringId);
        }

        [Fact]
        public async Task Test_BoolField()
        {
            await VerifyRefactoringAsync(@"
class C
{
    public const bool KB = true;

    void M()
    {
        bool b = [|KB|];
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
", RefactoringId);
        }

        [Fact]
        public async Task Test_CharFieldAsync()
        {
            await VerifyRefactoringAsync(@"
class C
{
    public const char KC = '\n';

    void M()
    {
        char c = [|KC|];
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
", RefactoringId);
        }

        [Fact]
        public async Task Test_IntFieldAsync()
        {
            await VerifyRefactoringAsync(@"
class C
{
    public const int KI = int.MaxValue;

    void M()
    {
        int i = [|KI|];
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
", RefactoringId);
        }

        [Fact]
        public async Task Test_LongFieldAsync()
        {
            await VerifyRefactoringAsync(@"
class C
{
    public const long KL = 1;

    void M()
    {
        long l = [|KL|];
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
", RefactoringId);
        }

        [Fact]
        public async Task Test_MultipleDocumentsAsync()
        {
            await VerifyRefactoringAsync(@"
namespace A.B
{
    class C
    {
        public const string K = C2.K2;

        void M(string s)
        {
            s = [|K|];
        }
    }
}
", @"
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
",
RefactoringId, additionalSources: new string[] { @"
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
", });
        }

        [Fact]
        public async Task TestNoRefactoring()
        {
            await VerifyNoRefactoringAsync(@"
using System;

class C
{
    public readonly string F = null;

    void M(string s)
    {
        s = [|""x""|];
        s = [|""x"" + ""x""|];
        s = [|F|];
        s = [|string.Empty|];
        var options = [|StringSplitOptions.None|];
    }
}
", RefactoringId);
        }
    }
}
