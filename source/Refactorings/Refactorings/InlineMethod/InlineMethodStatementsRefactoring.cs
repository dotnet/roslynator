// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings.InlineMethod
{
    internal class InlineMethodStatementsRefactoring : AbstractInlineMethodRefactoring
    {
        public InlineMethodStatementsRefactoring(
            Document document,
            InvocationExpressionSyntax invocationExpression,
            INamedTypeSymbol invocationEnclosingType,
            IMethodSymbol methodSymbol,
            MethodDeclarationSyntax methodDeclaration,
            ImmutableArray<ParameterInfo> parameterInfos,
            SemanticModel invocationSemanticModel,
            SemanticModel declarationSemanticModel,
            CancellationToken cancellationToken) : base(document, invocationExpression, invocationEnclosingType, methodSymbol, methodDeclaration, parameterInfos, invocationSemanticModel, declarationSemanticModel, cancellationToken)
        {
        }

        public Task<Document> InlineMethodAsync(
            ExpressionStatementSyntax expressionStatement,
            SyntaxList<StatementSyntax> statements)
        {
            int count = statements.Count;

            StatementSyntax[] newStatements = RewriteStatements(statements);

            newStatements[0] = newStatements[0].WithLeadingTrivia(expressionStatement.GetLeadingTrivia());
            newStatements[count - 1] = newStatements[count - 1].WithTrailingTrivia(expressionStatement.GetTrailingTrivia());

            StatementsInfo statementsInfo = SyntaxInfo.StatementsInfo(expressionStatement);

            if (statementsInfo.Success)
            {
                StatementsInfo newInfo = statementsInfo.WithStatements(statementsInfo.Statements.ReplaceRange(expressionStatement, newStatements));

                return Document.ReplaceNodeAsync(statementsInfo.Node, newInfo.Node, CancellationToken);
            }
            else
            {
                return Document.ReplaceNodeAsync(expressionStatement, Block(newStatements), CancellationToken);
            }
        }

        public async Task<Solution> InlineAndRemoveMethodAsync(
            ExpressionStatementSyntax expressionStatement,
            SyntaxList<StatementSyntax> statements)
        {
            if (expressionStatement.SyntaxTree == MethodDeclaration.SyntaxTree)
            {
                DocumentEditor editor = await DocumentEditor.CreateAsync(Document, CancellationToken).ConfigureAwait(false);

                StatementSyntax[] newStatements = RewriteStatements(statements);

                int count = statements.Count;

                newStatements[0] = newStatements[0].WithLeadingTrivia(expressionStatement.GetLeadingTrivia());
                newStatements[count - 1] = newStatements[count - 1].WithTrailingTrivia(expressionStatement.GetTrailingTrivia());

                StatementsInfo statementsInfo = SyntaxInfo.StatementsInfo(expressionStatement);

                if (statementsInfo.Success)
                {
                    StatementsInfo newStatementsInfo = statementsInfo.WithStatements(statementsInfo.Statements.ReplaceRange(expressionStatement, newStatements));

                    editor.ReplaceNode(statementsInfo.Node, newStatementsInfo.Node);
                }
                else
                {
                    editor.ReplaceNode(expressionStatement, Block(newStatements));
                }

                editor.RemoveNode(MethodDeclaration);

                return editor.GetChangedDocument().Solution();
            }
            else
            {
                Document newDocument = await InlineMethodAsync(expressionStatement, statements).ConfigureAwait(false);

                DocumentId documentId = Document.Solution().GetDocumentId(MethodDeclaration.SyntaxTree);

                newDocument = await newDocument.Solution().GetDocument(documentId).RemoveMemberAsync(MethodDeclaration, CancellationToken).ConfigureAwait(false);

                return newDocument.Solution();
            }
        }

        private StatementSyntax[] RewriteStatements(SyntaxList<StatementSyntax> statements)
        {
            var newStatements = new StatementSyntax[statements.Count];

            for (int i = 0; i < statements.Count; i++)
                newStatements[i] = RewriteNode(statements[i]).WithFormatterAnnotation();

            return newStatements;
        }
    }
}
