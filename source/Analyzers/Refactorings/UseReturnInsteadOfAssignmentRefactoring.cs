// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.FindSymbols;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseReturnInsteadOfAssignmentRefactoring
    {
        public static void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.ContainsDiagnostics)
                return;

            var ifStatement = (IfStatementSyntax)context.Node;

            if (!ifStatement.IsSimpleIf())
            {
                StatementsInfo statementsInfo = SyntaxInfo.StatementsInfo(ifStatement);
                if (statementsInfo.Success)
                {
                    int index = statementsInfo.Statements.IndexOf(ifStatement);

                    ReturnStatementSyntax returnStatement = FindReturnStatementBelow(statementsInfo.Statements, index);
                    if (returnStatement?.ContainsDiagnostics == false)
                    {
                        ExpressionSyntax expression = returnStatement.Expression;
                        if (expression != null
                            && !ifStatement.SpanOrTrailingTriviaContainsDirectives()
                            && !returnStatement.SpanOrLeadingTriviaContainsDirectives())
                        {
                            SemanticModel semanticModel = context.SemanticModel;
                            CancellationToken cancellationToken = context.CancellationToken;

                            ISymbol symbol = semanticModel.GetSymbol(expression, cancellationToken);

                            if (IsLocalDeclaredInScopeOrNonRefOrOutParameterOfEnclosingSymbol(symbol, statementsInfo.Node, semanticModel, cancellationToken)
                                && ifStatement
                                    .GetChain()
                                    .All(ifOrElse => IsSymbolAssignedInLastStatement(ifOrElse, symbol, semanticModel, cancellationToken)))
                            {
                                context.ReportDiagnostic(DiagnosticDescriptors.UseReturnInsteadOfAssignment, ifStatement);
                            }
                        }
                    }
                }
            }
        }

        public static void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.ContainsDiagnostics)
                return;

            var switchStatement = (SwitchStatementSyntax)context.Node;

            StatementsInfo statementsInfo = SyntaxInfo.StatementsInfo(switchStatement);
            if (statementsInfo.Success)
            {
                int index = statementsInfo.Statements.IndexOf(switchStatement);

                ReturnStatementSyntax returnStatement = FindReturnStatementBelow(statementsInfo.Statements, index);
                if (returnStatement != null)
                {
                    ExpressionSyntax expression = returnStatement.Expression;
                    if (expression?.ContainsDiagnostics == false
                        && !switchStatement.SpanOrTrailingTriviaContainsDirectives()
                        && !returnStatement.SpanOrLeadingTriviaContainsDirectives())
                    {
                        SemanticModel semanticModel = context.SemanticModel;
                        CancellationToken cancellationToken = context.CancellationToken;

                        ISymbol symbol = semanticModel.GetSymbol(expression, cancellationToken);

                        if (IsLocalDeclaredInScopeOrNonRefOrOutParameterOfEnclosingSymbol(symbol, statementsInfo.Node, semanticModel, cancellationToken)
                            && switchStatement
                                .Sections
                                .All(section => IsValueAssignedInLastStatement(section, symbol, semanticModel, cancellationToken)))
                        {
                            context.ReportDiagnostic(DiagnosticDescriptors.UseReturnInsteadOfAssignment, switchStatement);
                        }
                    }
                }
            }
        }

        private static ReturnStatementSyntax FindReturnStatementBelow(SyntaxList<StatementSyntax> statements, int i)
        {
            int count = statements.Count;

            i++;

            while (i <= count - 1)
            {
                if (!statements[i].IsKind(SyntaxKind.LocalFunctionStatement))
                    break;

                i++;
            }

            if (i <= count - 1
                && statements[i].IsKind(SyntaxKind.ReturnStatement))
            {
                return (ReturnStatementSyntax)statements[i];
            }

            return null;
        }

        private static LocalDeclarationStatementSyntax FindLocalDeclarationStatementAbove(SyntaxList<StatementSyntax> statements, int i)
        {
            i--;

            while (i >=0)
            {
                if (!statements[i].IsKind(SyntaxKind.LocalFunctionStatement))
                    break;

                i--;
            }

            if (i >= 0
                && statements[i].IsKind(SyntaxKind.LocalDeclarationStatement))
            {
                return (LocalDeclarationStatementSyntax)statements[i];
            }

            return null;
        }

        private static bool IsLocalDeclaredInScopeOrNonRefOrOutParameterOfEnclosingSymbol(ISymbol symbol, SyntaxNode containingNode, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            switch (symbol?.Kind)
            {
                case SymbolKind.Local:
                    {
                        var localSymbol = (ILocalSymbol)symbol;

                        var localDeclarationStatement = localSymbol.GetSyntax(cancellationToken).Parent.Parent as LocalDeclarationStatementSyntax;

                        return localDeclarationStatement?.Parent == containingNode;
                    }
                case SymbolKind.Parameter:
                    {
                        var parameterSymbol = (IParameterSymbol)symbol;

                        if (parameterSymbol.RefKind == RefKind.None)
                        {
                            return semanticModel
                                .GetEnclosingSymbol(containingNode.SpanStart, cancellationToken)?
                                .GetParameters()
                                .Contains(parameterSymbol) == true;
                        }

                        break;
                    }
            }

            return false;
        }

        private static bool IsSymbolAssignedInLastStatement(IfStatementOrElseClause ifOrElse, ISymbol symbol, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            StatementSyntax statement = GetLastStatementOrDefault(ifOrElse.Statement);

            return IsSymbolAssignedInStatement(symbol, statement, semanticModel, cancellationToken);
        }

        private static bool IsValueAssignedInLastStatement(SwitchSectionSyntax switchSection, ISymbol symbol, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            StatementSyntax statement = GetLastStatementBeforeBreakStatementOrDefault(switchSection);

            return IsSymbolAssignedInStatement(symbol, statement, semanticModel, cancellationToken);
        }

        private static bool IsSymbolAssignedInStatement(ISymbol symbol, StatementSyntax statement, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            SimpleAssignmentStatementInfo assignmentInfo = SyntaxInfo.SimpleAssignmentStatementInfo(statement);

            return assignmentInfo.Success
                && symbol.Equals(semanticModel.GetSymbol(assignmentInfo.Left, cancellationToken));
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

        public static async Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            StatementsInfo statementsInfo = SyntaxInfo.StatementsInfo(statement);

            int index = statementsInfo.Statements.IndexOf(statement);

            switch (statement.Kind())
            {
                case SyntaxKind.IfStatement:
                    {
                        var ifStatement = (IfStatementSyntax)statement;

                        IfStatementInfo ifStatementInfo = SyntaxInfo.IfStatementInfo(ifStatement);

                        IEnumerable<ExpressionStatementSyntax> expressionStatements = ifStatementInfo
                            .Nodes
                            .Select(ifOrElse => (ExpressionStatementSyntax)GetLastStatementOrDefault(ifOrElse.Statement));

                        IfStatementSyntax newIfStatement = ifStatement.ReplaceNodes(
                            expressionStatements,
                            (f, _) =>
                            {
                                var assignment = (AssignmentExpressionSyntax)f.Expression;

                                return ReturnStatement(assignment.Right).WithTriviaFrom(f);
                            });

                        StatementsInfo newStatementsInfo = await RefactorAsync(
                            document,
                            statementsInfo,
                            ifStatement,
                            newIfStatement,
                            index,
                            ifStatementInfo.Nodes.Length,
                            ifStatementInfo.EndsWithElse,
                            semanticModel,
                            cancellationToken).ConfigureAwait(false);

                        return await document.ReplaceNodeAsync(statementsInfo.Node, newStatementsInfo.Node, cancellationToken).ConfigureAwait(false);
                    }
                case SyntaxKind.SwitchStatement:
                    {
                        var switchStatement = (SwitchStatementSyntax)statement;

                        SyntaxList<SwitchSectionSyntax> newSections = switchStatement
                            .Sections
                            .Select(CreateNewSection)
                            .ToSyntaxList();

                        SwitchStatementSyntax newSwitchStatement = switchStatement.WithSections(newSections);

                        StatementsInfo newStatementsInfo = await RefactorAsync(
                            document,
                            statementsInfo,
                            switchStatement,
                            newSwitchStatement,
                            index,
                            switchStatement.Sections.Count,
                            switchStatement.Sections.Any(f => f.ContainsDefaultLabel()),
                            semanticModel,
                            cancellationToken).ConfigureAwait(false);

                        return await document.ReplaceNodeAsync(statementsInfo.Node, newStatementsInfo.Node, cancellationToken).ConfigureAwait(false);
                    }
            }

            Debug.Fail(statement.Kind().ToString());

            return document;
        }

        private static SwitchSectionSyntax CreateNewSection(SwitchSectionSyntax section)
        {
            var expressionStatement = (ExpressionStatementSyntax)GetLastStatementBeforeBreakStatementOrDefault(section);

            var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

            section = section.ReplaceNode(expressionStatement, ReturnStatement(assignment.Right).WithTriviaFrom(expressionStatement));

            return section.RemoveStatement(GetStatements(section).Last());
        }

        private static async Task<StatementsInfo> RefactorAsync(
            Document document,
            StatementsInfo statementsInfo,
            StatementSyntax statement,
            StatementSyntax newStatement,
            int index,
            int count,
            bool removeReturnStatement,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ReturnStatementSyntax returnStatement = FindReturnStatementBelow(statementsInfo.Statements, index);

            ExpressionSyntax expression = returnStatement.Expression;
            ExpressionSyntax newExpression = null;

            ISymbol symbol = semanticModel.GetSymbol(expression, cancellationToken);

            if (symbol.IsLocal()
                && index > 0)
            {
                LocalDeclarationStatementSyntax localDeclarationStatement = FindLocalDeclarationStatementAbove(statementsInfo.Statements, index);

                if (localDeclarationStatement?.ContainsDiagnostics == false
                    && !localDeclarationStatement.SpanOrTrailingTriviaContainsDirectives()
                    && !statement.GetLeadingTrivia().Any(f => f.IsDirective))
                {
                    SeparatedSyntaxList<VariableDeclaratorSyntax> declarators = localDeclarationStatement.Declaration.Variables;
                    VariableDeclaratorSyntax declarator = FindVariableDeclarator(semanticModel, symbol, declarators, cancellationToken);

                    if (declarator != null)
                    {
                        ExpressionSyntax value = declarator.Initializer?.Value;

                        if (removeReturnStatement || value != null)
                        {
                            IEnumerable<ReferencedSymbol> referencedSymbols = await SymbolFinder.FindReferencesAsync(symbol, document.Solution(), cancellationToken).ConfigureAwait(false);

                            if (referencedSymbols.First().Locations.Count() == count + 1)
                            {
                                newExpression = value;

                                if (declarators.Count == 1)
                                {
                                    statementsInfo = statementsInfo.RemoveNode(localDeclarationStatement, RemoveHelper.GetRemoveOptions(localDeclarationStatement));
                                    index--;
                                }
                                else
                                {
                                    statementsInfo = statementsInfo.ReplaceNode(localDeclarationStatement, localDeclarationStatement.RemoveNode(declarator, RemoveHelper.GetRemoveOptions(declarator)));
                                }

                                returnStatement = FindReturnStatementBelow(statementsInfo.Statements, index);
                            }
                        }
                    }
                }
            }

            if (removeReturnStatement)
            {
                statementsInfo = statementsInfo.RemoveNode(returnStatement, RemoveHelper.GetRemoveOptions(returnStatement));
            }
            else if (newExpression != null)
            {
                statementsInfo = statementsInfo.ReplaceNode(returnStatement, returnStatement.WithExpression(newExpression.WithTriviaFrom(expression)));
            }

            return statementsInfo.ReplaceNode(statementsInfo.Statements[index], newStatement);
        }

        private static VariableDeclaratorSyntax FindVariableDeclarator(SemanticModel semanticModel, ISymbol symbol, SeparatedSyntaxList<VariableDeclaratorSyntax> declarators, CancellationToken cancellationToken)
        {
            foreach (VariableDeclaratorSyntax declarator in declarators)
            {
                if (semanticModel.GetDeclaredSymbol(declarator, cancellationToken)?.Equals(symbol) == true)
                    return declarator;
            }

            return null;
        }
    }
}
