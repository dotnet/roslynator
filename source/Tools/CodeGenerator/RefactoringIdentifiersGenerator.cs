// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator;
using Roslynator.CSharp;
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
                //.WithUsings(
                //    UsingDirective(
                //        ParseName("")))
                .WithMembers(
                    NamespaceDeclaration(DefaultNamespace)
                        .WithMembers(
                            ClassDeclaration("RefactoringIdentifiers")
                                .WithModifiers(Modifiers.PublicStatic())
                                .WithMembers(
                                    CreateMembers(refactorings))));
        }

        private static IEnumerable<MemberDeclarationSyntax> CreateMembers(IEnumerable<RefactoringInfo> refactorings)
        {
            foreach (RefactoringInfo refactoring in refactorings)
                yield return CreateConstantDeclaration(refactoring.Identifier);

            //yield return MethodDeclaration(ParseTypeName("ImmutableArray<string>"), "GetIdentifiers")
            //    .WithModifiers(Modifiers.InternalStatic())
            //    .WithStatements(
            //        ReturnStatement(
            //            InvocationExpression(
            //                SimpleMemberAccessExpression("ImmutableArray", "Create"),
            //                ArgumentList(refactorings.Select(identifier =>
            //                {
            //                    return Argument(identifier.Identifier)
            //                        .WithLeadingTrivia(NewLine);
            //                }).ToArray())
            //            )
            //        )
            //    );
        }

        private static MemberDeclarationSyntax CreateConstantDeclaration(string name)
        {
            return FieldDeclaration(Modifiers.PublicConst(), StringType(), name, StringLiteralExpression(name));
        }
    }
}
