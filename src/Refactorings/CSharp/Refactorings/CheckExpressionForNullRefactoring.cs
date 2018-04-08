// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CheckExpressionForNullRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, ExpressionSyntax expression)
        {
            expression = expression.WalkUpParentheses();

            var assignmentExpression = expression.Parent as AssignmentExpressionSyntax;

            if (expression != assignmentExpression?.Left)
                return;

            SimpleAssignmentStatementInfo assignmentInfo = SyntaxInfo.SimpleAssignmentStatementInfo(assignmentExpression);

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            if (!CanRefactor(assignmentInfo, expression, semanticModel, context.CancellationToken))
                return;

            RegisterRefactoring(context, expression, assignmentInfo.Statement);
        }

        private static bool CanRefactor(
            SimpleAssignmentStatementInfo assignmentInfo,
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (!assignmentInfo.Success)
                return false;

            if (assignmentInfo.Right.IsKind(SyntaxKind.NullLiteralExpression, SyntaxKind.DefaultExpression))
                return false;

            if (CannotBeEqualToNull(assignmentInfo.Right))
                return false;

            if (NullCheckExists(expression, assignmentInfo.Statement))
                return false;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, cancellationToken);

            if (typeSymbol == null)
                return false;

            return typeSymbol.IsReferenceTypeOrNullableType();
        }

        internal static async Task ComputeRefactoringAsync(RefactoringContext context, VariableDeclarationSyntax variableDeclaration)
        {
            SingleLocalDeclarationStatementInfo localInfo = SyntaxInfo.SingleLocalDeclarationStatementInfo(variableDeclaration);

            if (!context.Span.IsContainedInSpanOrBetweenSpans(localInfo.Identifier))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            if (!CanRefactor(localInfo, semanticModel, context.CancellationToken))
                return;

            RegisterRefactoring(context, IdentifierName(localInfo.Identifier), localInfo.Statement);
        }

        private static bool CanRefactor(SingleLocalDeclarationStatementInfo localInfo, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (!localInfo.Success)
                return false;

            ExpressionSyntax value = localInfo.Value;

            if (value?.IsKind(SyntaxKind.NullLiteralExpression, SyntaxKind.DefaultExpression) != false)
                return false;

            if (CannotBeEqualToNull(value))
                return false;

            IdentifierNameSyntax identifierName = IdentifierName(localInfo.Identifier);

            if (NullCheckExists(identifierName, localInfo.Statement))
                return false;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(localInfo.Type, cancellationToken);

            if (typeSymbol == null)
                return false;

            return typeSymbol.IsReferenceTypeOrNullableType();
        }

        internal static async Task ComputeRefactoringAsync(RefactoringContext context, StatementListSelection selectedStatements)
        {
            if (selectedStatements.Count <= 1)
                return;

            StatementSyntax statement = selectedStatements.First();

            SyntaxKind kind = statement.Kind();

            if (kind == SyntaxKind.LocalDeclarationStatement)
            {
                var localDeclaration = (LocalDeclarationStatementSyntax)statement;

                SingleLocalDeclarationStatementInfo localInfo = SyntaxInfo.SingleLocalDeclarationStatementInfo(localDeclaration);

                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                if (!CanRefactor(localInfo, semanticModel, context.CancellationToken))
                    return;

                RegisterRefactoring(context, IdentifierName(localInfo.Identifier), localDeclaration, selectedStatements.Count - 1);
            }
            else if (kind == SyntaxKind.ExpressionStatement)
            {
                var expressionStatement = (ExpressionStatementSyntax)statement;

                SimpleAssignmentStatementInfo assignmentInfo = SyntaxInfo.SimpleAssignmentStatementInfo(expressionStatement);

                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                if (!CanRefactor(assignmentInfo, assignmentInfo.Left, semanticModel, context.CancellationToken))
                    return;

                RegisterRefactoring(context, assignmentInfo.Left, expressionStatement, selectedStatements.Count - 1);
            }
        }

        private static bool CannotBeEqualToNull(ExpressionSyntax expression)
        {
            switch (expression.Kind())
            {
                case SyntaxKind.AnonymousObjectCreationExpression:
                case SyntaxKind.ArrayCreationExpression:
                case SyntaxKind.ImplicitArrayCreationExpression:
                case SyntaxKind.ObjectCreationExpression:
                case SyntaxKind.ThisExpression:
                case SyntaxKind.CharacterLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                case SyntaxKind.NumericLiteralExpression:
                case SyntaxKind.StringLiteralExpression:
                case SyntaxKind.TrueLiteralExpression:
                    return true;
                default:
                    return false;
            }
        }

        private static void RegisterRefactoring(RefactoringContext context, ExpressionSyntax expression, StatementSyntax statement)
        {
            context.RegisterRefactoring(
                GetTitle(expression),
                cancellationToken => RefactorAsync(context.Document, expression, statement, cancellationToken));
        }

        private static void RegisterRefactoring(RefactoringContext context, ExpressionSyntax expression, StatementSyntax statement, int statementCount)
        {
            context.RegisterRefactoring(
                GetTitle(expression),
                cancellationToken => RefactorAsync(context.Document, expression, statement, statementCount, cancellationToken));
        }

        private static string GetTitle(ExpressionSyntax expression)
        {
            return $"Check '{expression}' for null";
        }

        private static bool NullCheckExists(ExpressionSyntax expression, StatementSyntax statement)
        {
            StatementListInfo statementsInfo = SyntaxInfo.StatementListInfo(statement);

            if (!statementsInfo.Success)
                return false;

            SyntaxList<StatementSyntax> statements = statementsInfo.Statements;

            int index = statements.IndexOf(statement);

            if (index >= statements.Count - 1)
                return false;

            StatementSyntax nextStatement = statements[index + 1];

            if (!(nextStatement is IfStatementSyntax ifStatement))
                return false;

            NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(ifStatement.Condition, NullCheckStyles.NotEqualsToNull);

            if (!nullCheck.Success)
                return false;

            return CSharpFactory.AreEquivalent(expression, nullCheck.Expression);
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            StatementSyntax statement,
            CancellationToken cancellationToken)
        {
            if (statement.IsEmbedded())
            {
                return await document.ReplaceNodeAsync(statement, Block(statement, CreateNullCheck(expression)), cancellationToken).ConfigureAwait(false);
            }
            else
            {
                StatementListInfo statementsInfo = SyntaxInfo.StatementListInfo(statement);
                SyntaxList<StatementSyntax> statements = statementsInfo.Statements;

                int statementIndex = statements.IndexOf(statement);

                SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                ISymbol symbol = (statement is LocalDeclarationStatementSyntax localDeclaration)
                    ? semanticModel.GetDeclaredSymbol(localDeclaration.Declaration.Variables.First(), cancellationToken)
                    : semanticModel.GetSymbol(expression, cancellationToken);

                int lastStatementIndex = IncludeAllReferencesOfSymbol(symbol, expression.Kind(), statements, statementIndex + 1, semanticModel, cancellationToken);

                if (lastStatementIndex != -1)
                {
                    if (lastStatementIndex < statements.Count - 1)
                        lastStatementIndex = IncludeAllReferencesOfVariablesDeclared(statements, statementIndex + 1, lastStatementIndex, semanticModel, cancellationToken);

                    return await RefactorAsync(
                        document,
                        expression,
                        statements,
                        statementsInfo,
                        statementIndex,
                        lastStatementIndex,
                        cancellationToken).ConfigureAwait(false);
                }
            }

            return await document.InsertNodeAfterAsync(statement, CreateNullCheck(expression), cancellationToken).ConfigureAwait(false);
        }

        private static Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            StatementSyntax statement,
            int statementCount,
            CancellationToken cancellationToken)
        {
            StatementListInfo statementsInfo = SyntaxInfo.StatementListInfo(statement);

            SyntaxList<StatementSyntax> statements = statementsInfo.Statements;

            int statementIndex = statements.IndexOf(statement);

            return RefactorAsync(
                document,
                expression,
                statements,
                statementsInfo,
                statementIndex,
                statementIndex + statementCount,
                cancellationToken);
        }

        private static Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            SyntaxList<StatementSyntax> statements,
            StatementListInfo statementsInfo,
            int statementIndex,
            int lastStatementIndex,
            CancellationToken cancellationToken)
        {
            IEnumerable<StatementSyntax> blockStatements = statements
                .Skip(statementIndex + 1)
                .Take(lastStatementIndex - statementIndex);

            IfStatementSyntax ifStatement = CreateNullCheck(expression, List(blockStatements));

            if (lastStatementIndex < statements.Count - 1)
                ifStatement = ifStatement.AppendToTrailingTrivia(NewLine());

            IEnumerable<StatementSyntax> newStatements = statements.Take(statementIndex + 1)
                .Concat(new IfStatementSyntax[] { ifStatement })
                .Concat(statements.Skip(lastStatementIndex + 1));

            return document.ReplaceStatementsAsync(statementsInfo, newStatements, cancellationToken);
        }

        private static IfStatementSyntax CreateNullCheck(ExpressionSyntax expression, SyntaxList<StatementSyntax> statements = default(SyntaxList<StatementSyntax>))
        {
            SyntaxToken openBrace = (statements.Any())
                ? OpenBraceToken()
                : Token(default(SyntaxTriviaList), SyntaxKind.OpenBraceToken, TriviaList(NewLine()));

            SyntaxToken closeBrace = (statements.Any())
                ? CloseBraceToken()
                : Token(TriviaList(NewLine()), SyntaxKind.CloseBraceToken, default(SyntaxTriviaList));

            IfStatementSyntax ifStatement = IfStatement(
                NotEqualsExpression(expression.WithoutTrivia(), NullLiteralExpression()),
                Block(openBrace, statements, closeBrace));

            return ifStatement.WithFormatterAnnotation();
        }

        private static int IncludeAllReferencesOfSymbol(
            ISymbol symbol,
            SyntaxKind kind,
            SyntaxList<StatementSyntax> statements,
            int lastStatementIndex,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            for (int i = statements.Count - 1; i >= lastStatementIndex; i--)
            {
                foreach (SyntaxNode node in statements[i].DescendantNodes())
                {
                    if (node.IsKind(kind))
                    {
                        ISymbol symbol2 = semanticModel.GetSymbol(node, cancellationToken);

                        if (symbol.Equals(symbol2))
                            return i;
                    }
                }
            }

            return -1;
        }

        private static int IncludeAllReferencesOfVariablesDeclared(
            SyntaxList<StatementSyntax> statements,
            int statementIndex,
            int lastStatementIndex,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            for (int i = statementIndex; i <= lastStatementIndex; i++)
            {
                if (statements[i] is LocalDeclarationStatementSyntax localDeclaration)
                {
                    VariableDeclarationSyntax declaration = localDeclaration.Declaration;

                    if (declaration != null)
                    {
                        foreach (VariableDeclaratorSyntax variable in declaration.Variables)
                        {
                            ISymbol symbol = semanticModel.GetDeclaredSymbol(variable, cancellationToken);

                            if (symbol != null)
                            {
                                int index = IncludeAllReferencesOfSymbol(symbol, SyntaxKind.IdentifierName, statements, i + 1, semanticModel, cancellationToken);

                                if (index > lastStatementIndex)
                                    lastStatementIndex = index;
                            }
                        }
                    }
                }
            }

            return lastStatementIndex;
        }
    }
}
