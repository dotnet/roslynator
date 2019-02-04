// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis.RemoveRedundantStatement;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class InvertIfRefactoring
    {
        public static readonly string RecursiveRefactoringIdentifier = EquivalenceKey.Join(RefactoringIdentifiers.InvertIf, "Recursive");

        public static void ComputeRefactoring(RefactoringContext context, IfStatementSyntax ifStatement)
        {
            ExpressionSyntax condition = ifStatement.Condition;

            if (condition?.IsMissing != false)
                return;

            StatementSyntax statement = ifStatement.Statement;

            if (statement?.IsMissing != false)
                return;

            Document document = context.Document;

            ElseClauseSyntax elseClause = ifStatement.Else;

            if (elseClause != null)
            {
                if (context.IsRefactoringEnabled(RefactoringIdentifiers.InvertIfElse))
                {
                    StatementSyntax elseStatement = elseClause.Statement;

                    if (elseStatement?.IsMissing == false
                        && !elseStatement.IsKind(SyntaxKind.IfStatement))
                    {
                        context.RegisterRefactoring(
                            "Invert if-else",
                            ct => InvertIfElseAsync(document, ifStatement, ct),
                            RefactoringIdentifiers.InvertIfElse);
                    }
                }
            }
            else if (context.IsRefactoringEnabled(RefactoringIdentifiers.InvertIf))
            {
                InvertIfAnalysis analysis = InvertIfAnalysis.Create(ifStatement, statement);

                if (analysis.Success)
                {
                    context.RegisterRefactoring(
                        "Invert if",
                        ct => InvertIfAsync(document, ifStatement, recursive: false, ct),
                        RefactoringIdentifiers.InvertIf);

                    if (analysis.AnalyzeNextStatement().Success)
                    {
                        context.RegisterRefactoring(
                            "Invert if (recursively)",
                            ct => InvertIfAsync(document, ifStatement, recursive: true, ct),
                            RecursiveRefactoringIdentifier);
                    }
                }
            }
        }

        private static async Task<Document> InvertIfElseAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            ElseClauseSyntax elseClause = ifStatement.Else;
            StatementSyntax whenTrue = ifStatement.Statement;
            StatementSyntax whenFalse = elseClause.Statement;

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            IfStatementSyntax newIfStatement = ifStatement.Update(
                ifKeyword: ifStatement.IfKeyword,
                openParenToken: ifStatement.OpenParenToken,
                condition: SyntaxInverter.LogicallyInvert(ifStatement.Condition, semanticModel, cancellationToken),
                closeParenToken: ifStatement.CloseParenToken,
                statement: whenFalse.WithTriviaFrom(whenTrue),
                @else: elseClause.WithStatement(whenTrue.WithTriviaFrom(whenFalse)));

            newIfStatement = newIfStatement.WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(ifStatement, newIfStatement, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<Document> InvertIfAsync(
            Document document,
            IfStatementSyntax ifStatement,
            bool recursive = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            StatementSyntax statement = ifStatement.Statement;

            StatementListInfo statementsInfo = SyntaxInfo.StatementListInfo(ifStatement);

            SyntaxList<StatementSyntax> statements = statementsInfo.Statements;

            InvertIfAnalysis analysis = InvertIfAnalysis.Create(ifStatement, statement);

            int ifStatementIndex = statements.IndexOf(ifStatement);

            StatementSyntax lastStatement = analysis.LastStatement;

            int lastStatementIndex = statements.IndexOf(lastStatement);

            bool isLastStatementRedundant = IsLastStatementRedundant();

            bool shouldUseElseClause = !CSharpFacts.IsJumpStatement(lastStatement.Kind());

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            SyntaxList<StatementSyntax> newStatements = statements;

            if (!recursive)
            {
                Refactor();
            }
            else
            {
                IfStatementSyntax lastIfStatement = ifStatement;

                InvertIfAnalysis a = analysis.AnalyzeNextStatement();

                do
                {
                    lastIfStatement = a.IfStatement;

                    a = a.AnalyzeNextStatement();

                } while (a.Success);

                int firstLastStatementIndex = lastStatementIndex;

                int index = statements.IndexOf(lastIfStatement);

                int firstIndex = ifStatementIndex;

                while (index >= firstIndex)
                {
                    ifStatementIndex = index;
                    ifStatement = (IfStatementSyntax)statements[ifStatementIndex];
                    statement = ifStatement.Statement;
                    Refactor();
                    lastStatementIndex = firstLastStatementIndex + newStatements.Count - statements.Count;
                    lastStatement = (statement is BlockSyntax block) ? block.Statements.Last() : statement;
                    index--;
                }
            }

            return await document.ReplaceStatementsAsync(statementsInfo, newStatements, cancellationToken).ConfigureAwait(false);

            void Refactor()
            {
                cancellationToken.ThrowIfCancellationRequested();

                SyntaxList<StatementSyntax> nextStatements = newStatements
                    .Skip(ifStatementIndex + 1)
                    .Take(lastStatementIndex - ifStatementIndex)
                    .ToSyntaxList()
                    .TrimTrivia();

                BlockSyntax newStatement;
                SyntaxList<StatementSyntax> newNextStatements;

                if (statement is BlockSyntax block)
                {
                    newStatement = block.WithStatements(nextStatements);
                    newNextStatements = block.Statements;
                }
                else
                {
                    newStatement = Block(nextStatements);
                    newNextStatements = SingletonList(statement);
                }

                if (isLastStatementRedundant)
                    newNextStatements = newNextStatements.RemoveAt(newNextStatements.Count - 1);

                ElseClauseSyntax elseClause = null;

                if (newNextStatements.Any()
                    && shouldUseElseClause)
                {
                    elseClause = ElseClause(Block(newNextStatements));
                    newNextStatements = default;
                }

                IfStatementSyntax newIfStatement = ifStatement.Update(
                    ifKeyword: ifStatement.IfKeyword,
                    openParenToken: ifStatement.OpenParenToken,
                    condition: SyntaxInverter.LogicallyInvert(ifStatement.Condition, semanticModel, cancellationToken),
                    closeParenToken: ifStatement.CloseParenToken,
                    statement: newStatement,
                    @else: elseClause);

                newIfStatement = newIfStatement.WithFormatterAnnotation();

                SyntaxList<StatementSyntax> newNodes = newNextStatements.Insert(0, newIfStatement);

                newStatements = newStatements.ReplaceRange(ifStatementIndex, lastStatementIndex - ifStatementIndex + 1, newNodes);
            }

            bool IsLastStatementRedundant()
            {
                StatementSyntax jumpStatement = analysis.JumpStatement;

                switch (jumpStatement.Kind())
                {
                    case SyntaxKind.ReturnStatement:
                        {
                            if (((ReturnStatementSyntax)jumpStatement).Expression == null
                                && RemoveRedundantStatementAnalysis.IsFixable(lastStatement, SyntaxKind.ReturnStatement))
                            {
                                return true;
                            }

                            break;
                        }
                    case SyntaxKind.ContinueStatement:
                        {
                            if (RemoveRedundantStatementAnalysis.IsFixable(lastStatement, SyntaxKind.ContinueStatement))
                                return true;

                            break;
                        }
                }

                return false;
            }
        }

        private readonly struct InvertIfAnalysis
        {
            public InvertIfAnalysis(
                IfStatementSyntax ifStatement,
                StatementSyntax lastStatement,
                StatementSyntax jumpStatement)
            {
                IfStatement = ifStatement;
                LastStatement = lastStatement;
                JumpStatement = jumpStatement;
            }

            public IfStatementSyntax IfStatement { get; }

            public StatementSyntax NextStatement
            {
                get { return (Success) ? IfStatement.NextStatement() : null; }
            }

            public StatementSyntax LastStatement { get; }

            public StatementSyntax JumpStatement { get; }

            public bool Success
            {
                get { return IfStatement != null; }
            }

            public static InvertIfAnalysis Create(
                IfStatementSyntax ifStatement,
                StatementSyntax statement)
            {
                if (statement is BlockSyntax block)
                {
                    statement = block.Statements.LastOrDefault();

                    if (statement == null)
                        return default;
                }

                if (!statement.IsKind(SyntaxKind.BreakStatement, SyntaxKind.ContinueStatement, SyntaxKind.ReturnStatement))
                    return default;

                if (!ifStatement.TryGetContainingList(out SyntaxList<StatementSyntax> statements))
                    return default;

                StatementSyntax lastStatement = null;

                FindLastStatement();

                if (lastStatement == null)
                    return default;

                return new InvertIfAnalysis(ifStatement, lastStatement, statement);

                void FindLastStatement()
                {
                    int count = statements.Count;

                    int i = statements.IndexOf(ifStatement) + 1;

                    if (i == count)
                        return;

                    StatementSyntax next = statements[i];

                    if (next.IsKind(SyntaxKind.LocalFunctionStatement))
                        return;

                    i++;

                    while (i < count)
                    {
                        if (statements[i].IsKind(SyntaxKind.LocalFunctionStatement))
                        {
                            i++;

                            while (i < count)
                            {
                                if (!statements[i].IsKind(SyntaxKind.LocalFunctionStatement))
                                    return;

                                i++;
                            }

                            break;
                        }

                        lastStatement = statements[i];
                        i++;
                    }

                    lastStatement = lastStatement ?? next;
                }
            }

            public InvertIfAnalysis AnalyzeNextStatement()
            {
                if (!(NextStatement is IfStatementSyntax ifStatement))
                    return default;

                SimpleIfStatementInfo simpleIf = SyntaxInfo.SimpleIfStatementInfo(ifStatement);

                if (!simpleIf.Success)
                    return default;

                return Create(ifStatement, simpleIf.Statement);
            }
        }
    }
}
