// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.MakeMemberVirtual
{
    internal static class MakePropertyVirtualRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, PropertyDeclarationSyntax propertyDeclaration)
        {
            if (!propertyDeclaration.Modifiers.Contains(SyntaxKind.AbstractKeyword))
                return;

            var classDeclaration = propertyDeclaration.Parent as ClassDeclarationSyntax;

            if (classDeclaration == null)
                return;

            if (classDeclaration.Modifiers.Contains(SyntaxKind.SealedKeyword))
                return;

            context.RegisterRefactoring(
                "Make property virtual",
                cancellationToken => RefactorAsync(context.Document, propertyDeclaration, cancellationToken));
        }

        private static Task<Document> RefactorAsync(
            Document document,
            PropertyDeclarationSyntax propertyDeclaration,
            CancellationToken cancellationToken)
        {
            PropertyDeclarationSyntax newNode = propertyDeclaration
                .WithSemicolonToken(default(SyntaxToken))
                .WithAccessorList(MakeMemberAbstractHelper.ExpandAccessorList(propertyDeclaration.AccessorList))
                .WithModifiers(propertyDeclaration.Modifiers.Replace(SyntaxKind.AbstractKeyword, SyntaxKind.VirtualKeyword))
                .WithTriviaFrom(propertyDeclaration)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(propertyDeclaration, newNode, cancellationToken);
        }
    }
}