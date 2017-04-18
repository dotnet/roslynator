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
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceIfStatementWithReturnStatementRefactoring
    {
        private static DiagnosticDescriptor FadeOutDescriptor
        {
            get { return DiagnosticDescriptors.ReplaceIfStatementWithReturnStatementFadeOut; }
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, IfStatementSyntax ifStatement)
        {
            SyntaxNode parent = ifStatement.Parent;

            if (parent?.IsKind(SyntaxKind.Block) == true)
            {
                ReturnStatementSyntax returnStatement = GetReturnStatement(ifStatement.Statement);
                LiteralExpressionSyntax booleanLiteral = GetBooleanLiteral(returnStatement);

                if (booleanLiteral != null)
                {
                    ReturnStatementSyntax returnStatement2 = null;
                    LiteralExpressionSyntax booleanLiteral2 = null;
                    TextSpan span = ifStatement.Span;
                    ElseClauseSyntax @else = ifStatement.Else;

                    if (@else != null)
                    {
                        returnStatement2 = GetReturnStatement(@else.Statement);
                        booleanLiteral2 = GetBooleanLiteral(returnStatement2);
                    }
                    else
                    {
                        var block = (BlockSyntax)parent;
                        SyntaxList<StatementSyntax> statements = block.Statements;

                        int index = statements.IndexOf(ifStatement);

                        if (index < statements.Count - 1
                            && (index == 0 || !IsIfStatementWithReturnStatement(statements[index - 1])))
                        {
                            StatementSyntax nextStatement = statements[index + 1];

                            if (nextStatement.IsKind(SyntaxKind.ReturnStatement))
                            {
                                returnStatement2 = (ReturnStatementSyntax)nextStatement;
                                booleanLiteral2 = GetBooleanLiteral(returnStatement2);

                                if (booleanLiteral2 != null)
                                    span = TextSpan.FromBounds(ifStatement.SpanStart, returnStatement2.Span.End);
                            }
                        }
                    }

                    if (booleanLiteral2 != null
                        && IsOppositeBooleanLiteral(booleanLiteral, booleanLiteral2)
                        && parent
                            .DescendantTrivia(span)
                            .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.ReplaceIfStatementWithReturnStatement,
                            Location.Create(context.Node.SyntaxTree, span));

                        context.ReportToken(FadeOutDescriptor, ifStatement.IfKeyword);
                        context.ReportToken(FadeOutDescriptor, ifStatement.OpenParenToken);
                        context.ReportToken(FadeOutDescriptor, ifStatement.CloseParenToken);
                        context.ReportNode(FadeOutDescriptor, ifStatement.Statement);

                        if (ifStatement.Else != null)
                        {
                            context.ReportNode(FadeOutDescriptor, @else);
                        }
                        else
                        {
                            context.ReportNode(FadeOutDescriptor, returnStatement2);
                        }
                    }
                }
            }
        }

        private static LiteralExpressionSyntax GetBooleanLiteral(ReturnStatementSyntax returnStatement)
        {
            if (returnStatement != null)
            {
                ExpressionSyntax expression = returnStatement.Expression;

                if (expression?.IsBooleanLiteralExpression() == true)
                    return (LiteralExpressionSyntax)expression;
            }

            return null;
        }

        private static LiteralExpressionSyntax GetBooleanLiteral(StatementSyntax statement)
        {
            return GetBooleanLiteral(GetReturnStatement(statement));
        }

        private static ReturnStatementSyntax GetReturnStatement(StatementSyntax statement)
        {
            switch (statement?.Kind())
            {
                case SyntaxKind.Block:
                    {
                        StatementSyntax statement2 = ((BlockSyntax)statement).SingleStatementOrDefault();

                        if (statement2?.IsKind(SyntaxKind.ReturnStatement) == true)
                            return (ReturnStatementSyntax)statement2;

                        break;
                    }
                case SyntaxKind.ReturnStatement:
                    {
                        return (ReturnStatementSyntax)statement;
                    }
            }

            return null;
        }

        private static bool IsOppositeBooleanLiteral(LiteralExpressionSyntax literal1, LiteralExpressionSyntax literal2)
        {
            if (literal1.IsKind(SyntaxKind.TrueLiteralExpression))
            {
                return literal2.IsKind(SyntaxKind.FalseLiteralExpression);
            }
            else
            {
                return literal2.IsKind(SyntaxKind.TrueLiteralExpression);
            }
        }

        private static bool IsIfStatementWithReturnStatement(StatementSyntax statement)
        {
            if (statement.IsKind(SyntaxKind.IfStatement))
            {
                var ifStatement = (IfStatementSyntax)statement;

                if (ifStatement.Else == null)
                    return GetBooleanLiteral(ifStatement.Statement) != null;
            }

            return false;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken)
        {
            ReturnStatementSyntax newReturnStatement = CreateReturnStatement(ifStatement);

            if (ifStatement.Else != null)
            {
                newReturnStatement = newReturnStatement.WithTriviaFrom(ifStatement);

                return document.ReplaceNodeAsync(ifStatement, newReturnStatement, cancellationToken);
            }
            else
            {
                var block = (BlockSyntax)ifStatement.Parent;
                SyntaxList<StatementSyntax> statements = block.Statements;

                int index = statements.IndexOf(ifStatement);

                var returnStatement = (ReturnStatementSyntax)statements[index + 1];

                newReturnStatement = newReturnStatement
                    .WithLeadingTrivia(ifStatement.GetLeadingTrivia())
                    .WithTrailingTrivia(returnStatement.GetTrailingTrivia());

                SyntaxList<StatementSyntax> newStatements = statements
                    .RemoveAt(index)
                    .ReplaceAt(index, newReturnStatement);

                return document.ReplaceNodeAsync(block, block.WithStatements(newStatements), cancellationToken);
            }
        }

        private static ReturnStatementSyntax CreateReturnStatement(IfStatementSyntax ifStatement)
        {
            ExpressionSyntax expression = ifStatement.Condition;

            if (GetBooleanLiteral(ifStatement.Statement).IsKind(SyntaxKind.FalseLiteralExpression))
                expression = Negator.LogicallyNegate(expression);

            return ReturnStatement(
                ReturnKeyword().WithTrailingTrivia(Space),
                expression,
                SemicolonToken());
        }
    }
}
