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

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceWhileWithForRefactoring
    {
        public const string Title = "Replace while with for";

        public static async Task ComputeRefactoringAsync(RefactoringContext context, StatementListSelection selectedStatements)
        {
            if (!(selectedStatements.Last() is WhileStatementSyntax whileStatement))
                return;

            if (selectedStatements.Count == 1)
            {
                context.RegisterRefactoring(
                    Title,
                    cancellationToken => RefactorAsync(context.Document, whileStatement, cancellationToken));
            }
            else
            {
                SyntaxKind kind = selectedStatements.First().Kind();

                if (kind == SyntaxKind.LocalDeclarationStatement)
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    if (VerifyLocalDeclarationStatements(selectedStatements, semanticModel, context.CancellationToken))
                    {
                        List<LocalDeclarationStatementSyntax> localDeclarations = selectedStatements
                            .Take(selectedStatements.Count - 1)
                            .Cast<LocalDeclarationStatementSyntax>()
                            .ToList();

                        context.RegisterRefactoring(
                            Title,
                            cancellationToken => RefactorAsync(context.Document, whileStatement, localDeclarations, cancellationToken));
                    }
                }
                else if (kind == SyntaxKind.ExpressionStatement)
                {
                    if (VerifyExpressionStatements(selectedStatements))
                    {
                        List<ExpressionStatementSyntax> expressionStatements = selectedStatements
                            .Take(selectedStatements.Count - 1)
                            .Cast<ExpressionStatementSyntax>()
                            .ToList();

                        context.RegisterRefactoring(
                            Title,
                            cancellationToken => RefactorAsync(context.Document, whileStatement, expressionStatements, cancellationToken));
                    }
                }
            }
        }

        private static bool VerifyLocalDeclarationStatements(
            StatementListSelection selectedStatements,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ITypeSymbol typeSymbol = null;

            for (int i = 0; i < selectedStatements.Count - 1; i++)
            {
                StatementSyntax statement = selectedStatements[i];

                if (!(statement is LocalDeclarationStatementSyntax localDeclaration))
                    return false;

                VariableDeclarationSyntax declaration = localDeclaration.Declaration;

                foreach (VariableDeclaratorSyntax variable in declaration.Variables)
                {
                    var symbol = (ILocalSymbol)semanticModel.GetDeclaredSymbol(variable, cancellationToken);

                    if (symbol == null)
                        continue;

                    if (symbol.Type.IsErrorType())
                        continue;

                    if (typeSymbol == null)
                    {
                        typeSymbol = symbol.Type;
                    }
                    else if (!typeSymbol.Equals(symbol.Type))
                    {
                        return false;
                    }

                    SyntaxList<StatementSyntax> statements = selectedStatements.UnderlyingList;

                    for (int j = selectedStatements.LastIndex + 1; j < statements.Count; j++)
                    {
                        foreach (SyntaxNode node in statements[j].DescendantNodes())
                        {
                            if (node.IsKind(SyntaxKind.IdentifierName)
                                && symbol.Equals(semanticModel.GetSymbol(node, cancellationToken)))
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        private static bool VerifyExpressionStatements(StatementListSelection selectedStatements)
        {
            for (int i = 0; i < selectedStatements.Count - 1; i++)
            {
                StatementSyntax statement = selectedStatements[i];

                if (!(statement is ExpressionStatementSyntax expressionStatement))
                    return false;

                if (!CSharpFacts.CanBeInitializerExpressionInForStatement(expressionStatement.Expression.Kind()))
                    return false;
            }

            return true;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            WhileStatementSyntax whileStatement,
            CancellationToken cancellationToken)
        {
            return document.ReplaceNodeAsync(
                whileStatement,
                ConvertWhileToFor(whileStatement),
                cancellationToken);
        }

        private static Task<Document> RefactorAsync(
            Document document,
            WhileStatementSyntax whileStatement,
            List<ExpressionStatementSyntax> expressionStatements,
            CancellationToken cancellationToken)
        {
            SeparatedSyntaxList<ExpressionSyntax> initializers = expressionStatements
                .Select(f => f.Expression.TrimTrivia())
                .ToSeparatedSyntaxList();

            ForStatementSyntax forStatement = ConvertWhileToFor(whileStatement, initializers: initializers);

            return RefactorAsync(document, whileStatement, forStatement, expressionStatements, cancellationToken);
        }

        private static Task<Document> RefactorAsync(
            Document document,
            WhileStatementSyntax whileStatement,
            List<LocalDeclarationStatementSyntax> localDeclarations,
            CancellationToken cancellationToken)
        {
            IEnumerable<VariableDeclarationSyntax> declarations = localDeclarations
                .Select(f => f.Declaration);

            TypeSyntax type = declarations.First().Type.TrimTrivia();

            SeparatedSyntaxList<VariableDeclaratorSyntax> variables = declarations
                .SelectMany(f => f.Variables)
                .Select(f => f.TrimTrivia())
                .ToSeparatedSyntaxList();

            VariableDeclarationSyntax declaration = VariableDeclaration(type, variables);

            ForStatementSyntax forStatement = ConvertWhileToFor(whileStatement, declaration);

            return RefactorAsync(document, whileStatement, forStatement, localDeclarations, cancellationToken);
        }

        private static Task<Document> RefactorAsync<TNode>(
            Document document,
            WhileStatementSyntax whileStatement,
            ForStatementSyntax forStatement,
            List<TNode> list,
            CancellationToken cancellationToken) where TNode : StatementSyntax
        {
            forStatement = forStatement
                .TrimLeadingTrivia()
                .PrependToLeadingTrivia(list[0].GetLeadingTrivia());

            StatementListInfo statementsInfo = SyntaxInfo.StatementListInfo(whileStatement);

            SyntaxList<StatementSyntax> statements = statementsInfo.Statements;

            int index = statements.IndexOf(list[0]);

            IEnumerable<StatementSyntax> newStatements = statements.Take(index)
                .Concat(new ForStatementSyntax[] { forStatement })
                .Concat(statements.Skip(index + list.Count + 1));

            return document.ReplaceStatementsAsync(statementsInfo, newStatements, cancellationToken);
        }

        private static ForStatementSyntax ConvertWhileToFor(
            WhileStatementSyntax whileStatement,
            VariableDeclarationSyntax declaration = default(VariableDeclarationSyntax),
            SeparatedSyntaxList<ExpressionSyntax> initializers = default(SeparatedSyntaxList<ExpressionSyntax>))
        {
            var incrementors = default(SeparatedSyntaxList<ExpressionSyntax>);

            StatementSyntax statement = whileStatement.Statement;

            if (statement is BlockSyntax block)
            {
                incrementors = SeparatedList(GetIncrementors(block));

                if (incrementors.Any())
                {
                    SyntaxList<StatementSyntax> statements = block.Statements;

                    statement = block.WithStatements(List(statements.Take(statements.Count - incrementors.Count)));
                }
            }

            return ForStatement(
                declaration,
                initializers,
                whileStatement.Condition,
                incrementors,
                statement);
        }

        private static IEnumerable<ExpressionSyntax> GetIncrementors(BlockSyntax block)
        {
            SyntaxList<StatementSyntax> statements = block.Statements;

            for (int i = statements.Count - 1; i >= 0; i--)
            {
                if (statements[i] is ExpressionStatementSyntax expressionStatement)
                {
                    ExpressionSyntax expression = expressionStatement.Expression;

                    if (expression != null
                        && CSharpFacts.IsIncrementOrDecrementExpression(expression.Kind()))
                    {
                        yield return expression;
                    }
                    else
                    {
                        yield break;
                    }
                }
                else
                {
                    yield break;
                }
            }
        }
    }
}