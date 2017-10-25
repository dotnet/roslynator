// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceIfStatementWithReturnStatementRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, IfStatementSyntax ifStatement)
        {
            StatementsInfo statementsInfo = SyntaxInfo.StatementsInfo(ifStatement);

            if (!statementsInfo.Success)
                return;

            ReturnStatementSyntax returnStatement = GetReturnStatement(ifStatement.Statement);
            LiteralExpressionSyntax booleanLiteral = GetBooleanLiteral(returnStatement);

            if (booleanLiteral == null)
                return;

            ReturnStatementSyntax returnStatement2 = null;
            LiteralExpressionSyntax booleanLiteral2 = null;

            TextSpan span = ifStatement.Span;
            ElseClauseSyntax elseClause = ifStatement.Else;

            if (elseClause != null)
            {
                returnStatement2 = GetReturnStatement(elseClause.Statement);
                booleanLiteral2 = GetBooleanLiteral(returnStatement2);

                if (booleanLiteral2 == null)
                    return;
            }
            else
            {
                SyntaxList<StatementSyntax> statements = statementsInfo.Statements;

                int index = statements.IndexOf(ifStatement);

                if (index == statements.Count - 1)
                    return;

                if (index > 0
                    && ((statements[index - 1] is IfStatementSyntax ifStatement2)
                    && ifStatement2.Else == null
                    && GetBooleanLiteral(ifStatement2.Statement) != null))
                {
                    return;
                }

                StatementSyntax nextStatement = statements[index + 1];

                if (nextStatement.Kind() != SyntaxKind.ReturnStatement)
                    return;

                returnStatement2 = (ReturnStatementSyntax)nextStatement;
                booleanLiteral2 = GetBooleanLiteral(returnStatement2);

                if (booleanLiteral2 == null)
                    return;

                span = TextSpan.FromBounds(ifStatement.SpanStart, returnStatement2.Span.End);
            }

            if (booleanLiteral.Kind() == booleanLiteral2.Kind())
                return;

            if (!statementsInfo.Node
                .DescendantTrivia(span)
                .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                return;
            }

            context.ReportDiagnostic(DiagnosticDescriptors.ReplaceIfStatementWithReturnStatement, ifStatement.IfKeyword);

            context.ReportToken(DiagnosticDescriptors.ReplaceIfStatementWithReturnStatementFadeOut, ifStatement.IfKeyword);
            context.ReportToken(DiagnosticDescriptors.ReplaceIfStatementWithReturnStatementFadeOut, ifStatement.OpenParenToken);
            context.ReportToken(DiagnosticDescriptors.ReplaceIfStatementWithReturnStatementFadeOut, ifStatement.CloseParenToken);
            context.ReportNode(DiagnosticDescriptors.ReplaceIfStatementWithReturnStatementFadeOut, ifStatement.Statement);

            if (elseClause != null)
            {
                context.ReportNode(DiagnosticDescriptors.ReplaceIfStatementWithReturnStatementFadeOut, elseClause);
            }
            else
            {
                context.ReportNode(DiagnosticDescriptors.ReplaceIfStatementWithReturnStatementFadeOut, returnStatement2);
            }
        }

        private static LiteralExpressionSyntax GetBooleanLiteral(ReturnStatementSyntax returnStatement)
        {
            if (returnStatement == null)
                return null;

            ExpressionSyntax expression = returnStatement.Expression;

            if (expression == null)
                return null;

            if (!expression.Kind().IsBooleanLiteralExpression())
                return null;

            return (LiteralExpressionSyntax)expression;
        }

        private static LiteralExpressionSyntax GetBooleanLiteral(StatementSyntax statement)
        {
            return GetBooleanLiteral(GetReturnStatement(statement));
        }

        private static ReturnStatementSyntax GetReturnStatement(StatementSyntax statement)
        {
            switch (statement)
            {
                case BlockSyntax block:
                    return block.Statements.SingleOrDefault(throwException: false) as ReturnStatementSyntax;
                case ReturnStatementSyntax returnStatement:
                    return returnStatement;
            }

            return null;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax returnExpression = ifStatement.Condition;

            if (GetBooleanLiteral(ifStatement.Statement).Kind() == SyntaxKind.FalseLiteralExpression)
            {
                SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                returnExpression = CSharpUtility.LogicallyNegate(returnExpression, semanticModel, cancellationToken);
            }

            ReturnStatementSyntax newReturnStatement = ReturnStatement(
                ReturnKeyword().WithTrailingTrivia(Space),
                returnExpression,
                SemicolonToken());

            if (ifStatement.Else != null)
            {
                newReturnStatement = newReturnStatement.WithTriviaFrom(ifStatement);

                return await document.ReplaceNodeAsync(ifStatement, newReturnStatement, cancellationToken).ConfigureAwait(false);
            }

            StatementsInfo statementsInfo = SyntaxInfo.StatementsInfo(ifStatement);

            SyntaxList<StatementSyntax> statements = statementsInfo.Statements;

            int index = statements.IndexOf(ifStatement);

            var returnStatement = (ReturnStatementSyntax)statements[index + 1];

            newReturnStatement = newReturnStatement
                .WithLeadingTrivia(ifStatement.GetLeadingTrivia())
                .WithTrailingTrivia(returnStatement.GetTrailingTrivia());

            SyntaxList<StatementSyntax> newStatements = statements
                .RemoveAt(index)
                .ReplaceAt(index, newReturnStatement);

            return await document.ReplaceStatementsAsync(statementsInfo, newStatements, cancellationToken).ConfigureAwait(false);
        }
    }
}
