// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests;

public class RR0016CallExtensionMethodAsInstanceMethodTests : AbstractCSharpRefactoringVerifier
{
    public override string RefactoringId { get; } = RefactoringIdentifiers.CallExtensionMethodAsInstanceMethod;

    [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.CallExtensionMethodAsInstanceMethod)]
    public async Task Test()
    {
        await VerifyRefactoringAsync(@"
using System;
using System.Linq;
using System.Collections.Generic;

class C
{
    void M(List<int> items)
    {
        var x = Enumerable.Select[||](items, f => f.ToString());
    }
}
", @"
using System;
using System.Linq;
using System.Collections.Generic;

class C
{
    void M(List<int> items)
    {
        var x = items.Select(f => f.ToString());
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
    }
}
