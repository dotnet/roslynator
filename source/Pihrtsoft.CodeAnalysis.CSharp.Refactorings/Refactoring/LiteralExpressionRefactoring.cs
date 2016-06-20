// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class LiteralExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, LiteralExpressionSyntax literalExpression)
        {
            switch (literalExpression.Kind())
            {
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                    {
                        context.RegisterRefactoring(
                            "Negate boolean literal",
                            cancellationToken => NegateBooleanLiteralTypeAsync(context.Document, literalExpression, cancellationToken));

                        break;
                    }
            }
        }

        private static async Task<Document> NegateBooleanLiteralTypeAsync(
            Document document,
            LiteralExpressionSyntax literalExpression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            LiteralExpressionSyntax newNode = GetNewNode(literalExpression)
                .WithTriviaFrom(literalExpression);

            SyntaxNode newRoot = oldRoot.ReplaceNode(literalExpression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static LiteralExpressionSyntax GetNewNode(LiteralExpressionSyntax literalExpression)
        {
            if (literalExpression.IsKind(SyntaxKind.TrueLiteralExpression))
                return SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression);
            else
                return SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression);
        }
    }
}