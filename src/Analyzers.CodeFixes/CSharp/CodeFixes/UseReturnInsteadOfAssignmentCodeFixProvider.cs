// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.Analysis.UseReturnInsteadOfAssignmentAnalyzer;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseReturnInsteadOfAssignmentCodeFixProvider))]
    [Shared]
    public class UseReturnInsteadOfAssignmentCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.UseReturnInsteadOfAssignment); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out StatementSyntax statement, predicate: f => f.IsKind(SyntaxKind.IfStatement, SyntaxKind.SwitchStatement)))
                return;

            CodeAction codeAction = CodeAction.Create(
                "Use return instead of assignment",
                cancellationToken => RefactorAsync(context.Document, statement, cancellationToken),
                GetEquivalenceKey(DiagnosticIdentifiers.UseReturnInsteadOfAssignment));

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

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
                            CreateNewIfStatement,
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

                        StatementListInfo newStatementsInfo = await RefactorAsync(
                            document,
                            statementsInfo,
                            switchStatement,
                            CreateNewSwitchStatement,
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

        private static IfStatementSyntax CreateNewIfStatement(IfStatementSyntax ifStatement)
        {
            IEnumerable<ExpressionStatementSyntax> expressionStatements = ifStatement
                .AsCascade()
                .Select(ifOrElse => (ExpressionStatementSyntax)GetLastStatementOrDefault(ifOrElse.Statement));

            return ifStatement.ReplaceNodes(
                expressionStatements,
                (f, _) =>
                {
                    var assignment = (AssignmentExpressionSyntax)f.Expression;

                    return ReturnStatement(assignment.Right).WithTriviaFrom(f);
                });
        }

        private static SwitchStatementSyntax CreateNewSwitchStatement(SwitchStatementSyntax switchStatement)
        {
            SyntaxList<SwitchSectionSyntax> newSections = switchStatement
                .Sections
                .Select(CreateNewSection)
                .ToSyntaxList();

            return switchStatement.WithSections(newSections);

            SwitchSectionSyntax CreateNewSection(SwitchSectionSyntax section)
            {
                var expressionStatement = (ExpressionStatementSyntax)GetLastStatementBeforeBreakStatementOrDefault(section);

                var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

                section = section.ReplaceNode(expressionStatement, ReturnStatement(assignment.Right).WithTriviaFrom(expressionStatement));

                return section.RemoveStatement(GetStatements(section).Last());
            }
        }

        private static async Task<StatementListInfo> RefactorAsync<TStatement>(
            Document document,
            StatementListInfo statementsInfo,
            TStatement statement,
            Func<TStatement, TStatement> createNewStatement,
            int index,
            int count,
            bool removeReturnStatement,
            SemanticModel semanticModel,
            CancellationToken cancellationToken) where TStatement : StatementSyntax
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

            StatementSyntax oldNode = statementsInfo[index];
            TStatement newNode = createNewStatement((TStatement)oldNode).WithFormatterAnnotation();

            return statementsInfo.ReplaceNode(oldNode, newNode);
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
