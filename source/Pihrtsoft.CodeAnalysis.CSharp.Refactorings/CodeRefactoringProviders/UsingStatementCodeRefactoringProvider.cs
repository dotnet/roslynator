// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(UsingStatementCodeRefactoringProvider))]
    public class UsingStatementCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            UsingStatementSyntax usingStatement = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<UsingStatementSyntax>();

            if (usingStatement == null)
                return;

            if (context.Document.SupportsSemanticModel)
            {
                SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

                ExtractDeclarationFromUsingStatement(context, semanticModel, usingStatement);
            }
        }

        private static void ExtractDeclarationFromUsingStatement(
            CodeRefactoringContext context,
            SemanticModel semanticModel,
            UsingStatementSyntax usingStatement)
        {
            if (usingStatement.Declaration == null)
                return;

            if (usingStatement.Declaration.Type == null)
                return;

            if (usingStatement.Parent?.IsKind(SyntaxKind.Block) != true)
                return;

            if (!usingStatement.Declaration.Span.Contains(context.Span))
                return;

            ITypeSymbol typeSymbol = semanticModel
                .GetTypeInfo(usingStatement.Declaration.Type, context.CancellationToken)
                .ConvertedType;

            if (typeSymbol == null)
                return;

            context.RegisterRefactoring(
                "Extract declaration from using statement",
                cancellationToken => ExtractDeclarationFromUsingStatementAsync(context.Document, usingStatement, cancellationToken));
        }

        private static async Task<Document> ExtractDeclarationFromUsingStatementAsync(
            Document document,
            UsingStatementSyntax usingStatement,
            CancellationToken cancellationToken)
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
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(block, newBlock);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}