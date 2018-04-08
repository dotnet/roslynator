// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.Analysis.UseReturnInsteadOfAssignmentAnalyzer;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseReturnInsteadOfAssignmentRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            StatementListInfo statementsInfo = SyntaxInfo.StatementListInfo(statement);

            int index = statementsInfo.IndexOf(statement);

            switch (statement.Kind())
            {
                case SyntaxKind.IfStatement:
                    {
                        var ifStatement = (IfStatementSyntax)statement;

                        IEnumerable<ExpressionStatementSyntax> expressionStatements = ifStatement
                            .AsCascade()
                            .Select(ifOrElse => (ExpressionStatementSyntax)GetLastStatementOrDefault(ifOrElse.Statement));

                        IfStatementSyntax newIfStatement = ifStatement.ReplaceNodes(
                            expressionStatements,
                            (f, _) =>
                            {
                                var assignment = (AssignmentExpressionSyntax)f.Expression;

                                return ReturnStatement(assignment.Right).WithTriviaFrom(f);
                            });

                        int count = 0;
                        bool endsWithElse = false;

                        foreach (IfStatementOrElseClause ifOrElse in ifStatement.AsCascade())
                        {
                            count++;
                            endsWithElse = ifOrElse.IsElse;
                        }

                        StatementListInfo newStatementsInfo = await RefactorAsync(
                            document,
                            statementsInfo,
                            ifStatement,
                            newIfStatement,
                            index,
                            count,
                            endsWithElse,
                            semanticModel,
                            cancellationToken).ConfigureAwait(false);

                        return await document.ReplaceNodeAsync(statementsInfo.Parent, newStatementsInfo.Parent, cancellationToken).ConfigureAwait(false);
                    }
                case SyntaxKind.SwitchStatement:
                    {
                        var switchStatement = (SwitchStatementSyntax)statement;

                        SyntaxList<SwitchSectionSyntax> newSections = switchStatement
                            .Sections
                            .Select(CreateNewSection)
                            .ToSyntaxList();

                        SwitchStatementSyntax newSwitchStatement = switchStatement.WithSections(newSections);

                        StatementListInfo newStatementsInfo = await RefactorAsync(
                            document,
                            statementsInfo,
                            switchStatement,
                            newSwitchStatement,
                            index,
                            switchStatement.Sections.Count,
                            switchStatement.Sections.Any(f => f.ContainsDefaultLabel()),
                            semanticModel,
                            cancellationToken).ConfigureAwait(false);

                        return await document.ReplaceNodeAsync(statementsInfo.Parent, newStatementsInfo.Parent, cancellationToken).ConfigureAwait(false);
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

        private static async Task<StatementListInfo> RefactorAsync(
            Document document,
            StatementListInfo statementsInfo,
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

            if (symbol?.Kind == SymbolKind.Local
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
                                    statementsInfo = statementsInfo.RemoveNode(localDeclarationStatement, SyntaxRemover.GetRemoveOptions(localDeclarationStatement));
                                    index--;
                                }
                                else
                                {
                                    statementsInfo = statementsInfo.ReplaceNode(localDeclarationStatement, localDeclarationStatement.RemoveNode(declarator, SyntaxRemover.GetRemoveOptions(declarator)));
                                }

                                returnStatement = FindReturnStatementBelow(statementsInfo.Statements, index);
                            }
                        }
                    }
                }
            }

            if (removeReturnStatement)
            {
                statementsInfo = statementsInfo.RemoveNode(returnStatement, SyntaxRemover.GetRemoveOptions(returnStatement));
            }
            else if (newExpression != null)
            {
                statementsInfo = statementsInfo.ReplaceNode(returnStatement, returnStatement.WithExpression(newExpression.WithTriviaFrom(expression)));
            }

            return statementsInfo.ReplaceNode(statementsInfo[index], newStatement);
        }

        private static LocalDeclarationStatementSyntax FindLocalDeclarationStatementAbove(SyntaxList<StatementSyntax> statements, int i)
        {
            i--;

            while (i >= 0)
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
