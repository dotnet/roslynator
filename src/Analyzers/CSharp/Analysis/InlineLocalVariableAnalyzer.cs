// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InlineLocalVariableAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.InlineLocalVariable,
                    DiagnosticDescriptors.InlineLocalVariableFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeLocalDeclarationStatement, SyntaxKind.LocalDeclarationStatement);
        }

        public static void AnalyzeLocalDeclarationStatement(SyntaxNodeAnalysisContext context)
        {
            var localDeclarationStatement = (LocalDeclarationStatementSyntax)context.Node;

            if (localDeclarationStatement.ContainsDiagnostics)
                return;

            if (localDeclarationStatement.SpanOrTrailingTriviaContainsDirectives())
                return;

            SingleLocalDeclarationStatementInfo localDeclarationInfo = SyntaxInfo.SingleLocalDeclarationStatementInfo(localDeclarationStatement);

            if (!localDeclarationInfo.Success)
                return;

            ExpressionSyntax value = localDeclarationInfo.Value;

            if (value == null)
                return;

            SyntaxList<StatementSyntax> statements = SyntaxInfo.StatementListInfo(localDeclarationStatement).Statements;

            if (!statements.Any())
                return;

            int index = statements.IndexOf(localDeclarationStatement);

            if (index == statements.Count - 1)
                return;

            StatementSyntax nextStatement = statements[index + 1];

            if (nextStatement.ContainsDiagnostics)
                return;

            switch (nextStatement.Kind())
            {
                case SyntaxKind.ExpressionStatement:
                    {
                        Analyze(context, statements, localDeclarationInfo, index, (ExpressionStatementSyntax)nextStatement);
                        break;
                    }
                case SyntaxKind.LocalDeclarationStatement:
                    {
                        Analyze(context, statements, localDeclarationInfo, index, (LocalDeclarationStatementSyntax)nextStatement);
                        break;
                    }
                case SyntaxKind.ReturnStatement:
                    {
                        var returnStatement = (ReturnStatementSyntax)nextStatement;

                        if (!returnStatement.SpanOrLeadingTriviaContainsDirectives())
                        {
                            ExpressionSyntax expression = returnStatement.Expression;

                            if (expression?.Kind() == SyntaxKind.IdentifierName)
                            {
                                var identifierName = (IdentifierNameSyntax)expression;

                                if (string.Equals(localDeclarationInfo.IdentifierText, identifierName.Identifier.ValueText, StringComparison.Ordinal))
                                    ReportDiagnostic(context, localDeclarationInfo, expression);
                            }
                        }

                        break;
                    }
                case SyntaxKind.ForEachStatement:
                    {
                        if (value.IsSingleLine()
                            && !value.IsKind(SyntaxKind.ArrayInitializerExpression))
                        {
                            var forEachStatement = (ForEachStatementSyntax)nextStatement;
                            Analyze(context, statements, localDeclarationInfo, forEachStatement.Expression);
                        }

                        break;
                    }
                case SyntaxKind.SwitchStatement:
                    {
                        if (value.IsSingleLine())
                        {
                            var switchStatement = (SwitchStatementSyntax)nextStatement;
                            Analyze(context, statements, localDeclarationInfo, switchStatement.Expression);
                        }

                        break;
                    }
            }
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            SyntaxList<StatementSyntax> statements,
            SingleLocalDeclarationStatementInfo localDeclarationInfo,
            int index,
            ExpressionStatementSyntax expressionStatement)
        {
            SimpleAssignmentExpressionInfo assignment = SyntaxInfo.SimpleAssignmentExpressionInfo(expressionStatement.Expression);

            if (!assignment.Success)
                return;

            if (assignment.Right.Kind() != SyntaxKind.IdentifierName)
                return;

            var identifierName = (IdentifierNameSyntax)assignment.Right;

            string name = localDeclarationInfo.IdentifierText;

            if (!string.Equals(name, identifierName.Identifier.ValueText, StringComparison.Ordinal))
                return;

            VariableDeclaratorSyntax declarator = localDeclarationInfo.Declarator;

            ISymbol localSymbol = context.SemanticModel.GetDeclaredSymbol(declarator, context.CancellationToken);

            if (localSymbol?.IsErrorType() != false)
                return;

            bool isReferenced = IsLocalVariableReferenced(localSymbol, name, assignment.Left, assignment.Left.Span, context.SemanticModel, context.CancellationToken);

            if (!isReferenced
                && index < statements.Count - 2)
            {
                TextSpan span = TextSpan.FromBounds(statements[index + 2].SpanStart, statements.Last().Span.End);

                isReferenced = IsLocalVariableReferenced(localSymbol, name, localDeclarationInfo.Statement.Parent, span, context.SemanticModel, context.CancellationToken);
            }

            if (isReferenced)
                return;

            if (expressionStatement.SpanOrLeadingTriviaContainsDirectives())
                return;

            ReportDiagnostic(context, localDeclarationInfo, assignment.Right);
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            SyntaxList<StatementSyntax> statements,
            SingleLocalDeclarationStatementInfo localDeclarationInfo,
            int index,
            LocalDeclarationStatementSyntax localDeclaration2)
        {
            ExpressionSyntax value2 = localDeclaration2
                .Declaration?
                .Variables
                .SingleOrDefault(shouldThrow: false)?
                .Initializer?
                .Value;

            if (value2?.Kind() != SyntaxKind.IdentifierName)
                return;

            var identifierName = (IdentifierNameSyntax)value2;

            string name = localDeclarationInfo.IdentifierText;

            if (!string.Equals(name, identifierName.Identifier.ValueText, StringComparison.Ordinal))
                return;

            bool isReferenced = false;

            if (index < statements.Count - 2)
            {
                TextSpan span = TextSpan.FromBounds(statements[index + 2].SpanStart, statements.Last().Span.End);

                isReferenced = IsLocalVariableReferenced(localDeclarationInfo.Declarator, name, localDeclarationInfo.Statement.Parent, span, context.SemanticModel, context.CancellationToken);
            }

            if (isReferenced)
                return;

            if (localDeclaration2.SpanOrLeadingTriviaContainsDirectives())
                return;

            ReportDiagnostic(context, localDeclarationInfo, value2);
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            SyntaxList<StatementSyntax> statements,
            SingleLocalDeclarationStatementInfo localDeclarationInfo,
            ExpressionSyntax expression)
        {
            if (expression?.Kind() != SyntaxKind.IdentifierName)
                return;

            var identifierName = (IdentifierNameSyntax)expression;

            string name = localDeclarationInfo.IdentifierText;

            if (!string.Equals(name, identifierName.Identifier.ValueText, StringComparison.Ordinal))
                return;

            TextSpan span = TextSpan.FromBounds(expression.Span.End, statements.Last().Span.End);

            SyntaxNode parent = localDeclarationInfo.Statement.Parent;

            if (IsLocalVariableReferenced(localDeclarationInfo.Declarator, name, parent, span, context.SemanticModel, context.CancellationToken))
                return;

            if (parent.ContainsDirectives(TextSpan.FromBounds(localDeclarationInfo.Statement.SpanStart, expression.Span.End)))
                return;

            ReportDiagnostic(context, localDeclarationInfo, expression);
        }

        private static void ReportDiagnostic(
            SyntaxNodeAnalysisContext context,
            SingleLocalDeclarationStatementInfo localDeclarationInfo,
            ExpressionSyntax expression)
        {
            context.ReportDiagnostic(DiagnosticDescriptors.InlineLocalVariable, localDeclarationInfo.Statement);

            foreach (SyntaxToken modifier in localDeclarationInfo.Modifiers)
                context.ReportToken(DiagnosticDescriptors.InlineLocalVariableFadeOut, modifier);

            context.ReportNode(DiagnosticDescriptors.InlineLocalVariableFadeOut, localDeclarationInfo.Type);
            context.ReportToken(DiagnosticDescriptors.InlineLocalVariableFadeOut, localDeclarationInfo.Identifier);
            context.ReportToken(DiagnosticDescriptors.InlineLocalVariableFadeOut, localDeclarationInfo.EqualsToken);
            context.ReportToken(DiagnosticDescriptors.InlineLocalVariableFadeOut, localDeclarationInfo.SemicolonToken);
            context.ReportNode(DiagnosticDescriptors.InlineLocalVariableFadeOut, expression);
        }

        private static bool IsLocalVariableReferenced(
            VariableDeclaratorSyntax declarator,
            string name,
            SyntaxNode node,
            TextSpan span,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ISymbol symbol = semanticModel.GetDeclaredSymbol(declarator, cancellationToken);

            return symbol?.IsErrorType() == false
                && IsLocalVariableReferenced(symbol, name, node, span, semanticModel, cancellationToken);
        }

        private static bool IsLocalVariableReferenced(
            ISymbol symbol,
            string name,
            SyntaxNode node,
            TextSpan span,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            foreach (SyntaxNode descendant in node.DescendantNodesAndSelf(span))
            {
                if (descendant.IsKind(SyntaxKind.IdentifierName))
                {
                    var identifierName = (IdentifierNameSyntax)descendant;

                    if (string.Equals(identifierName.Identifier.ValueText, name, StringComparison.Ordinal)
                        && symbol.Equals(semanticModel.GetSymbol(identifierName, cancellationToken)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
