// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Analyzers
{
    internal static class SimplifyIfStatementToReturnStatementAnalyzer
    {
        private static readonly DiagnosticDescriptor _fadeOutDescriptor = DiagnosticDescriptors.ReplaceIfStatementWithReturnStatementFadeOut;

        public static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = (IfStatementSyntax)context.Node;

            if (ifStatement.Parent?.IsKind(SyntaxKind.Block) != true)
                return;

            ReturnStatementSyntax returnStatement = GetReturnStatement(ifStatement.Statement);
            LiteralExpressionSyntax booleanLiteral = GetBooleanLiteral(returnStatement);

            if (booleanLiteral != null)
            {
                ReturnStatementSyntax returnStatement2 = null;
                LiteralExpressionSyntax booleanLiteral2 = null;
                TextSpan span = ifStatement.Span;

                if (ifStatement.Else != null)
                {
                    returnStatement2 = GetReturnStatement(ifStatement.Else.Statement);
                    booleanLiteral2 = GetBooleanLiteral(returnStatement2);
                }
                else
                {
                    var block = (BlockSyntax)ifStatement.Parent;

                    int index = block.Statements.IndexOf(ifStatement);

                    if (index < block.Statements.Count - 1
                        && (index == 0 || !IsPreviousStatementIfStatementWithReturnStatement(block.Statements[index - 1])))
                    {
                        StatementSyntax nextStatement = block.Statements[index + 1];

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
                    && IsNegation(booleanLiteral, booleanLiteral2)
                    && ((BlockSyntax)ifStatement.Parent)
                        .DescendantTrivia(span)
                        .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.ReplaceIfStatementWithReturnStatement,
                        Location.Create(context.Node.SyntaxTree, span));

                    FadeOut(context, ifStatement, returnStatement2);
                }
            }
        }

        internal static LiteralExpressionSyntax GetBooleanLiteral(ReturnStatementSyntax returnStatement)
        {
            switch (returnStatement?.Expression?.Kind())
            {
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                    return (LiteralExpressionSyntax)returnStatement.Expression;
                default:
                    return null;
            }
        }

        internal static LiteralExpressionSyntax GetBooleanLiteral(StatementSyntax statement)
        {
            ReturnStatementSyntax returnStatement = GetReturnStatement(statement);

            return GetBooleanLiteral(returnStatement);
        }

        internal static ReturnStatementSyntax GetReturnStatement(StatementSyntax statement)
        {
            switch (statement?.Kind())
            {
                case SyntaxKind.Block:
                    {
                        var block = (BlockSyntax)statement;

                        if (block.Statements.Count == 1
                            && block.Statements[0].IsKind(SyntaxKind.ReturnStatement))
                        {
                            return (ReturnStatementSyntax)block.Statements[0];
                        }

                        break;
                    }
                case SyntaxKind.ReturnStatement:
                    {
                        return (ReturnStatementSyntax)statement;
                    }
            }

            return null;
        }

        private static bool IsNegation(LiteralExpressionSyntax literal, LiteralExpressionSyntax literal2)
        {
            if (literal.IsKind(SyntaxKind.TrueLiteralExpression))
            {
                return literal2.IsKind(SyntaxKind.FalseLiteralExpression);
            }
            else
            {
                return literal2.IsKind(SyntaxKind.TrueLiteralExpression);
            }
        }

        private static void FadeOut(SyntaxNodeAnalysisContext context, IfStatementSyntax ifStatement, ReturnStatementSyntax returnStatement)
        {
            context.FadeOutToken(_fadeOutDescriptor, ifStatement.IfKeyword);
            context.FadeOutToken(_fadeOutDescriptor, ifStatement.OpenParenToken);
            context.FadeOutToken(_fadeOutDescriptor, ifStatement.CloseParenToken);
            context.FadeOutNode(_fadeOutDescriptor, ifStatement.Statement);

            if (ifStatement.Else != null)
            {
                context.FadeOutNode(_fadeOutDescriptor, ifStatement.Else);
            }
            else
            {
                context.FadeOutNode(_fadeOutDescriptor, returnStatement);
            }
        }

        private static bool IsPreviousStatementIfStatementWithReturnStatement(StatementSyntax statement)
        {
            if (statement.IsKind(SyntaxKind.IfStatement))
            {
                var ifStatement2 = (IfStatementSyntax)statement;

                if (ifStatement2.Else == null)
                    return GetBooleanLiteral(ifStatement2.Statement) != null;
            }

            return false;
        }
    }
}
