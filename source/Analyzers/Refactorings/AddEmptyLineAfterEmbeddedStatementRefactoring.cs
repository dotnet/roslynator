// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddEmptyLineAfterEmbeddedStatementRefactoring
    {
        internal static void Analyze(SyntaxNodeAnalysisContext context, IfStatementSyntax ifStatement)
        {
            Analyze(context, ifStatement, ifStatement.CloseParenToken, ifStatement.Statement);
        }

        internal static void Analyze(SyntaxNodeAnalysisContext context, CommonForEachStatementSyntax forEachStatement)
        {
            Analyze(context, forEachStatement, forEachStatement.CloseParenToken, forEachStatement.Statement);
        }

        internal static void Analyze(SyntaxNodeAnalysisContext context, ForStatementSyntax forStatement)
        {
            Analyze(context, forStatement, forStatement.CloseParenToken, forStatement.Statement);
        }

        internal static void Analyze(SyntaxNodeAnalysisContext context, UsingStatementSyntax usingStatement)
        {
            Analyze(context, usingStatement, usingStatement.CloseParenToken, usingStatement.Statement);
        }

        internal static void Analyze(SyntaxNodeAnalysisContext context, WhileStatementSyntax whileStatement)
        {
            Analyze(context, whileStatement, whileStatement.CloseParenToken, whileStatement.Statement);
        }

        internal static void Analyze(SyntaxNodeAnalysisContext context, LockStatementSyntax lockStatement)
        {
            Analyze(context, lockStatement, lockStatement.CloseParenToken, lockStatement.Statement);
        }

        internal static void Analyze(SyntaxNodeAnalysisContext context, FixedStatementSyntax fixedStatement)
        {
            Analyze(context, fixedStatement, fixedStatement.CloseParenToken, fixedStatement.Statement);
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, ElseClauseSyntax elseClause)
        {
            StatementSyntax statement = elseClause.Statement;
            SyntaxToken elseKeyword = elseClause.ElseKeyword;

            if (statement?.IsKind(SyntaxKind.Block, SyntaxKind.IfStatement) == false
                && context.SyntaxTree().IsMultiLineSpan(TextSpan.FromBounds(elseKeyword.SpanStart, statement.SpanStart)))
            {
                IfStatementSyntax topmostIf = elseClause.GetTopmostIf();

                if (topmostIf != null)
                    Analyze(context, topmostIf, elseKeyword, statement);
            }
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            StatementSyntax containingStatement,
            SyntaxToken token,
            StatementSyntax statement)
        {
            if (!token.IsKind(SyntaxKind.None)
                && !token.IsMissing
                && statement?.IsKind(SyntaxKind.Block, SyntaxKind.EmptyStatement) == false
                && context.SyntaxTree().IsMultiLineSpan(TextSpan.FromBounds(token.SpanStart, statement.SpanStart)))
            {
                SyntaxNode parent = containingStatement.Parent;

                if (parent?.IsKind(SyntaxKind.Block) == true)
                {
                    var block = (BlockSyntax)parent;

                    SyntaxList<StatementSyntax> statements = block.Statements;

                    int index = statements.IndexOf(containingStatement);

                    if (index < statements.Count - 1
                        && context
                            .SyntaxTree()
                            .GetLineCount(TextSpan.FromBounds(statement.Span.End, statements[index + 1].SpanStart)) <= 2)
                    {
                        SyntaxTrivia trivia = statement
                            .GetTrailingTrivia()
                            .FirstOrDefault(f => f.IsEndOfLineTrivia());

                        if (trivia.IsEndOfLineTrivia())
                        {
                            context.ReportDiagnostic(
                                DiagnosticDescriptors.AddEmptyLineAfterEmbeddedStatement,
                                trivia);
                        }
                    }
                }
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken)
        {
            StatementSyntax newNode = statement
                .AppendToTrailingTrivia(CSharpFactory.NewLine())
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(statement, newNode, cancellationToken);
        }
    }
}
