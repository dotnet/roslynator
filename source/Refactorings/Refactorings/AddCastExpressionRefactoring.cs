// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
{
    internal static class AddCastExpressionRefactoring
    {
        public static void RegisterRefactoring(
            RefactoringContext context,
            ExpressionSyntax expression,
            ITypeSymbol newType,
            SemanticModel semanticModel)
        {
            if (!newType.IsErrorType()
                && !newType.IsVoid())
            {
                Conversion conversion = semanticModel.ClassifyConversion(
                    expression,
                    newType,
                    isExplicitInSource: false);

                if (conversion.IsExplicit)
                {
                    context.RegisterRefactoring(
                        $"Cast to '{newType.ToDisplayString(TypeSyntaxRefactoring.SymbolDisplayFormat)}'",
                        cancellationToken =>
                        {
                            return RefactorAsync(
                                context.Document,
                                expression,
                                newType,
                                cancellationToken);
                        });
                }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            ITypeSymbol typeSymbol,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            TypeSyntax type = TypeSyntaxRefactoring.CreateTypeSyntax(typeSymbol)
                .WithSimplifierAnnotation();

            CastExpressionSyntax castExpression = SyntaxFactory.CastExpression(type, expression)
                .WithTriviaFrom(expression);

            root = root.ReplaceNode(expression, castExpression);

            return document.WithSyntaxRoot(root);
        }
    }
}

