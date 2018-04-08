// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.MakeMemberVirtual
{
    internal static class MakeMethodVirtualRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, MethodDeclarationSyntax methodDeclaration)
        {
            if (!methodDeclaration.Modifiers.Contains(SyntaxKind.AbstractKeyword))
                return;

            var classDeclaration = methodDeclaration.Parent as ClassDeclarationSyntax;

            if (classDeclaration == null)
                return;

            if (classDeclaration.Modifiers.Contains(SyntaxKind.SealedKeyword))
                return;

            context.RegisterRefactoring(
                "Make method virtual",
                cancellationToken => RefactorAsync(context.Document, methodDeclaration, cancellationToken));
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            MethodDeclarationSyntax methodDeclaration,
            CancellationToken cancellationToken)
        {
            BlockSyntax body = Block();

            TypeSyntax returnType = methodDeclaration.ReturnType;

            if (returnType?.IsVoid() == false)
            {
                SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, cancellationToken);

                ExpressionSyntax expression = methodSymbol.ReturnType.GetDefaultValueSyntax(returnType);

                body = body.AddStatements(ReturnStatement(expression));
            }

            body = body.WithFormatterAnnotation();

            MethodDeclarationSyntax newNode = methodDeclaration
                .WithModifiers(methodDeclaration.Modifiers.Replace(SyntaxKind.AbstractKeyword, SyntaxKind.VirtualKeyword))
                .WithBody(body)
                .WithSemicolonToken(default(SyntaxToken))
                .WithTrailingTrivia(methodDeclaration.GetTrailingTrivia());

            return await document.ReplaceNodeAsync(methodDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}