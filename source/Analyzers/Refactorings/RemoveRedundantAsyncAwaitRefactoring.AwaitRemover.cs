// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static partial class RemoveRedundantAsyncAwaitRefactoring
    {
        private class AwaitRemover : CSharpSyntaxRewriter
        {
            public AwaitRemover(SemanticModel semanticModel, CancellationToken cancellationToken)
            {
                SemanticModel = semanticModel;
                CancellationToken = cancellationToken;
            }

            public SemanticModel SemanticModel { get; }

            public CancellationToken CancellationToken { get; }

            public static SimpleLambdaExpressionSyntax Visit(SimpleLambdaExpressionSyntax lambda, SemanticModel semanticModel, CancellationToken cancellationToken)
            {
                return lambda.WithBody(Visit(lambda.Body, semanticModel, cancellationToken));
            }

            public static ParenthesizedLambdaExpressionSyntax Visit(ParenthesizedLambdaExpressionSyntax lambda, SemanticModel semanticModel, CancellationToken cancellationToken)
            {
                return lambda.WithBody(Visit(lambda.Body, semanticModel, cancellationToken));
            }

            public static AnonymousMethodExpressionSyntax Visit(AnonymousMethodExpressionSyntax anonymousMethod, SemanticModel semanticModel, CancellationToken cancellationToken)
            {
                return anonymousMethod.WithBody(Visit(anonymousMethod.Body, semanticModel, cancellationToken));
            }

            public static LocalFunctionStatementSyntax Visit(LocalFunctionStatementSyntax localFunction, SemanticModel semanticModel, CancellationToken cancellationToken)
            {
                BlockSyntax body = localFunction.Body;

                if (body != null)
                {
                    return localFunction.WithBody(Visit(body, semanticModel, cancellationToken));
                }
                else
                {
                    ArrowExpressionClauseSyntax expressionBody = localFunction.ExpressionBody;

                    if (expressionBody != null)
                        return localFunction.WithExpressionBody(Visit(expressionBody, semanticModel, cancellationToken));
                }

                return localFunction;
            }

            public static TNode Visit<TNode>(TNode node, SemanticModel semanticModel, CancellationToken cancellationToken) where TNode : SyntaxNode
            {
                var remover = new AwaitRemover(semanticModel, cancellationToken);

                return (TNode)remover.Visit(node);
            }

            public override SyntaxNode VisitAwaitExpression(AwaitExpressionSyntax node)
            {
                node = (AwaitExpressionSyntax)base.VisitAwaitExpression(node);

                return ExtractExpressionFromAwait(node, SemanticModel, CancellationToken);
            }

            public override SyntaxNode VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
            {
                return node;
            }

            public override SyntaxNode VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
            {
                return node;
            }

            public override SyntaxNode VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
            {
                return node;
            }

            public override SyntaxNode VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
            {
                return node;
            }

            private static ExpressionSyntax ExtractExpressionFromAwait(AwaitExpressionSyntax awaitExpression, SemanticModel semanticModel, CancellationToken cancellationToken)
            {
                ExpressionSyntax expression = awaitExpression.Expression;

                if (semanticModel.GetTypeSymbol(expression, cancellationToken) is INamedTypeSymbol typeSymbol)
                {
                    if (typeSymbol.Equals(semanticModel.GetTypeByMetadataName(MetadataNames.System_Runtime_CompilerServices_ConfiguredTaskAwaitable))
                        || typeSymbol.ConstructedFrom.Equals(semanticModel.GetTypeByMetadataName(MetadataNames.System_Runtime_CompilerServices_ConfiguredTaskAwaitable_T)))
                    {
                        if (expression is InvocationExpressionSyntax invocation)
                        {
                            var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;

                            if (string.Equals(memberAccess?.Name?.Identifier.ValueText, "ConfigureAwait", StringComparison.Ordinal))
                                expression = memberAccess.Expression;
                        }
                    }
                }

                return expression.WithTriviaFrom(awaitExpression);
            }
        }
    }
}
