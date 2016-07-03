// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class FormatStatementOnNextLineRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            StatementSyntax newStatement = statement
                .WithLeadingTrivia(statement.GetLeadingTrivia().Insert(0, CSharpFactory.NewLine))
                .WithAdditionalAnnotations(Formatter.Annotation);

            if (statement.Parent.IsKind(SyntaxKind.Block))
            {
                var block = (BlockSyntax)statement.Parent;

                if (block.IsSingleline(includeExteriorTrivia: false))
                {
                    SyntaxTriviaList triviaList = block.CloseBraceToken.LeadingTrivia
                        .Add(CSharpFactory.NewLine);

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
    }
}
