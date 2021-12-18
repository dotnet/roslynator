// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RRTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.AddBraces;

        //[Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddBraces)]
        public async Task Test()
        {
            await VerifyRefactoringAsync(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class C
{
    void M()
    {
    }
}
", @"
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        //[Theory, Trait(Traits.Refactoring, RefactoringIdentifiers.AddBraces)]
        //[InlineData("", "")]
        public async Task Test2(string source, string expected)
        {
            await VerifyRefactoringAsync(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class C
{
    void M()
    {
    }
}
", source, expected, equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        //[Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.AddBraces)]
        public async Task TestNoRefactoring()
        {
            await VerifyNoRefactoringAsync(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class C
{
    void M()
    {
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }
    }
}
