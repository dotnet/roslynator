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

            if (localDeclaration.ContainsDiagnostics)
                return;

            if (localDeclaration.SpanOrTrailingTriviaContainsDirectives())
                return;

            if (localDeclaration.IsConst)
                return;

            SingleLocalDeclarationStatementInfo localInfo = SyntaxInfo.SingleLocalDeclarationStatementInfo(localDeclaration);

            if (!localInfo.Success)
                return;

            SimpleAssignmentStatementInfo assignmentInfo = SyntaxInfo.SimpleAssignmentStatementInfo(localDeclaration.NextStatementOrDefault());

            if (!assignmentInfo.Success)
                return;

            if (assignmentInfo.Statement.ContainsDiagnostics)
                return;

            if (assignmentInfo.Statement.SpanOrLeadingTriviaContainsDirectives())
                return;

            if (!(assignmentInfo.Left is IdentifierNameSyntax identifierName))
                return;

            string name = identifierName.Identifier.ValueText;

            if (!string.Equals(localInfo.IdentifierText, name, StringComparison.Ordinal))
                return;

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            var localSymbol = semanticModel.GetSymbol(identifierName, cancellationToken) as ILocalSymbol;

            if (localSymbol == null)
                return;

            if (!localSymbol.Equals(semanticModel.GetDeclaredSymbol(localInfo.Declarator, cancellationToken)))
                return;

            EqualsValueClauseSyntax initializer = localInfo.Initializer;
            ExpressionSyntax value = initializer?.Value;

            if (value != null)
            {
                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(localInfo.Type, cancellationToken);

                if (typeSymbol == null)
                    return;

                if (!semanticModel.IsDefaultValue(typeSymbol, value, cancellationToken))
                    return;

                if (IsReferenced(localSymbol, assignmentInfo.Right, semanticModel, cancellationToken))
                    return;
            }

            context.ReportDiagnostic(DiagnosticDescriptors.MergeLocalDeclarationWithAssignment, localInfo.Identifier);

            if (value != null)
            {
                context.ReportNode(DiagnosticDescriptors.MergeLocalDeclarationWithAssignmentFadeOut, initializer);
                context.ReportToken(DiagnosticDescriptors.MergeLocalDeclarationWithAssignmentFadeOut, assignmentInfo.OperatorToken);
            }

            context.ReportToken(DiagnosticDescriptors.MergeLocalDeclarationWithAssignmentFadeOut, localDeclaration.SemicolonToken);
            context.ReportNode(DiagnosticDescriptors.MergeLocalDeclarationWithAssignmentFadeOut, assignmentInfo.Left);
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

        public static Task<Document> RefactorAsync(
            Document document,
            VariableDeclaratorSyntax declarator,
            CancellationToken cancellationToken = default(CancellationToken))
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
                newLocalDeclaration = newLocalDeclaration.WithLeadingTrivia(newLocalDeclaration.GetLeadingTrivia().Concat(leadingTrivia));

            SyntaxList<StatementSyntax> newStatements = statements
                .Replace(localDeclaration, newLocalDeclaration)
                .RemoveAt(index + 1);

            return document.ReplaceStatementsAsync(statementsInfo, newStatements, cancellationToken);
        }
    }
}