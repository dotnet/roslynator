// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Tests;
using Xunit;

#pragma warning disable RCS1090

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0021ChangeMethodReturnTypeToVoidTests : AbstractCSharpCodeRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.ChangeMethodReturnTypeToVoid;

        public override CodeVerificationOptions Options => base.Options.AddAllowedCompilerDiagnosticsId("CS0161");

        [Fact]
        public async Task Test_Method()
        {
            await VerifyRefactoringAsync(@"
class C
{
    string M()
    {[||]
        M();
    }
}
", @"
class C
{
    void M()
    {
        M();
    }
}
", RefactoringId);
        }

        [Fact]
        public async Task Test_Method_IEnumerableOfT()
        {
            await VerifyRefactoringAsync(@"
class C
{
    System.Collections.Generic.IEnumerable<string> M()
    {[||]
        M();
    }
}
", @"
class C
{
    void M()
    {
        M();
    }
}
", RefactoringId);
        }

        [Fact]
        public async Task Test_LocalFunction()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M()
    {
        string LF()
        {[||]
            LF();
        }
    }
}
", @"
class C
{
    void M()
    {
        void LF()
        {
            LF();
        }
    }
}
", RefactoringId);
        }

        [Fact]
        public async Task TestNoRefactoring_MethodWithReturn()
        {
            await VerifyNoRefactoringAsync(@"
class C
{
    string M()
    {[||]
        return M();
    }
}
", RefactoringId);
        }

        [Fact]
        public async Task TestNoRefactoring_VoidMethod()
        {
            await VerifyNoRefactoringAsync(@"
class C
{
    void M()
    {[||]
        M();
    }
}
", RefactoringId);
        }

        [Fact]
        public async Task TestNoRefactoring_Iterator()
        {
            await VerifyNoRefactoringAsync(@"
using System.Collections.Generic;

class C
{
    IEnumerable<string> M()
    {[||]
        yield return null;
    }
}
", RefactoringId);
        }

        [Fact]
        public async Task TestNoRefactoring_Iterator_YieldBreak()
        {
            await VerifyNoRefactoringAsync(@"
using System.Collections.Generic;

class C
{
    IEnumerable<string> M()
    {[||]
        yield break;
    }
}
", RefactoringId);
        }

        [Fact]
        public async Task TestNoRefactoring_ReturnsTask()
        {
            await VerifyNoRefactoringAsync(@"
using System.Threading.Tasks;

class C
{
    async Task M()
    {[||]
        string x = await Task.FromResult(default(string));
    }
}
", RefactoringId);
        }

        [Fact]
        public async Task TestNoRefactoring_Throws()
        {
            await VerifyNoRefactoringAsync(@"
using System;

class C
{
    string M()
    {[||]
        throw new InvalidOperationException();
    }
}
", RefactoringId);
        }

        [Fact]
        public async Task TestNoRefactoring_InterfaceImplementation()
        {
            await VerifyNoRefactoringAsync(@"
interface IFoo
{
    string M();
}

class C : IFoo
{
    public string M()
    {[||]
        M();
    }
}
", RefactoringId);
        }

        [Fact]
        public async Task TestNoRefactoring_Override()
        {
            await VerifyNoRefactoringAsync(@"
class B
{
    public virtual string M() => null;
}

class C : B
{
    public override string M()
    {[||]
        object x = null;
    }
}
", RefactoringId);
        }
    }
}
