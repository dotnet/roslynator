// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0112RenameIdentifierAccordingToTypeNameTest : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.RenameIdentifierAccordingToTypeName;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.RenameIdentifierAccordingToTypeName)]
        public async Task TestRefactoring_VariableDeclaration()
        {
            await VerifyRefactoringAsync(@"
using System;
using Microsoft.CodeAnalysis;

class C
{
        void M()
        {
            DateTime [||]dt = default(DateTime);
        }
}
", @"
using System;
using Microsoft.CodeAnalysis;

class C
{
        void M()
        {
            DateTime dateTime = default(DateTime);
        }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.RenameIdentifierAccordingToTypeName)]
        public async Task TestRefactoring_VariableDeclaration_Syntax()
        {
            await VerifyRefactoringAsync(@"
using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

class C
{
        void M()
        {
            CompilationUnitSyntax [||]cu = default(CompilationUnitSyntax);
        }
}
", @"
using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

class C
{
        void M()
        {
            CompilationUnitSyntax compilationUnit = default(CompilationUnitSyntax);
        }
}
", equivalenceKey: EquivalenceKey.Create(RefactoringId));
        }
    }
}
