// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
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

            Diagnostic diagnostic = context.Diagnostics[0];

            const string title = "Use return instead of assignment";

            switch (statement.Kind())
            {
                case SyntaxKind.IfStatement:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            title,
                            ct => RefactorAsync(context.Document, (IfStatementSyntax)statement, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case SyntaxKind.SwitchStatement:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            title,
                            ct => RefactorAsync(context.Document, (SwitchStatementSyntax)statement, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken)
        {
            StatementListInfo statementsInfo = SyntaxInfo.StatementListInfo(ifStatement);

            IfStatementCascadeInfo ifCascadeInfo = ifStatement.GetCascadeInfo();

            StatementListInfo newStatementsInfo = await RefactorAsync(
                document,
                ifStatement,
                statementsInfo,
                CreateNewIfStatement,
                ifCascadeInfo.Count,
                ifCascadeInfo.EndsWithElse,
                cancellationToken).ConfigureAwait(false);

            return await document.ReplaceNodeAsync(statementsInfo.Parent, newStatementsInfo.Parent, cancellationToken).ConfigureAwait(false);
        }

        private static IfStatementSyntax CreateNewIfStatement(IfStatementSyntax ifStatement)
        {
            IEnumerable<ExpressionStatementSyntax> statements = ifStatement
                .AsCascade()
                .Select(ifOrElse =>
                {
                    StatementSyntax statement = ifOrElse.Statement;

                    if (statement is BlockSyntax block)
                        statement = block.Statements.Last();

                    return (ExpressionStatementSyntax)statement;
                });

            return ifStatement.ReplaceNodes(
                statements,
                (expressionStatement, _) =>
                {
                    var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

                    return ReturnStatement(assignment.Right).WithTriviaFrom(expressionStatement);
                });
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            SwitchStatementSyntax switchStatement,
            CancellationToken cancellationToken)
        {
            StatementListInfo statementsInfo = SyntaxInfo.StatementListInfo(switchStatement);

            StatementListInfo newStatementsInfo = await RefactorAsync(
                document,
                switchStatement,
                statementsInfo,
                CreateNewSwitchStatement,
                switchStatement.Sections.Count,
                switchStatement.Sections.Any(f => f.ContainsDefaultLabel()),
                cancellationToken).ConfigureAwait(false);

            return await document.ReplaceNodeAsync(statementsInfo.Parent, newStatementsInfo.Parent, cancellationToken).ConfigureAwait(false);
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
                var expressionStatement = (ExpressionStatementSyntax)section.GetStatements().LastButOne();

                var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

                section = section.ReplaceNode(expressionStatement, ReturnStatement(assignment.Right).WithTriviaFrom(expressionStatement));

                return section.RemoveStatement(section.GetStatements().Last());
            }
        }

        private static async Task<StatementListInfo> RefactorAsync<TStatement>(
            Document document,
            TStatement statement,
            StatementListInfo statementsInfo,
            Func<TStatement, TStatement> createNewStatement,
            int count,
            bool removeReturnStatement,
            CancellationToken cancellationToken) where TStatement : StatementSyntax
        {
            int index = statementsInfo.IndexOf(statement);

            var returnStatement = (ReturnStatementSyntax)statementsInfo[index + 1];

            ExpressionSyntax expression = returnStatement.Expression;
            ExpressionSyntax newExpression = null;

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ISymbol symbol = semanticModel.GetSymbol(expression, cancellationToken);

            if (symbol.Kind == SymbolKind.Local
                && index > 0)
            {
                var localDeclarationStatement = statementsInfo[index - 1] as LocalDeclarationStatementSyntax;

                if (localDeclarationStatement?.ContainsDiagnostics == false
                    && !localDeclarationStatement.SpanOrTrailingTriviaContainsDirectives()
                    && !statement.GetLeadingTrivia().Any(f => f.IsDirective))
                {
                    SeparatedSyntaxList<VariableDeclaratorSyntax> declarators = localDeclarationStatement.Declaration.Variables;

                    VariableDeclaratorSyntax declarator = declarators.FirstOrDefault(f => semanticModel.GetDeclaredSymbol(f, cancellationToken)?.Equals(symbol) == true);

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

                                returnStatement = (ReturnStatementSyntax)statementsInfo[index + 1];
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
    }
}
