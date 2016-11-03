// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CreateConditionFromBooleanExpressionRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, ExpressionSyntax expression)
        {
            if (context.Span.IsBetweenSpans(expression)
                && !expression.IsKind(
                    SyntaxKind.TrueLiteralExpression,
                    SyntaxKind.FalseLiteralExpression,
                    SyntaxKind.ConditionalExpression))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                ITypeSymbol expressionSymbol = semanticModel
                    .GetTypeInfo(expression, context.CancellationToken)
                    .ConvertedType;

                if (expressionSymbol?.IsBoolean() == true)
                {
                    context.RegisterRefactoring(
                        $"Create condition from '{expression.ToString()}'",
                        cancellationToken => RefactorAsync(context.Document, expression, cancellationToken));
                }
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            IfStatementSyntax newNode = IfStatement(expression, Block())
                .WithTriviaFrom(expression.Parent)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = root.ReplaceNode(expression.Parent, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
