// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CodeRefactorings;
using Roslynator.CSharp.Testing;
using Roslynator.Testing;
using Roslynator.Testing.Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public abstract class AbstractCSharpRefactoringVerifier : CSharpRefactoringVerifier
    {
        protected override IAssert Assert => XunitAssert.Instance;

        public override CodeRefactoringProvider RefactoringProvider { get; } = new RoslynatorCodeRefactoringProvider();
    }
}
