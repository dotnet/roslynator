// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests;

public class RR0046ExtractTypeDeclarationToNewFileTests : AbstractCSharpRefactoringVerifier
{
    public override string RefactoringId { get; } = RefactoringIdentifiers.ExtractTypeDeclarationToNewFile;

    [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ExtractTypeDeclarationToNewFile)]
    public async Task Test_Namespace()
    {
        await VerifyRefactoringAsync("""
namespace N
{
    public class C1
    {

    }

    public class [||]C2
    {

    }
}
""", """
namespace N
{
    public class C1
    {

    }
}
""", equivalenceKey: EquivalenceKey.Create(RefactoringId));
    }

    [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ExtractTypeDeclarationToNewFile)]
    public async Task Test_FileScopedNamespace()
    {
        await VerifyRefactoringAsync("""
namespace N;

public class C1
{
}

public class [||]C2
{
}
""", """
namespace N;

public class C1
{
}

""", equivalenceKey: EquivalenceKey.Create(RefactoringId));
    }
}
