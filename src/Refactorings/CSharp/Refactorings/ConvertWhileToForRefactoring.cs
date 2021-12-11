// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;
using Roslynator.CSharp.SyntaxWalkers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ConvertWhileToForRefactoring
    {
        public const string Title = "Convert to 'for'";

        public static async Task ComputeRefactoringAsync(RefactoringContext context, StatementListSelection selectedStatements)
        {
            if (selectedStatements.Last() is not WhileStatementSyntax whileStatement)
                return;

            if (selectedStatements.Count == 1)
            {
                context.RegisterRefactoring(
                    Title,
                    ct => RefactorAsync(context.Document, whileStatement, ct),
                    RefactoringDescriptors.ConvertWhileToFor);
            }
            else
            {
                SyntaxKind kind = selectedStatements.First().Kind();

                if (kind == SyntaxKind.LocalDeclarationStatement)
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    if (FindLocalDeclarationStatementIndex(
                        whileStatement,
                        selectedStatements.UnderlyingList,
                        selectedStatements.FirstIndex,
                        selectedStatements.Count,
                        mustBeReferencedInsideWhileStatement: false,
                        semanticModel: semanticModel,
                        cancellationToken: context.CancellationToken) == selectedStatements.FirstIndex)
                    {
                        List<LocalDeclarationStatementSyntax> localDeclarations = selectedStatements
                            .Take(selectedStatements.Count - 1)
                            .Cast<LocalDeclarationStatementSyntax>()
                            .ToList();

                        context.RegisterRefactoring(
                            Title,
                            ct => RefactorAsync(context.Document, whileStatement, localDeclarations, ct),
                            RefactoringDescriptors.ConvertWhileToFor);
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
                            ct => RefactorAsync(context.Document, whileStatement, expressionStatements, ct),
                            RefactoringDescriptors.ConvertWhileToFor);
                    }
                }
            }
        }

        private static int FindLocalDeclarationStatementIndex(
            WhileStatementSyntax whileStatement,
            SyntaxList<StatementSyntax> statements,
            int startIndex,
            int count,
            bool mustBeReferencedInsideWhileStatement,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default)
        {
            int resultIndex = -1;
            int whileStatementIndex = -1;
            ITypeSymbol typeSymbol = null;

            for (int i = count - 1; i >= startIndex; i--)
            {
                StatementSyntax statement = statements[i];

                if (statement is not LocalDeclarationStatementSyntax localDeclaration)
                    return resultIndex;

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
                    else if (!SymbolEqualityComparer.Default.Equals(typeSymbol, symbol.Type))
                    {
                        return resultIndex;
                    }

                    ContainsLocalOrParameterReferenceWalker walker = null;

                    try
                    {
                        walker = ContainsLocalOrParameterReferenceWalker.GetInstance(symbol, semanticModel, cancellationToken);

                        if (mustBeReferencedInsideWhileStatement)
                        {
                            walker.VisitWhileStatement(whileStatement);

                            if (!walker.Result)
                            {
                                ContainsLocalOrParameterReferenceWalker.Free(walker);
                                return resultIndex;
                            }
                        }

                        walker.Result = false;

                        if (whileStatementIndex == -1)
                            whileStatementIndex = statements.IndexOf(whileStatement);

                        walker.VisitList(statements, whileStatementIndex + 1);

                        if (walker.Result)
                            return resultIndex;
                    }
                    finally
                    {
                        if (walker != null)
                            ContainsLocalOrParameterReferenceWalker.Free(walker);
                    }

                    resultIndex = i;
                }
            }

            return resultIndex;
        }

        private static bool VerifyExpressionStatements(StatementListSelection selectedStatements)
        {
            for (int i = 0; i < selectedStatements.Count - 1; i++)
            {
                StatementSyntax statement = selectedStatements[i];

                if (statement is not ExpressionStatementSyntax expressionStatement)
                    return false;

                if (!CSharpFacts.CanBeInitializerExpressionInForStatement(expressionStatement.Expression.Kind()))
                    return false;
            }

            return true;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            WhileStatementSyntax whileStatement,
            CancellationToken cancellationToken)
        {
            StatementListInfo statementsInfo = SyntaxInfo.StatementListInfo(whileStatement);

            if (statementsInfo.Success)
            {
                SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                int index = FindLocalDeclarationStatementIndex(
                    whileStatement,
                    statementsInfo.Statements,
                    startIndex: 0,
                    count: statementsInfo.IndexOf(whileStatement),
                    mustBeReferencedInsideWhileStatement: true,
                    semanticModel: semanticModel,
                    cancellationToken: cancellationToken);

                if (index >= 0)
                {
                    List<LocalDeclarationStatementSyntax> localDeclarations = statementsInfo
                        .Statements
                        .Skip(index)
                        .Take(statementsInfo.IndexOf(whileStatement) - index)
                        .Cast<LocalDeclarationStatementSyntax>()
                        .ToList();

                    return await RefactorAsync(document, whileStatement, localDeclarations, cancellationToken).ConfigureAwait(false);
                }
            }

            return await document.ReplaceNodeAsync(
                whileStatement,
                SyntaxRefactorings.ConvertWhileStatementToForStatement(whileStatement),
                cancellationToken)
                .ConfigureAwait(false);
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

            ForStatementSyntax forStatement = SyntaxRefactorings.ConvertWhileStatementToForStatement(whileStatement, initializers: initializers);

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

            ForStatementSyntax forStatement = SyntaxRefactorings.ConvertWhileStatementToForStatement(whileStatement, declaration);

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
                .WithFormatterAnnotation()
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
    }
}
