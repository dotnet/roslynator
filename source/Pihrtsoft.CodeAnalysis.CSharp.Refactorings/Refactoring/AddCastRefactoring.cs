// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Simplification;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class AddCastRefactoring
    {
        public static void Refactor(
            CodeRefactoringContext context,
            ExpressionSyntax expression,
            ITypeSymbol typeSymbol)
        {
            context.RegisterRefactoring(
                $"Add cast to '{typeSymbol.ToDisplayString(TypeSyntaxRefactoring.SymbolDisplayFormat)}'",
                cancellationToken =>
                {
                    return RefactorAsync(
                        context.Document,
                        expression,
                        typeSymbol,
                        cancellationToken);
                });
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            ITypeSymbol typeSymbol,
            CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken);

            TypeSyntax type = TypeSyntaxRefactoring.CreateTypeSyntax(typeSymbol)
                .WithAdditionalAnnotations(Simplifier.Annotation);

            CastExpressionSyntax castExpression = SyntaxFactory.CastExpression(type, expression)
                .WithTriviaFrom(expression);

            root = root.ReplaceNode(expression, castExpression);

            return document.WithSyntaxRoot(root);
        }
    }
}

