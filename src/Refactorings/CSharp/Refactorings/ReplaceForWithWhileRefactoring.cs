// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceForWithWhileRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            ForStatementSyntax forStatement,
            CancellationToken cancellationToken)
        {
            var statements = new List<StatementSyntax>();

            VariableDeclarationSyntax declaration = forStatement.Declaration;

            if (declaration != null)
            {
                statements.Add(LocalDeclarationStatement(declaration));
            }
            else
            {
                foreach (ExpressionSyntax initializer in forStatement.Initializers)
                    statements.Add(ExpressionStatement(initializer));
            }

            StatementSyntax statement = forStatement.Statement ?? Block();

            SeparatedSyntaxList<ExpressionSyntax> incrementors = forStatement.Incrementors;

            if (incrementors.Any())
            {
                if (!statement.IsKind(SyntaxKind.Block))
                    statement = Block(statement);

                ExpressionStatementSyntax[] incrementorStatements = incrementors
                    .Select(f => ExpressionStatement(f).WithFormatterAnnotation())
                    .ToArray();

                var rewriter = new InsertIncrementorsBeforeContinueRewriter(incrementorStatements);

                statement = (StatementSyntax)rewriter.Visit(statement);

                statement = ((BlockSyntax)statement).AddStatements(incrementorStatements);
            }

            statements.Add(WhileStatement(forStatement.Condition ?? TrueLiteralExpression(), statement));

            statements[0] = statements[0].WithLeadingTrivia(forStatement.GetLeadingTrivia());

            if (forStatement.IsEmbedded())
            {
                return document.ReplaceNodeAsync(forStatement, Block(statements), cancellationToken);
            }
            else
            {
                return document.ReplaceNodeAsync(forStatement, statements, cancellationToken);
            }
        }

        private class InsertIncrementorsBeforeContinueRewriter : CSharpSyntaxRewriter
        {
            private readonly ExpressionStatementSyntax[] _incrementorStatements;

            public InsertIncrementorsBeforeContinueRewriter(ExpressionStatementSyntax[] incrementorStatements)
            {
                _incrementorStatements = incrementorStatements;
            }

            public override SyntaxNode VisitContinueStatement(ContinueStatementSyntax node)
            {
                if (node.IsEmbedded())
                    return Block(_incrementorStatements).AddStatements(node);

                return base.VisitContinueStatement(node);
            }

            public override SyntaxNode VisitBlock(BlockSyntax node)
            {
                node = (BlockSyntax)base.VisitBlock(node);

                foreach (StatementSyntax statement in node.Statements)
                {
                    if (statement.IsKind(SyntaxKind.ContinueStatement))
                        return node.InsertNodesBefore(statement, _incrementorStatements);
                }

                return node;
            }

            public override SyntaxNode VisitForStatement(ForStatementSyntax node)
            {
                return node;
            }

            public override SyntaxNode VisitForEachStatement(ForEachStatementSyntax node)
            {
                return node;
            }

            public override SyntaxNode VisitForEachVariableStatement(ForEachVariableStatementSyntax node)
            {
                return node;
            }

            public override SyntaxNode VisitWhileStatement(WhileStatementSyntax node)
            {
                return node;
            }

            public override SyntaxNode VisitDoStatement(DoStatementSyntax node)
            {
                return node;
            }
        }
    }
}