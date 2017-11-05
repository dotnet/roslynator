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
using System.Collections.Generic;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantAssignmentRefactoring
    {
        internal static void AnalyzeSimpleAssignment(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.ContainsDiagnostics)
                return;

            if (context.Node.SpanOrTrailingTriviaContainsDirectives())
                return;

            var assignment = (AssignmentExpressionSyntax)context.Node;

            SimpleAssignmentStatementInfo assignmentInfo = SyntaxInfo.SimpleAssignmentStatementInfo(assignment);

            if (!assignmentInfo.Success)
                return;

            if (!(assignmentInfo.Left is IdentifierNameSyntax identifierName))
                return;

            StatementSyntax nextStatement = assignmentInfo.ExpressionStatement.NextStatementOrDefault();

            if (nextStatement == null)
                return;

            if (nextStatement.SpanOrLeadingTriviaContainsDirectives())
                return;

            if (!IsFixableNextStatement(nextStatement, identifierName))
                return;

            ISymbol symbol = context.SemanticModel.GetSymbol(identifierName, context.CancellationToken);

            if (symbol == null)
                return;

            if (!IsFixableSymbol(symbol))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.RemoveRedundantAssignment, assignment);
        }

        private static bool IsFixableSymbol(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.Local:
                    return true;
                case SymbolKind.Parameter:
                    return ((IParameterSymbol)symbol).RefKind == RefKind.None;
            }

            return false;
        }

        private static bool IsFixableNextStatement(StatementSyntax statement, IdentifierNameSyntax identifierName)
        {
            switch (statement.Kind())
            {
                case SyntaxKind.ExpressionStatement:
                    {
                        SimpleAssignmentStatementInfo assignmentInfo = SyntaxInfo.SimpleAssignmentStatementInfo((ExpressionStatementSyntax)statement);

                        if (!assignmentInfo.Success)
                            return false;

                        if (!(assignmentInfo.Left is IdentifierNameSyntax identifierName2))
                            return false;

                        return identifierName.Identifier.ValueText == identifierName2.Identifier.ValueText;
                    }
                case SyntaxKind.ReturnStatement:
                    {
                        var returnStatement = (ReturnStatementSyntax)statement;

                        if (!(returnStatement.Expression?.WalkDownParentheses() is IdentifierNameSyntax identifierName2))
                            return false;

                        return identifierName.Identifier.ValueText == identifierName2.Identifier.ValueText;
                    }
            }

            return false;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            AssignmentExpressionSyntax assignmentExpression,
            CancellationToken cancellationToken)
        {
            var statement = (StatementSyntax)assignmentExpression.Parent;

            StatementsInfo statementsInfo = SyntaxInfo.StatementsInfo(statement);

            SyntaxList<StatementSyntax> statements = statementsInfo.Statements;

            StatementSyntax nextStatement = statement.NextStatementOrDefault();

            int index = statements.IndexOf(statement);

            statements = statements.RemoveAt(index);

            if (statement is ReturnStatementSyntax returnStatement)
                nextStatement = returnStatement.WithExpression(assignmentExpression.Left.WithTriviaFrom(returnStatement.Expression));

            IEnumerable<SyntaxTrivia> trivia = statementsInfo
                .Node
                .DescendantTrivia(TextSpan.FromBounds(statement.SpanStart, nextStatement.SpanStart))
                .Where(f => !f.IsWhitespaceOrEndOfLineTrivia());

            trivia = statement
                .GetLeadingTrivia()
                .Concat(trivia);

            nextStatement = nextStatement.WithLeadingTrivia(trivia);

            statements = statements.ReplaceAt(index, nextStatement);

            return document.ReplaceStatementsAsync(statementsInfo, statements, cancellationToken);
        }
    }
}
