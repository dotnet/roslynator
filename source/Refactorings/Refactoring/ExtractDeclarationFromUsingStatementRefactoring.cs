// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ExtractDeclarationFromUsingStatementRefactoring
    {
        public static async Task<bool> CanRefactorAsync(
            RefactoringContext context,
            UsingStatementSyntax usingStatement)
        {
            if (context.SupportsSemanticModel
                && usingStatement.Declaration?.Type != null
                && usingStatement.Parent?.IsKind(SyntaxKind.Block) == true
                && usingStatement.Declaration.Span.Contains(context.Span))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync();

                ITypeSymbol typeSymbol = semanticModel
                    .GetTypeInfo(usingStatement.Declaration.Type, context.CancellationToken)
                    .ConvertedType;

                if (typeSymbol != null)
                    return true;
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            UsingStatementSyntax usingStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            var block = (BlockSyntax)usingStatement.Parent;

            int index = block.Statements.IndexOf(usingStatement);

            BlockSyntax newBlock = block.WithStatements(block.Statements.RemoveAt(index));

            var statements = new List<StatementSyntax>();

            statements.Add(SyntaxFactory.LocalDeclarationStatement(usingStatement.Declaration));
            statements.AddRange(usingStatement.GetStatements());

            if (statements.Count > 0)
            {
                statements[0] = statements[0]
                    .WithLeadingTrivia(usingStatement.GetLeadingTrivia());

                statements[statements.Count - 1] = statements[statements.Count - 1]
                    .WithTrailingTrivia(usingStatement.GetTrailingTrivia());
            }

            newBlock = newBlock
                .WithStatements(newBlock.Statements.InsertRange(index, statements))
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(block, newBlock);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
