// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.CSharp.Extensions;
using Roslynator.Metadata;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace CodeGenerator
{
    public class RefactoringIdentifiersGenerator : Generator
    {
        public CompilationUnitSyntax Generate(IEnumerable<RefactoringInfo> refactorings)
        {
            return CompilationUnit()
                .WithMembers(
                    NamespaceDeclaration(DefaultNamespace)
                        .WithMembers(
                            ClassDeclaration("RefactoringIdentifiers")
                                .WithModifiers(ModifierFactory.PublicStatic())
                                .WithMembers(
                                    CreateMembers(refactorings))));
        }

        private static IEnumerable<MemberDeclarationSyntax> CreateMembers(IEnumerable<RefactoringInfo> refactorings)
        {
            foreach (RefactoringInfo refactoring in refactorings)
                yield return CreateConstantDeclaration(refactoring.Identifier);
        }

        private static MemberDeclarationSyntax CreateConstantDeclaration(string name)
        {
            return FieldDeclaration(ModifierFactory.PublicConst(), StringType(), name, StringLiteralExpression(name));
        }
    }
}
