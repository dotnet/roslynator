// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MergeLocalDeclarationWithAssignmentRefactoring
    {
        public static void AnalyzeLocalDeclarationStatement(SyntaxNodeAnalysisContext context)
        {
            var localDeclaration = (LocalDeclarationStatementSyntax)context.Node;

            if (!localDeclaration.IsConst
                && !localDeclaration.ContainsDiagnostics
                && !localDeclaration.SpanOrTrailingTriviaContainsDirectives())
            {
                VariableDeclarationSyntax declaration = localDeclaration.Declaration;

                if (declaration != null)
                {
                    SeparatedSyntaxList<VariableDeclaratorSyntax> variables = declaration.Variables;

                    if (variables.Count == 1)
                    {
                        StatementSyntax nextStatement = localDeclaration.NextStatementOrDefault();

                        if (nextStatement?.ContainsDiagnostics == false
                            && nextStatement?.SpanOrLeadingTriviaContainsDirectives() == false)
                        {
                            SimpleAssignmentStatementInfo assignmentInfo = SyntaxInfo.SimpleAssignmentStatementInfo(nextStatement);
                            if (assignmentInfo.Success
                                && assignmentInfo.Left.IsKind(SyntaxKind.IdentifierName))
                            {
                                SemanticModel semanticModel = context.SemanticModel;
                                CancellationToken cancellationToken = context.CancellationToken;

                                LocalInfo localInfo = FindInitializedVariable((IdentifierNameSyntax)assignmentInfo.Left, variables[0], semanticModel, cancellationToken);

                                if (localInfo.IsValid)
                                {
                                    EqualsValueClauseSyntax initializer = localInfo.Declarator.Initializer;
                                    ExpressionSyntax value = initializer?.Value;

                                    if (value == null
                                        || (IsDefaultValue(declaration.Type, value, semanticModel, cancellationToken)
                                            && !IsReferenced(localInfo.Symbol, assignmentInfo.Right, semanticModel, cancellationToken)))
                                    {
                                        context.ReportDiagnostic(DiagnosticDescriptors.MergeLocalDeclarationWithAssignment, localInfo.Declarator.Identifier);

                                        if (value != null)
                                        {
                                            context.ReportNode(DiagnosticDescriptors.MergeLocalDeclarationWithAssignmentFadeOut, initializer);
                                            context.ReportToken(DiagnosticDescriptors.MergeLocalDeclarationWithAssignmentFadeOut, assignmentInfo.AssignmentExpression.OperatorToken);
                                        }

                                        context.ReportToken(DiagnosticDescriptors.MergeLocalDeclarationWithAssignmentFadeOut, localDeclaration.SemicolonToken);
                                        context.ReportNode(DiagnosticDescriptors.MergeLocalDeclarationWithAssignmentFadeOut, assignmentInfo.Left);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static bool IsReferenced(
            ILocalSymbol localSymbol,
            SyntaxNode node,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            foreach (SyntaxNode descendantOrSelf in node.DescendantNodesAndSelf())
            {
                if (descendantOrSelf.IsKind(SyntaxKind.IdentifierName)
                    && semanticModel
                        .GetSymbol((IdentifierNameSyntax)descendantOrSelf, cancellationToken)?
                        .Equals(localSymbol) == true)
                {
                    return true;
                }
            }

            return false;
        }

        private static LocalInfo FindInitializedVariable(
            IdentifierNameSyntax identifierName,
            VariableDeclaratorSyntax variableDeclarator,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            string name = identifierName.Identifier.ValueText;

            ILocalSymbol localSymbol = null;

            if (string.Equals(variableDeclarator.Identifier.ValueText, name, StringComparison.Ordinal))
            {
                if (localSymbol == null)
                {
                    localSymbol = semanticModel.GetSymbol(identifierName, cancellationToken) as ILocalSymbol;

                    if (localSymbol == null)
                        return default(LocalInfo);
                }

                if (localSymbol.Equals(semanticModel.GetDeclaredSymbol(variableDeclarator, cancellationToken)))
                    return new LocalInfo(variableDeclarator, localSymbol);
            }

            return default(LocalInfo);
        }

        private static bool IsDefaultValue(TypeSyntax type, ExpressionSyntax value, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, cancellationToken);

            if (typeSymbol != null)
            {
                return semanticModel.IsDefaultValue(typeSymbol, value, cancellationToken);
            }
            else
            {
                return false;
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            VariableDeclaratorSyntax declarator,
            CancellationToken cancellationToken)
        {
            var declaration = (VariableDeclarationSyntax)declarator.Parent;

            var localDeclaration = (LocalDeclarationStatementSyntax)declaration.Parent;

            StatementsInfo statementsInfo = SyntaxInfo.StatementsInfo(localDeclaration);

            SyntaxList<StatementSyntax> statements = statementsInfo.Statements;

            int index = statements.IndexOf(localDeclaration);

            StatementSyntax nextStatement = statements[index + 1];

            var expressionStatement = (ExpressionStatementSyntax)nextStatement;

            var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

            ExpressionSyntax right = assignment.Right;

            EqualsValueClauseSyntax initializer = declarator.Initializer;

            ExpressionSyntax value = initializer?.Value;

            VariableDeclaratorSyntax newDeclarator = (value != null)
                ? declarator.ReplaceNode(value, right)
                : declarator.WithInitializer(EqualsValueClause(right));

            LocalDeclarationStatementSyntax newLocalDeclaration = localDeclaration.ReplaceNode(declarator, newDeclarator);

            SyntaxTriviaList trailingTrivia = localDeclaration.GetTrailingTrivia();

            if (!trailingTrivia.IsEmptyOrWhitespace())
            {
                newLocalDeclaration = newLocalDeclaration.WithTrailingTrivia(trailingTrivia.Concat(nextStatement.GetTrailingTrivia()));
            }
            else
            {
                newLocalDeclaration = newLocalDeclaration.WithTrailingTrivia(nextStatement.GetTrailingTrivia());
            }

            SyntaxTriviaList leadingTrivia = nextStatement.GetLeadingTrivia();

            if (!leadingTrivia.IsEmptyOrWhitespace())
            {
                newLocalDeclaration = newLocalDeclaration.WithLeadingTrivia(newLocalDeclaration.GetLeadingTrivia().Concat(leadingTrivia));
            }

            SyntaxList<StatementSyntax> newStatements = statements
                .Replace(localDeclaration, newLocalDeclaration)
                .RemoveAt(index + 1);

            return document.ReplaceStatementsAsync(statementsInfo, newStatements, cancellationToken);
        }

        private struct LocalInfo
        {
            public LocalInfo(VariableDeclaratorSyntax declarator, ILocalSymbol symbol)
            {
                Declarator = declarator;
                Symbol = symbol;
            }

            public bool IsValid
            {
                get
                {
                    return Declarator != null
                        && Symbol != null;
                }
            }

            public VariableDeclaratorSyntax Declarator { get; }

            public ILocalSymbol Symbol { get; }
        }
    }
}