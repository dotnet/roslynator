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
                StatementContainer container;
                if (StatementContainer.TryCreate(ifStatement, out container))
                {
                    int index = container.Statements.IndexOf(ifStatement);

                    ReturnStatementSyntax returnStatement = FindReturnStatementBelow(container.Statements, index);
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

                            if (IsLocalDeclaredInScopeOrNonRefOrOutParameterOfEnclosingSymbol(symbol, container.Node, semanticModel, cancellationToken)
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

            StatementContainer container;
            if (StatementContainer.TryCreate(switchStatement, out container))
            {
                int index = container.Statements.IndexOf(switchStatement);

                ReturnStatementSyntax returnStatement = FindReturnStatementBelow(container.Statements, index);
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

                        if (IsLocalDeclaredInScopeOrNonRefOrOutParameterOfEnclosingSymbol(symbol, container.Node, semanticModel, cancellationToken)
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
            SimpleAssignmentStatement assignment;
            if (SimpleAssignmentStatement.TryCreate(statement, out assignment))
            {
                return symbol.Equals(semanticModel.GetSymbol(assignment.Left, cancellationToken));
            }

            return false;
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

            StatementContainer container = StatementContainer.Create(statement);

            int index = container.Statements.IndexOf(statement);

            switch (statement.Kind())
            {
                case SyntaxKind.IfStatement:
                    {
                        var ifStatement = (IfStatementSyntax)statement;

                        IfStatement chain = Syntax.IfStatement.Create(ifStatement);

                        IEnumerable<ExpressionStatementSyntax> expressionStatements = chain
                            .Nodes
                            .Select(ifOrElse => (ExpressionStatementSyntax)GetLastStatementOrDefault(ifOrElse.Statement));

                        IfStatementSyntax newIfStatement = ifStatement.ReplaceNodes(
                            expressionStatements,
                            (f, g) =>
                            {
                                var assignment = (AssignmentExpressionSyntax)f.Expression;

                                return ReturnStatement(assignment.Right).WithTriviaFrom(f);
                            });

                        StatementContainer newContainer = await RefactorAsync(
                            document,
                            container,
                            ifStatement,
                            newIfStatement,
                            index,
                            chain.Nodes.Length,
                            semanticModel,
                            cancellationToken,
                            chain.EndsWithElse).ConfigureAwait(false);

                        return await document.ReplaceNodeAsync(container.Node, newContainer.Node, cancellationToken).ConfigureAwait(false);
                    }
                case SyntaxKind.SwitchStatement:
                    {
                        var switchStatement = (SwitchStatementSyntax)statement;

                        SyntaxList<SwitchSectionSyntax> newSections = switchStatement
                            .Sections
                            .Select(CreateNewSection)
                            .ToSyntaxList();

                        SwitchStatementSyntax newSwitchStatement = switchStatement.WithSections(newSections);

                        StatementContainer newContainer = await RefactorAsync(
                            document,
                            container,
                            switchStatement,
                            newSwitchStatement,
                            index,
                            switchStatement.Sections.Count,
                            semanticModel,
                            cancellationToken,
                            switchStatement.Sections.Any(f => f.ContainsDefaultLabel())).ConfigureAwait(false);

                        return await document.ReplaceNodeAsync(container.Node, newContainer.Node, cancellationToken).ConfigureAwait(false);
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

            section = section.RemoveStatement(GetStatements(section).Last());

            return section;
        }

        private static async Task<StatementContainer> RefactorAsync(
            Document document,
            StatementContainer container,
            StatementSyntax statement,
            StatementSyntax newStatement,
            int index,
            int count,
            SemanticModel semanticModel,
            CancellationToken cancellationToken,
            bool removeReturnStatement)
        {
            ReturnStatementSyntax returnStatement = FindReturnStatementBelow(container.Statements, index);

            ExpressionSyntax expression = returnStatement.Expression;
            ExpressionSyntax newExpression = null;

            ISymbol symbol = semanticModel.GetSymbol(expression, cancellationToken);

            if (symbol.IsLocal()
                && index > 0)
            {
                LocalDeclarationStatementSyntax localDeclarationStatement = FindLocalDeclarationStatementAbove(container.Statements, index);

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
                                    container = container.RemoveNode(localDeclarationStatement, RemoveHelper.GetRemoveOptions(localDeclarationStatement));
                                    index--;
                                }
                                else
                                {
                                    container = container.ReplaceNode(localDeclarationStatement, localDeclarationStatement.RemoveNode(declarator, RemoveHelper.GetRemoveOptions(declarator)));
                                }

                                returnStatement = FindReturnStatementBelow(container.Statements, index);
                            }
                        }
                    }
                }
            }

            if (removeReturnStatement)
            {
                container = container.RemoveNode(returnStatement, RemoveHelper.GetRemoveOptions(returnStatement));
            }
            else if (newExpression != null)
            {
                container = container.ReplaceNode(returnStatement, returnStatement.WithExpression(newExpression.WithTriviaFrom(expression)));
            }

            return container.ReplaceNode(container.Statements[index], newStatement);
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
