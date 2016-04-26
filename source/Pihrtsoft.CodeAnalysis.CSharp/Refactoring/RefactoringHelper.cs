// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class RefactoringHelper
    {
        public static async Task<Document> AddBracesToStatementAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            BlockSyntax block = Block(statement)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(statement, block);

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> FormatStatementOnNextLineAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            StatementSyntax newStatement = statement
                .WithLeadingTrivia(statement.GetLeadingTrivia().Insert(0, SyntaxHelper.NewLine))
                .WithAdditionalAnnotations(Formatter.Annotation);

            if (statement.Parent.IsKind(SyntaxKind.Block))
            {
                var block = (BlockSyntax)statement.Parent;

                if (block.IsSingleline(includeExteriorTrivia: false))
                {
                    SyntaxTriviaList triviaList = block.CloseBraceToken.LeadingTrivia
                        .Add(SyntaxHelper.NewLine);

                    BlockSyntax newBlock = block
                        .WithCloseBraceToken(block.CloseBraceToken.WithLeadingTrivia(triviaList))
                        .WithStatements(block.Statements.Replace(statement, newStatement))
                        .WithAdditionalAnnotations(Formatter.Annotation);

                    root = root.ReplaceNode(block, newBlock);
                }
                else
                {
                    root = root.ReplaceNode(statement, newStatement);
                }
            }
            else
            {
                root = root.ReplaceNode(statement, newStatement);
            }

            return document.WithSyntaxRoot(root);
        }

        public static async Task<Document> ConvertAnonymousMethodToLambdaExpressionAsync(
            Document document,
            AnonymousMethodExpressionSyntax anonymousMethod,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            LambdaExpressionSyntax lambda = ParenthesizedLambdaExpression(
                anonymousMethod.AsyncKeyword,
                anonymousMethod.ParameterList,
                Token(SyntaxKind.EqualsGreaterThanToken),
                anonymousMethod.Block);

            lambda = SimplifyLambdaExpressionRefactoring.SimplifyLambdaExpression(lambda)
                .WithTriviaFrom(anonymousMethod)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(anonymousMethod, lambda);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
