// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Extensions;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseCoalesceExpressionDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.UseCoalesceExpression,
                    DiagnosticDescriptors.UseCoalesceExpressionFadeOut);
            }
        }

        public DiagnosticDescriptor FadeOutDescriptor
        {
            get { return DiagnosticDescriptors.UseCoalesceExpressionFadeOut; }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(f => AnalyzeSimpleAssignmentExpression(f), SyntaxKind.SimpleAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeLocalDeclarationStatement(f), SyntaxKind.LocalDeclarationStatement);
        }

        private void AnalyzeSimpleAssignmentExpression(SyntaxNodeAnalysisContext context)
        {
            var assignment = (AssignmentExpressionSyntax)context.Node;

            ExpressionSyntax left = assignment.Left;

            if (left != null)
            {
                ExpressionSyntax right = assignment.Right;

                if (right != null)
                {
                    SyntaxNode parent = assignment.Parent;

                    if (parent?.IsKind(SyntaxKind.ExpressionStatement) == true)
                        Analyze(context, left, right, (StatementSyntax)parent);
                }
            }
        }

        private void AnalyzeLocalDeclarationStatement(SyntaxNodeAnalysisContext context)
        {
            var localDeclarationStatement = (LocalDeclarationStatementSyntax)context.Node;

            VariableDeclarationSyntax declaration = localDeclarationStatement.Declaration;

            if (declaration != null)
            {
                SeparatedSyntaxList<VariableDeclaratorSyntax> variables = declaration.Variables;

                if (variables.Count == 1)
                {
                    VariableDeclaratorSyntax declarator = variables[0];

                    ExpressionSyntax value = declarator.Initializer?.Value;

                    if (value != null)
                        Analyze(context, declarator.Identifier, value, localDeclarationStatement);
                }
            }
        }

        private void Analyze(
            SyntaxNodeAnalysisContext context,
            SyntaxNodeOrToken nodeOrToken,
            ExpressionSyntax value,
            StatementSyntax statement)
        {
            SyntaxList<StatementSyntax> statements = GetStatements(statement);

            if (statements.Any())
                Analyze(context, nodeOrToken, value, statement, statements);
        }

        private void Analyze(
            SyntaxNodeAnalysisContext context,
            SyntaxNodeOrToken nodeOrToken,
            ExpressionSyntax value,
            StatementSyntax statement,
            SyntaxList<StatementSyntax> statements)
        {
            int index = statements.IndexOf(statement);

            if (index < statements.Count - 1)
            {
                StatementSyntax nextStatement = statements[index + 1];

                if (nextStatement.IsKind(SyntaxKind.IfStatement))
                {
                    var ifStatement = (IfStatementSyntax)nextStatement;

                    if (!IfElseChain.IsPartOfChain(ifStatement))
                    {
                        ExpressionSyntax condition = ifStatement.Condition;

                        if (condition?.IsKind(SyntaxKind.EqualsExpression) == true)
                        {
                            var equalsExpression = (BinaryExpressionSyntax)condition;

                            ExpressionSyntax left = equalsExpression.Left;

                            if (IsEquivalent(nodeOrToken, left))
                            {
                                ExpressionSyntax right = equalsExpression.Right;

                                if (right?.IsKind(SyntaxKind.NullLiteralExpression) == true)
                                {
                                    StatementSyntax blockOrStatement = ifStatement.Statement;

                                    StatementSyntax childStatement = GetSingleStatementOrDefault(blockOrStatement);

                                    if (childStatement?.IsKind(SyntaxKind.ExpressionStatement) == true)
                                    {
                                        var expressionStatement = (ExpressionStatementSyntax)childStatement;

                                        ExpressionSyntax expression = expressionStatement.Expression;

                                        if (expression?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true)
                                        {
                                            var assignment = (AssignmentExpressionSyntax)expression;

                                            ExpressionSyntax left2 = assignment.Left;

                                            if (IsEquivalent(nodeOrToken, left2)
                                                && assignment.Right != null
                                                && !statement.Parent.ContainsDirectives(TextSpan.FromBounds(value.Span.End, nextStatement.Span.End)))
                                            {
                                                context.ReportDiagnostic(
                                                    DiagnosticDescriptors.UseCoalesceExpression,
                                                    nextStatement.GetLocation());

                                                context.ReportToken(FadeOutDescriptor, ifStatement.IfKeyword);

                                                context.ReportNode(FadeOutDescriptor, condition);

                                                if (blockOrStatement.IsKind(SyntaxKind.Block))
                                                    context.ReportBraces(FadeOutDescriptor, (BlockSyntax)blockOrStatement);

                                                context.ReportNode(FadeOutDescriptor, left2);
                                                context.ReportToken(FadeOutDescriptor, assignment.OperatorToken);
                                                context.ReportToken(FadeOutDescriptor, expressionStatement.SemicolonToken);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static StatementSyntax GetSingleStatementOrDefault(StatementSyntax statement)
        {
            if (statement != null)
            {
                if (statement.IsKind(SyntaxKind.Block))
                {
                    var block = (BlockSyntax)statement;

                    SyntaxList<StatementSyntax> statements = block.Statements;

                    if (statements.Count == 1)
                        return statements[0];
                }
                else
                {
                    return statement;
                }
            }

            return null;
        }

        private SyntaxList<StatementSyntax> GetStatements(StatementSyntax statement)
        {
            SyntaxNode parent = statement.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.Block:
                    return ((BlockSyntax)parent).Statements;
                case SyntaxKind.SwitchSection:
                    return ((SwitchSectionSyntax)parent).Statements;
                default:
                    return default(SyntaxList<StatementSyntax>);
            }
        }

        private bool IsEquivalent(SyntaxNodeOrToken nodeOrToken, ExpressionSyntax expression)
        {
            if (expression != null)
            {
                if (nodeOrToken.IsNode)
                {
                    return nodeOrToken.AsNode().IsEquivalentTo(expression, topLevel: false);
                }
                else if (nodeOrToken.IsToken)
                {
                    if (expression.IsKind(SyntaxKind.IdentifierName))
                    {
                        var identifierName = (IdentifierNameSyntax)expression;

                        return string.Equals(nodeOrToken.AsToken().ValueText, identifierName.Identifier.ValueText, StringComparison.Ordinal);
                    }
                }
            }

            return false;
        }
    }
}
