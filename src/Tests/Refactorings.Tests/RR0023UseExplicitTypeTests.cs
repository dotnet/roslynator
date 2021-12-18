// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0023UseExplicitTypeTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.UseExplicitType;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.UseExplicitType)]
        public async Task Test_NamespaceConflict()
        {
            await VerifyRefactoringAsync(@"
using B;

#pragma warning disable IDE0008

namespace A.B.C
{
    class C
    {
        void M()
        {
            [|var|] x = default(global::B.X);
        }
    }
}

namespace B
{
    class X
    {
    }
}", @"
using B;

#pragma warning disable IDE0008

namespace A.B.C
{
    class C
    {
        void M()
        {
            X x = default(global::B.X);
        }
    }
}

namespace B
{
    class X
    {
    }
}", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }
    }
}
