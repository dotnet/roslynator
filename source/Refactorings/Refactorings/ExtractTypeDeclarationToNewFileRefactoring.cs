// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ExtractTypeDeclarationToNewFileRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, ClassDeclarationSyntax classDeclaration)
        {
            SyntaxToken identifier = classDeclaration.Identifier;
            ComputeRefactorings(context, classDeclaration, identifier);
        }

        public static void ComputeRefactorings(RefactoringContext context, StructDeclarationSyntax structDeclaration)
        {
            SyntaxToken identifier = structDeclaration.Identifier;
            ComputeRefactorings(context, structDeclaration, identifier);
        }

        public static void ComputeRefactorings(RefactoringContext context, InterfaceDeclarationSyntax interfaceDeclaration)
        {
            SyntaxToken identifier = interfaceDeclaration.Identifier;
            ComputeRefactorings(context, interfaceDeclaration, identifier);
        }

        public static void ComputeRefactorings(RefactoringContext context, EnumDeclarationSyntax enumDeclaration)
        {
            SyntaxToken identifier = enumDeclaration.Identifier;
            ComputeRefactorings(context, enumDeclaration, identifier);
        }

        public static void ComputeRefactorings(RefactoringContext context, DelegateDeclarationSyntax delegateDeclaration)
        {
            SyntaxToken identifier = delegateDeclaration.Identifier;
            ComputeRefactorings(context, delegateDeclaration, identifier);
        }

        private static void ComputeRefactorings(RefactoringContext context, MemberDeclarationSyntax memberDeclaration, SyntaxToken identifier)
        {
            if (identifier.Span.Contains(context.Span)
                && memberDeclaration.IsParentKind(SyntaxKind.NamespaceDeclaration, SyntaxKind.CompilationUnit)
                && context.IsRootCompilationUnit
                && ExtractTypeDeclarationToNewDocumentRefactoring.GetNonNestedTypeDeclarations((CompilationUnitSyntax)context.Root).Skip(1).Any())
            {
                context.RegisterRefactoring(
                    ExtractTypeDeclarationToNewDocumentRefactoring.GetTitle(identifier.ValueText),
                    cancellationToken => ExtractTypeDeclarationToNewDocumentRefactoring.RefactorAsync(context.Document, memberDeclaration, cancellationToken));
            }
        }
    }
}