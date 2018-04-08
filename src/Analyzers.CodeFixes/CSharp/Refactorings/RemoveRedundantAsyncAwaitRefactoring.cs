// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    //XPERF:
    internal static partial class RemoveRedundantAsyncAwaitRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            SyntaxNode node,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            switch (node.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    return await RefactorAsync(document, (MethodDeclarationSyntax)node, semanticModel, cancellationToken).ConfigureAwait(false);
                case SyntaxKind.LocalFunctionStatement:
                    return await RefactorAsync(document, (LocalFunctionStatementSyntax)node, semanticModel, cancellationToken).ConfigureAwait(false);
                case SyntaxKind.SimpleLambdaExpression:
                    return await RefactorAsync(document, (SimpleLambdaExpressionSyntax)node, semanticModel, cancellationToken).ConfigureAwait(false);
                case SyntaxKind.ParenthesizedLambdaExpression:
                    return await RefactorAsync(document, (ParenthesizedLambdaExpressionSyntax)node, semanticModel, cancellationToken).ConfigureAwait(false);
                case SyntaxKind.AnonymousMethodExpression:
                    return await RefactorAsync(document, (AnonymousMethodExpressionSyntax)node, semanticModel, cancellationToken).ConfigureAwait(false);
            }

            Debug.Fail(node.Kind().ToString());

            return document;
        }

        private static Task<Document> RefactorAsync(Document document, MethodDeclarationSyntax methodDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            MethodDeclarationSyntax newNode = AwaitRemover.Visit(methodDeclaration, semanticModel, cancellationToken);

            newNode = newNode.RemoveModifier(SyntaxKind.AsyncKeyword);

            return document.ReplaceNodeAsync(methodDeclaration, newNode, cancellationToken);
        }

        private static Task<Document> RefactorAsync(Document document, LocalFunctionStatementSyntax localFunction, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            LocalFunctionStatementSyntax newNode = AwaitRemover.Visit(localFunction, semanticModel, cancellationToken);

            newNode = newNode.RemoveModifier(SyntaxKind.AsyncKeyword);

            return document.ReplaceNodeAsync(localFunction, newNode, cancellationToken);
        }

        private static Task<Document> RefactorAsync(Document document, SimpleLambdaExpressionSyntax lambda, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            SimpleLambdaExpressionSyntax newNode = AwaitRemover.Visit(lambda, semanticModel, cancellationToken);

            newNode = newNode.WithAsyncKeyword(GetMissingAsyncKeyword(lambda.AsyncKeyword));

            return document.ReplaceNodeAsync(lambda, newNode, cancellationToken);
        }

        private static Task<Document> RefactorAsync(Document document, ParenthesizedLambdaExpressionSyntax lambda, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            ParenthesizedLambdaExpressionSyntax newNode = AwaitRemover.Visit(lambda, semanticModel, cancellationToken);

            newNode = newNode.WithAsyncKeyword(GetMissingAsyncKeyword(lambda.AsyncKeyword));

            return document.ReplaceNodeAsync(lambda, newNode, cancellationToken);
        }

        private static Task<Document> RefactorAsync(Document document, AnonymousMethodExpressionSyntax anonymousMethod, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            AnonymousMethodExpressionSyntax newNode = AwaitRemover.Visit(anonymousMethod, semanticModel, cancellationToken);

            newNode = newNode.WithAsyncKeyword(GetMissingAsyncKeyword(anonymousMethod.AsyncKeyword));

            return document.ReplaceNodeAsync(anonymousMethod, newNode, cancellationToken);
        }

        private static SyntaxToken GetMissingAsyncKeyword(SyntaxToken asyncKeyword)
        {
            if (asyncKeyword.TrailingTrivia.All(f => f.IsWhitespaceTrivia()))
            {
                return MissingToken(SyntaxKind.AsyncKeyword).WithLeadingTrivia(asyncKeyword.LeadingTrivia);
            }
            else
            {
                return MissingToken(SyntaxKind.AsyncKeyword).WithTriviaFrom(asyncKeyword);
            }
        }
    }
}
