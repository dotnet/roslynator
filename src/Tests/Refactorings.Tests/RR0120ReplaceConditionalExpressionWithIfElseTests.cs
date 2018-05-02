// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Xunit;

#pragma warning disable RCS1090

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0120ReplaceConditionalExpressionWithIfElseTests : AbstractCSharpCodeRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.ReplaceConditionalExpressionWithIfElse;

        [Fact]
        public async Task Test_LocalDeclaration()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M(bool f, string x, string y)
    {
        string s = [||](f) ? x : y;
    }
}
", @"
class C
{
    void M(bool f, string x, string y)
    {
        string s;
        if (f)
        {
            s = x;
        }
        else
        {
            s = y;
        }
    }
}
", RefactoringId);
        }

        [Fact]
        public async Task Test_LocalDeclaration_Multiline()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M(bool f, string x, string y)
    {
        string s = [||](f)
            ? x
            : y;
    }
}
", @"
class C
{
    void M(bool f, string x, string y)
    {
        string s;
        if (f)
        {
            s = x;
        }
        else
        {
            s = y;
        }
    }
}
", RefactoringId);
        }

        [Fact]
        public async Task Test_SimpleAssignment()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M(bool f, string x, string y)
    {
        string s = null;
        s = [||](f) ? x : y;
    }
}
", @"
class C
{
    void M(bool f, string x, string y)
    {
        string s = null;
        if (f)
        {
            s = x;
        }
        else
        {
            s = y;
        }
    }
}
", RefactoringId);
        }

        [Fact]
        public async Task Test_SimpleAssignment_Multiline()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M(bool f, string x, string y)
    {
        string s = null;
        s = [||](f)
            ? x
            : y;
    }
}
", @"
class C
{
    void M(bool f, string x, string y)
    {
        string s = null;
        if (f)
        {
            s = x;
        }
        else
        {
            s = y;
        }
    }
}
", RefactoringId);
        }

        [Fact]
        public async Task Test_ReturnStatement()
        {
            await VerifyRefactoringAsync(@"
class C
{
    string M(bool f, string x, string y)
    {
        return [||](f) ? x : y;
    }
}
", @"
class C
{
    string M(bool f, string x, string y)
    {
        if (f)
        {
            return x;
        }
        else
        {
            return y;
        }
    }
}
", RefactoringId);
        }

        [Fact]
        public async Task Test_ReturnStatement_Multiline()
        {
            await VerifyRefactoringAsync(@"
class C
{
    string M(bool f, string x, string y)
    {
        return [||](f)
            ? x
            : y;
    }
}
", @"
class C
{
    string M(bool f, string x, string y)
    {
        if (f)
        {
            return x;
        }
        else
        {
            return y;
        }
    }
}
", RefactoringId);
        }

        [Fact]
        public async Task Test_YieldReturnStatement()
        {
            await VerifyRefactoringAsync(@"
using System.Collections.Generic;

class C
{
    IEnumerable<string> M(bool f, string x, string y)
    {
        yield return [||](f) ? x : y;
    }
}
", @"
using System.Collections.Generic;

class C
{
    IEnumerable<string> M(bool f, string x, string y)
    {
        if (f)
        {
            yield return x;
        }
        else
        {
            yield return y;
        }
    }
}
", RefactoringId);
        }

        [Fact]
        public async Task Test_YieldReturnStatement_Multiline()
        {
            await VerifyRefactoringAsync(@"
using System.Collections.Generic;

class C
{
    IEnumerable<string> M(bool f, string x, string y)
    {
        yield return [||](f)
            ? x
            : y;
    }
}
", @"
using System.Collections.Generic;

class C
{
    IEnumerable<string> M(bool f, string x, string y)
    {
        if (f)
        {
            yield return x;
        }
        else
        {
            yield return y;
        }
    }
}
", RefactoringId);
        }
    }
}
