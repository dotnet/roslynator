// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
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

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseReturnInsteadOfAssignmentRefactoring
    {
        public static void AnalyzeBlock(SyntaxNodeAnalysisContext context)
        {
            var block = (BlockSyntax)context.Node;

            Analyze(context, block, block.Statements);
        }

        public static void AnalyzeSwitchSection(SyntaxNodeAnalysisContext context)
        {
            var switchSection = (SwitchSectionSyntax)context.Node;

            Analyze(context, switchSection, switchSection.Statements);
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            SyntaxNode containingNode,
            SyntaxList<StatementSyntax> statements)
        {
            int count = statements.Count;

            if (count > 1)
            {
                int i = count - 1;

                if (i >= 1
                    && statements[i].IsKind(SyntaxKind.ReturnStatement))
                {
                    var returnStatement = (ReturnStatementSyntax)statements[i];

                    ExpressionSyntax expression = returnStatement.Expression;

                    if (expression?.IsMissing == false)
                    {
                        StatementSyntax statement = statements[i - 1];

                        SyntaxKind statementKind = statement.Kind();

                        if (statementKind == SyntaxKind.IfStatement)
                        {
                            var ifStatement = (IfStatementSyntax)statements[i - 1];

                            if (!ifStatement.IsSimpleIf())
                            {
                                SemanticModel semanticModel = context.SemanticModel;
                                CancellationToken cancellationToken = context.CancellationToken;

                                ISymbol symbol = semanticModel.GetSymbol(expression, cancellationToken);

                                if (IsLocalDeclaredInScopeOrNonRefOrOutParameterOfEnclosingSymbol(symbol, containingNode, semanticModel, cancellationToken)
                                    && ifStatement.GetChain().All(ifOrElse => IsValueAssignedInLastStatement(ifOrElse, symbol, semanticModel, cancellationToken))
                                    && !containingNode.ContainsDirectives(TextSpan.FromBounds(ifStatement.SpanStart, returnStatement.Span.End)))
                                {
                                    context.ReportDiagnostic(
                                        DiagnosticDescriptors.UseReturnInsteadOfAssignment,
                                        GetLastStatementOrDefault(ifStatement));
                                }
                            }
                        }
                        else if (statementKind == SyntaxKind.SwitchStatement)
                        {
                            SemanticModel semanticModel = context.SemanticModel;
                            CancellationToken cancellationToken = context.CancellationToken;

                            ISymbol symbol = semanticModel.GetSymbol(expression, cancellationToken);

                            if (IsLocalDeclaredInScopeOrNonRefOrOutParameterOfEnclosingSymbol(symbol, containingNode, semanticModel, cancellationToken))
                            {
                                var switchStatement = (SwitchStatementSyntax)statements[i - 1];

                                SyntaxList<SwitchSectionSyntax> sections = switchStatement.Sections;

                                if (sections.All(section => IsValueAssignedInLastStatement(section, symbol, semanticModel, cancellationToken))
                                    && !containingNode.ContainsDirectives(TextSpan.FromBounds(switchStatement.SpanStart, returnStatement.Span.End)))
                                {
                                    context.ReportDiagnostic(
                                        DiagnosticDescriptors.UseReturnInsteadOfAssignment,
                                        GetLastStatementBeforeBreakStatementOrDefault(sections.First()));
                                }
                            }
                        }
                    }
                }
            }
        }

        private static bool IsLocalDeclaredInScopeOrNonRefOrOutParameterOfEnclosingSymbol(ISymbol symbol, SyntaxNode containingNode, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            switch (symbol?.Kind)
            {
                case SymbolKind.Local:
                    {
                        var localSymbol = (ILocalSymbol)symbol;

                        return GetLocalDeclarationStatement(localSymbol, cancellationToken)?.Parent == containingNode;
                    }
                case SymbolKind.Parameter:
                    {
                        var parameterSymbol = (IParameterSymbol)symbol;

                        if (parameterSymbol.RefKind == RefKind.None)
                        {
                            ISymbol enclosingSymbol = semanticModel.GetEnclosingSymbol(containingNode.SpanStart, cancellationToken);

                            return enclosingSymbol?.GetParameters().Contains(parameterSymbol) == true;
                        }

                        break;
                    }
            }

            return false;
        }

        private static SyntaxNode GetLocalDeclarationStatement(ILocalSymbol localSymbol, CancellationToken cancellationToken)
        {
            var declarator = localSymbol.GetSyntax(cancellationToken) as VariableDeclaratorSyntax;

            if (declarator != null)
            {
                var declaration = declarator.Parent as VariableDeclarationSyntax;

                if (declaration != null)
                    return declaration.Parent as LocalDeclarationStatementSyntax;
            }

            return null;
        }

        private static bool IsValueAssignedInLastStatement(IfStatementOrElseClause ifOrElse, ISymbol symbol, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            StatementSyntax statement = GetLastStatementOrDefault(ifOrElse.Statement);

            ExpressionSyntax left = GetLastAssignmentOrDefault(statement)?.Left;

            return left != null
                && symbol.Equals(semanticModel.GetSymbol(left, cancellationToken));
        }

        private static bool IsValueAssignedInLastStatement(SwitchSectionSyntax switchSection, ISymbol symbol, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            StatementSyntax statement = GetLastStatementBeforeBreakStatementOrDefault(switchSection);

            ExpressionSyntax left = GetLastAssignmentOrDefault(statement)?.Left;

            return left != null
                && symbol.Equals(semanticModel.GetSymbol(left, cancellationToken));
        }

        private static StatementSyntax GetLastStatementOrDefault(StatementSyntax statement)
        {
            if (statement?.IsKind(SyntaxKind.Block) == true)
            {
                return ((BlockSyntax)statement).Statements.LastOrDefault();
            }
            else
            {
                return statement;
            }
        }

        private static StatementSyntax GetLastStatementBeforeBreakStatementOrDefault(SwitchSectionSyntax switchSection)
        {
            SyntaxList<StatementSyntax> statements = GetStatements(switchSection);

            if (statements.Count > 1
                && statements.Last().IsKind(SyntaxKind.BreakStatement))
            {
                return statements[statements.Count - 2];
            }

            return null;
        }

        private static SyntaxList<StatementSyntax> GetStatements(SwitchSectionSyntax switchSection)
        {
            SyntaxList<StatementSyntax> statements = switchSection.Statements;

            if (statements.Count == 1
                && statements[0].IsKind(SyntaxKind.Block))
            {
                return ((BlockSyntax)statements[0]).Statements;
            }

            return statements;
        }

        private static AssignmentExpressionSyntax GetLastAssignmentOrDefault(StatementSyntax lastStatement)
        {
            if (lastStatement?.IsKind(SyntaxKind.ExpressionStatement) == true)
            {
                var expressionStatement = (ExpressionStatementSyntax)lastStatement;

                ExpressionSyntax expression = expressionStatement.Expression;

                if (expression?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true)
                    return (AssignmentExpressionSyntax)expression;
            }

            return null;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken)
        {
            StatementContainer container = StatementContainer.Create(statement);

            SyntaxList<StatementSyntax> statements = container.Statements;

            var returnStatement = (ReturnStatementSyntax)statements.Last();

            int returnStatementIndex = statements.IndexOf(returnStatement);

            ExpressionSyntax expression = returnStatement.Expression;

            SyntaxKind kind = statement.Kind();

            if (kind == SyntaxKind.IfStatement)
            {
                var ifStatement = (IfStatementSyntax)statement;

                IfStatement chain = Syntax.IfStatement.Create(ifStatement);

                ExpressionStatementSyntax[] expressionStatements = chain
                    .Nodes
                    .Select(ifOrElse => (ExpressionStatementSyntax)GetLastStatementOrDefault(ifOrElse.Statement))
                    .ToArray();

                IfStatementSyntax newIfStatement = ifStatement.ReplaceNodes(
                    expressionStatements,
                    (f, g) =>
                    {
                        var assignment = (AssignmentExpressionSyntax)f.Expression;

                        return ReturnStatement(assignment.Right).WithTriviaFrom(f);
                    });

                SyntaxList<StatementSyntax> newStatements = statements.Replace(ifStatement, newIfStatement);

                StatementContainer newContainer = container.WithStatements(newStatements);

                SyntaxNode newNode = newContainer.Node;

                if (chain.EndsWithElse)
                    newNode = newNode.RemoveStatement(newContainer.Statements[returnStatementIndex]);

                return document.ReplaceNodeAsync(container.Node, newNode, cancellationToken);
            }
            else if (kind == SyntaxKind.SwitchStatement)
            {
                var switchStatement = (SwitchStatementSyntax)statement;

                SyntaxList<SwitchSectionSyntax> newSections = switchStatement
                    .Sections
                    .Select(section => CreateNewSection(section))
                    .ToSyntaxList();

                SwitchStatementSyntax newSwitchStatement = switchStatement.WithSections(newSections);

                SyntaxList<StatementSyntax> newStatements = statements.Replace(switchStatement, newSwitchStatement);

                StatementContainer newContainer = container.WithStatements(newStatements);

                SyntaxNode newNode = newContainer.Node;

                if (switchStatement.Sections.Any(f => f.ContainsDefaultLabel()))
                    newNode = newNode.RemoveStatement(newContainer.Statements[returnStatementIndex]);

                return document.ReplaceNodeAsync(container.Node, newNode, cancellationToken);
            }

            Debug.Assert(false, statement.Kind().ToString());

            return Task.FromResult(document);
        }

        private static SwitchSectionSyntax CreateNewSection(SwitchSectionSyntax section)
        {
            var expressionStatement = (ExpressionStatementSyntax)GetLastStatementBeforeBreakStatementOrDefault(section);

            var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

            section = section.ReplaceNode(expressionStatement, ReturnStatement(assignment.Right).WithTriviaFrom(expressionStatement));

            section = section.RemoveStatement(GetStatements(section).Last());

            return section;
        }
    }
}
