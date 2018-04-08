// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class InlineLocalVariableRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            LocalDeclarationStatementSyntax localDeclaration,
            CancellationToken cancellationToken)
        {
            StatementListInfo statementsInfo = SyntaxInfo.StatementListInfo(localDeclaration);

            int index = statementsInfo.Statements.IndexOf(localDeclaration);

            StatementSyntax nextStatement = statementsInfo.Statements[index + 1];

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ExpressionSyntax value = GetExpressionToInline(localDeclaration, semanticModel, cancellationToken);

            StatementSyntax newStatement = GetStatementWithInlinedExpression(nextStatement, value);

            SyntaxTriviaList leadingTrivia = localDeclaration.GetLeadingTrivia();

            IEnumerable<SyntaxTrivia> trivia = statementsInfo
                .Parent
                .DescendantTrivia(TextSpan.FromBounds(localDeclaration.SpanStart, nextStatement.SpanStart));

            if (!trivia.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                newStatement = newStatement.WithLeadingTrivia(leadingTrivia.Concat(trivia));
            }
            else
            {
                newStatement = newStatement.WithLeadingTrivia(leadingTrivia);
            }

            SyntaxList<StatementSyntax> newStatements = statementsInfo.Statements
                .Replace(nextStatement, newStatement)
                .RemoveAt(index);

            return await document.ReplaceStatementsAsync(statementsInfo, newStatements, cancellationToken).ConfigureAwait(false);
        }

        private static ExpressionSyntax GetExpressionToInline(LocalDeclarationStatementSyntax localDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            VariableDeclarationSyntax variableDeclaration = localDeclaration.Declaration;

            ExpressionSyntax expression = variableDeclaration
                .Variables
                .First()
                .Initializer
                .Value;

            if (expression.IsKind(SyntaxKind.ArrayInitializerExpression))
            {
                expression = SyntaxFactory.ArrayCreationExpression(
                    (ArrayTypeSyntax)variableDeclaration.Type.WithoutTrivia(),
                    (InitializerExpressionSyntax)expression);

                return expression.WithFormatterAnnotation();
            }
            else
            {
                expression = expression.Parenthesize();

                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(variableDeclaration.Type, cancellationToken);

                if (typeSymbol.SupportsExplicitDeclaration())
                {
                    TypeSyntax type = typeSymbol.ToMinimalTypeSyntax(semanticModel, localDeclaration.SpanStart);

                    expression = SyntaxFactory.CastExpression(type, expression).WithSimplifierAnnotation();
                }

                return expression;
            }
        }

        private static StatementSyntax GetStatementWithInlinedExpression(StatementSyntax statement, ExpressionSyntax expression)
        {
            switch (statement.Kind())
            {
                case SyntaxKind.ExpressionStatement:
                    {
                        var expressionStatement = (ExpressionStatementSyntax)statement;

                        var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

                        AssignmentExpressionSyntax newAssignment = assignment.WithRight(expression.WithTriviaFrom(assignment.Right));

                        return expressionStatement.WithExpression(newAssignment);
                    }
                case SyntaxKind.LocalDeclarationStatement:
                    {
                        var localDeclaration = (LocalDeclarationStatementSyntax)statement;

                        ExpressionSyntax value = localDeclaration
                            .Declaration
                            .Variables[0]
                            .Initializer
                            .Value;

                        return statement.ReplaceNode(value, expression.WithTriviaFrom(value));
                    }
                case SyntaxKind.ReturnStatement:
                    {
                        var returnStatement = (ReturnStatementSyntax)statement;

                        return returnStatement.WithExpression(expression.WithTriviaFrom(returnStatement.Expression));
                    }
                case SyntaxKind.ForEachStatement:
                    {
                        var forEachStatement = (ForEachStatementSyntax)statement;

                        return forEachStatement.WithExpression(expression.WithTriviaFrom(forEachStatement.Expression));
                    }
                case SyntaxKind.SwitchStatement:
                    {
                        var switchStatement = (SwitchStatementSyntax)statement;

                        return switchStatement.WithExpression(expression.WithTriviaFrom(switchStatement.Expression));
                    }
                default:
                    {
                        Debug.Fail("");
                        return statement;
                    }
            }
        }
    }
}
