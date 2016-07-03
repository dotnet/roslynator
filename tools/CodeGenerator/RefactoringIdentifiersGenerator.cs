// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp;
using Pihrtsoft.CodeAnalysis.Metadata;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Pihrtsoft.CodeAnalysis.CSharp.CSharpFactory;

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
                .WithMember(
                    NamespaceDeclaration(DefaultNamespace)
                        .WithMember(
                            ClassDeclaration("RefactoringIdentifiers")
                                .WithModifiers(
                                    SyntaxKind.PublicKeyword,
                                    SyntaxKind.StaticKeyword)
                                .WithMembers(
                                    CreateMembers(refactorings))));
        }

        private static IEnumerable<MemberDeclarationSyntax> CreateMembers(IEnumerable<RefactoringInfo> refactorings)
        {
            foreach (RefactoringInfo refactoring in refactorings)
                yield return CreateConstantDeclaration(refactoring.Identifier);

            //yield return MethodDeclaration(ParseTypeName("ImmutableArray<string>"), "GetIdentifiers")
            //    .WithModifiers(SyntaxKind.InternalKeyword, SyntaxKind.StaticKeyword)
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
            return FieldDeclaration(StringType(), name, StringLiteralExpression(name))
                .WithModifiers(SyntaxKind.PublicKeyword, SyntaxKind.ConstKeyword);
        }
    }
}
