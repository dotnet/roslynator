// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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

            if (methodDeclaration.Parent is not ClassDeclarationSyntax classDeclaration)
                return;

            if (classDeclaration.Modifiers.Contains(SyntaxKind.SealedKeyword))
                return;

            context.RegisterRefactoring(
                "Make method virtual",
                ct => RefactorAsync(context.Document, methodDeclaration, ct),
                RefactoringDescriptors.MakeMemberVirtual);
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

                ExpressionSyntax expression = methodSymbol.ReturnType.GetDefaultValueSyntax(returnType, document.GetDefaultSyntaxOptions());

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
