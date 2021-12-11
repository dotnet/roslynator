// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SemicolonTokenRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, SyntaxToken semicolonToken)
        {
            if (!semicolonToken.IsKind(SyntaxKind.SemicolonToken))
                return;

            if (semicolonToken.IsMissing)
                return;

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertExpressionBodyToBlockBody))
            {
                ArrowExpressionClauseSyntax arrowExpressionClause = GetArrowExpressionClause(semicolonToken);

                if (arrowExpressionClause?.IsMissing == false
                    && ExpandExpressionBodyAnalysis.IsFixable(arrowExpressionClause))
                {
                    context.RegisterRefactoring(
                        ConvertExpressionBodyToBlockBodyRefactoring.Title,
                        ct => ConvertExpressionBodyToBlockBodyRefactoring.RefactorAsync(context.Document, arrowExpressionClause, ct),
                        RefactoringDescriptors.ConvertExpressionBodyToBlockBody);
                }
            }
        }

        private static ArrowExpressionClauseSyntax GetArrowExpressionClause(SyntaxToken semicolonToken)
        {
            SyntaxNode parent = semicolonToken.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)parent).ExpressionBody;
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)parent).ExpressionBody;
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)parent).ExpressionBody;
                case SyntaxKind.OperatorDeclaration:
                    return ((OperatorDeclarationSyntax)parent).ExpressionBody;
                case SyntaxKind.ConversionOperatorDeclaration:
                    return ((ConversionOperatorDeclarationSyntax)parent).ExpressionBody;
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)parent).ExpressionBody;
                case SyntaxKind.DestructorDeclaration:
                    return ((DestructorDeclarationSyntax)parent).ExpressionBody;
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                    return ((AccessorDeclarationSyntax)parent).ExpressionBody;
                default:
                    return null;
            }
        }
    }
}
