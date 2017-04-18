// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.CSharp.Refactorings;
using Roslynator.Metadata;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace CodeGenerator
{
    public class RefactoringIdentifiersGenerator : Generator
    {
        public StringComparer InvariantComparer { get; } = StringComparer.InvariantCulture;

        public CompilationUnitSyntax Generate(IEnumerable<RefactoringDescriptor> refactorings)
        {
            return CompilationUnit()
                .WithUsings(List(new UsingDirectiveSyntax[] { }))
                .WithMembers(
                    NamespaceDeclaration(IdentifierName(DefaultNamespace))
                        .WithMembers(
                            ClassDeclaration("RefactoringIdentifiers")
                                .WithModifiers(Modifiers.PublicStaticPartial())
                                .WithMembers(
                                    CreateMembers(refactorings.OrderBy(f => f.Identifier, InvariantComparer)))));
        }

        private static IEnumerable<MemberDeclarationSyntax> CreateMembers(IEnumerable<RefactoringDescriptor> refactorings)
        {
            foreach (RefactoringDescriptor refactoring in refactorings)
                yield return FieldDeclaration(Modifiers.PublicConst(), StringType(), refactoring.Identifier, AddExpression(IdentifierName("Prefix"), StringLiteralExpression(refactoring.Id.Substring(RefactoringIdentifiers.Prefix.Length))));
        }
    }
}
