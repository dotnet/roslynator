// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0185RemoveInstantiationOfLocalVariableTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.RemoveInstantiationOfLocalVariable;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.RemoveInstantiationOfLocalVariable)]
        public async Task Test_WithoutNullableContext()
        {
            await VerifyRefactoringAsync(@"
class C
{
    void M()
    {
        var x = [||]new string(' ', 1);
    }
}
", @"
class C
{
    void M()
    {
        string x = null;
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringDescriptors.RemoveInstantiationOfLocalVariable));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.RemoveInstantiationOfLocalVariable)]
        public async Task Test_WithNullableContext()
        {
            await VerifyRefactoringAsync(@"
#nullable enable annotations

class C
{
    void M()
    {
        var x = [||]new string(' ', 1);
    }
}
", @"
#nullable enable annotations

class C
{
    void M()
    {
        string? x = null;
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringDescriptors.RemoveInstantiationOfLocalVariable));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.RemoveInstantiationOfLocalVariable)]
        public async Task TestNoDiagnostic_SpanInInitializer()
        {
            await VerifyNoRefactoringAsync(@"
class C
{
    void M()
    {
        var x = new string(' ', 1) { [||] };
    }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringDescriptors.RemoveInstantiationOfLocalVariable));
        }
    }
}
