// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Roslynator.CSharp.Refactorings;
using Roslynator.Testing.CSharp.Xunit;

namespace Roslynator.Testing.CSharp;

public abstract class AbstractCSharpRefactoringVerifier : XunitRefactoringVerifier<RoslynatorCodeRefactoringProvider>
{
    public abstract string RefactoringId { get; }

    public override CSharpTestOptions Options => DefaultCSharpTestOptions.Value;
}
