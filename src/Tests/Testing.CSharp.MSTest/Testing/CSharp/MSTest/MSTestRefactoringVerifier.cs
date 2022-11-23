// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CodeRefactorings;

namespace Roslynator.Testing.CSharp.MSTest;

/// <summary>
/// Represents verifier for a C# code refactoring.
/// </summary>
public abstract class MSTestRefactoringVerifier<TRefactoringProvider> : CSharpRefactoringVerifier<TRefactoringProvider>
    where TRefactoringProvider : CodeRefactoringProvider, new()
{
    /// <summary>
    /// Initializes a new instance of <see cref="MSTestRefactoringVerifier{TRefactoringProvider}"/>.
    /// </summary>
    protected MSTestRefactoringVerifier() : base(MSTestAssert.Instance)
    {
    }
}
