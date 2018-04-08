// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ExpandLambdaExpressionBodyRefactoring
    {
        public static bool CanRefactor(RefactoringContext context, LambdaExpressionSyntax lambda)
        {
            CSharpSyntaxNode body = lambda.Body;

            return body is ExpressionSyntax
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(body);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            LambdaExpressionSyntax lambda,
            ExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var methodSymbol = (IMethodSymbol)semanticModel.GetSymbol(lambda, cancellationToken);

            StatementSyntax statement;
            if (ShouldCreateExpressionStatement(expression, methodSymbol, semanticModel))
            {
                statement = ExpressionStatement(expression);
            }
            else
            {
                statement = ReturnStatement(expression);
            }

            BlockSyntax block = Block(statement);

            block = block
                .WithCloseBraceToken(
                    block.CloseBraceToken
                        .WithLeadingTrivia(TriviaList(NewLine())));

            LambdaExpressionSyntax newLambda = lambda;

            switch (lambda.Kind())
            {
                case SyntaxKind.SimpleLambdaExpression:
                    {
                        newLambda = ((SimpleLambdaExpressionSyntax)lambda).WithBody(block);
                        break;
                    }
                case SyntaxKind.ParenthesizedLambdaExpression:
                    {
                        newLambda = ((ParenthesizedLambdaExpressionSyntax)lambda).WithBody(block);
                        break;
                    }
            }

            newLambda = newLambda.WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(lambda, newLambda, cancellationToken).ConfigureAwait(false);
        }

        private static bool ShouldCreateExpressionStatement(ExpressionSyntax expression, IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            if (methodSymbol.ReturnsVoid)
                return true;

            if (expression.IsKind(SyntaxKind.AwaitExpression))
            {
                ITypeSymbol returnType = methodSymbol.ReturnType;

                if (returnType?.Kind == SymbolKind.NamedType
                    && !((INamedTypeSymbol)returnType).ConstructedFrom.EqualsOrInheritsFrom(semanticModel.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task_T)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
