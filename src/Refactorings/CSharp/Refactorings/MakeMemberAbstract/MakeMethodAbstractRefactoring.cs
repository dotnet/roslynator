// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.MakeMemberAbstract
{
    internal static class MakeMethodAbstractRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, MethodDeclarationSyntax methodDeclaration)
        {
            SyntaxTokenList modifiers = methodDeclaration.Modifiers;

            if (modifiers.ContainsAny(SyntaxKind.AbstractKeyword, SyntaxKind.StaticKeyword))
                return;

            if ((methodDeclaration.Parent as ClassDeclarationSyntax)?.Modifiers.Contains(SyntaxKind.AbstractKeyword) != true)
                return;

            context.RegisterRefactoring(
                "Make method abstract",
                cancellationToken => RefactorAsync(context.Document, methodDeclaration, cancellationToken));
        }

        public static Task<Document> RefactorAsync(
            Document document,
            MethodDeclarationSyntax methodDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            MethodDeclarationSyntax newNode = methodDeclaration
                .WithExpressionBody(null)
                .WithBody(null)
                .WithSemicolonToken(SemicolonToken())
                .InsertModifier(SyntaxKind.AbstractKeyword)
                .RemoveModifier(SyntaxKind.VirtualKeyword)
                .WithTriviaFrom(methodDeclaration)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(methodDeclaration, newNode, cancellationToken);
        }
    }
}
