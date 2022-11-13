// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0014AddUsingStaticDirectiveTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.AddUsingStaticDirective;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddUsingStaticDirective)]
        public async Task Test_Math_Max()
        {
            await VerifyRefactoringAsync(@"
using System;

class C
{
    void M()
    {
        int max = [|Math|].Max(1, 2);
    }
}
", @"
using System;
using static System.Math;

class C
{
    void M()
    {
        int max = Max(1, 2);
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddUsingStaticDirective)]
        public async Task Test_Math_Max2()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M()
    {
        int max = [|System.Math|].Max(1, 2);
    }
}
", @"using static System.Math;

class C
{
    void M()
    {
        int max = Max(1, 2);
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddUsingStaticDirective)]
        public async Task Test_Math_Max3()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M()
    {
        int max = [|global::System.Math|].Max(1, 2);
    }
}
", @"using static System.Math;

class C
{
    void M()
    {
        int max = Max(1, 2);
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddUsingStaticDirective)]
        public async Task Test_SimpleMemberAccessExpression()
        {
            await VerifyRefactoringAsync(@"
using System;

class C
{
    void M()
    {
        var x = [|StringComparer|].CurrentCulture.GetHashCode();
    }
}
", @"
using System;
using static System.StringComparer;

class C
{
    void M()
    {
        var x = CurrentCulture.GetHashCode();
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddUsingStaticDirective)]
        public async Task Test_SimpleMemberAccessExpression2()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M()
    {
        var x = [|System.StringComparer|].CurrentCulture.GetHashCode();
    }
}
", @"using static System.StringComparer;

class C
{
    void M()
    {
        var x = CurrentCulture.GetHashCode();
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddUsingStaticDirective)]
        public async Task Test_Struct()
        {
            await VerifyRefactoringAsync(@"
using System;

class C
{
    void M()
    {
        var x = [|TimeSpan|].Zero;
    }
}
", @"
using System;
using static System.TimeSpan;

class C
{
    void M()
    {
        var x = Zero;
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }
    }
}
