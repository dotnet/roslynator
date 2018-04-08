// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class FormatDeclarationBracesRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax declaration,
            CancellationToken cancellationToken)
        {
            MemberDeclarationSyntax newNode = GetNewDeclaration(declaration)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(declaration, newNode, cancellationToken);
        }

        private static MemberDeclarationSyntax GetNewDeclaration(MemberDeclarationSyntax declaration)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    {
                        var classDeclaration = (ClassDeclarationSyntax)declaration;

                        return classDeclaration
                            .WithOpenBraceToken(classDeclaration.OpenBraceToken.WithoutTrailingTrivia())
                            .WithCloseBraceToken(classDeclaration.CloseBraceToken.WithLeadingTrivia(CSharpFactory.NewLine()));
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var structDeclaration = (StructDeclarationSyntax)declaration;

                        return structDeclaration
                            .WithOpenBraceToken(structDeclaration.OpenBraceToken.WithoutTrailingTrivia())
                            .WithCloseBraceToken(structDeclaration.CloseBraceToken.WithLeadingTrivia(CSharpFactory.NewLine()));
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var interfaceDeclaration = (InterfaceDeclarationSyntax)declaration;

                        return interfaceDeclaration
                            .WithOpenBraceToken(interfaceDeclaration.OpenBraceToken.WithoutTrailingTrivia())
                            .WithCloseBraceToken(interfaceDeclaration.CloseBraceToken.WithLeadingTrivia(CSharpFactory.NewLine()));
                    }
            }

            return declaration;
        }
    }
}
