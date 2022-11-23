// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests;

public class RR0180InlineUsingStaticDirectiveTests : AbstractCSharpRefactoringVerifier
{
    public override string RefactoringId { get; } = RefactoringIdentifiers.InlineUsingStaticDirective;

    [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.InlineUsingStaticDirective)]
    public async Task Test()
    {
        await VerifyRefactoringAsync(@"
using System.Linq;
using [||]static System.Linq.Enumerable;

class C
{
    void M()
    {
        #region
        Empty<object>();
        #endregion

        Empty<object>();
    }
}
", @"
using System.Linq;

class C
{
    void M()
    {
        #region
        Enumerable.Empty<object>();
        #endregion

        Enumerable.Empty<object>();
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
    }

    [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.InlineUsingStaticDirective)]
    public async Task Test2()
    {
        await VerifyRefactoringAsync(@"
using System;
using [||]static System.StringComparer;

class C
{
    void M()
    {
        var a = CurrentCulture.GetHashCode();
        var b = CurrentCulture;
        var c = StringComparer.CurrentCulture;
        var d = System.StringComparer.CurrentCulture;
        var e = global::System.StringComparer.CurrentCulture;
    }
}
", @"
using System;

class C
{
    void M()
    {
        var a = StringComparer.CurrentCulture.GetHashCode();
        var b = StringComparer.CurrentCulture;
        var c = StringComparer.CurrentCulture;
        var d = System.StringComparer.CurrentCulture;
        var e = global::System.StringComparer.CurrentCulture;
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
    }

    [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.InlineUsingStaticDirective)]
    public async Task Test_Math_Max()
    {
        await VerifyRefactoringAsync(@"
using System;
using [||]static System.Math;

class C
{
    void M()
    {
        var max = Max(1, 2);
        var min = Min(1, 2);
    }
}
", @"
using System;

class C
{
    void M()
    {
        var max = Math.Max(1, 2);
        var min = Math.Min(1, 2);
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
    }
}
