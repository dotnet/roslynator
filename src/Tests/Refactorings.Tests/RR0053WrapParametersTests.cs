// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0053WrapParametersTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.WrapParameters;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.WrapParameters)]
        public async Task Test_ToMultiLine_NoNamespace()
        {
            await VerifyRefactoringAsync(@"
record R([||]object p, object q, object r)
{
    void M()
    {
    }
}

namespace System.Runtime.CompilerServices { internal static class IsExternalInit {} }
", @"
record R(
    object p,
    object q,
    object r)
{
    void M()
    {
    }
}

namespace System.Runtime.CompilerServices { internal static class IsExternalInit {} }
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }
    }
}
