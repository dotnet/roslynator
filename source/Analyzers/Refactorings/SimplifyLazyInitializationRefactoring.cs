// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyLazyInitializationRefactoring
    {
        public static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            Analyze(context, methodDeclaration, methodDeclaration.Body);
        }

        public static void AnalyzeGetAccessorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var accessor = (AccessorDeclarationSyntax)context.Node;

            Analyze(context, accessor, accessor.Body);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, SyntaxNode node, BlockSyntax body)
        {
            if (body == null)
                return;

            if (body.ContainsDiagnostics)
                return;

            SyntaxList<StatementSyntax> statements = body.Statements;

            if (statements.Count != 2)
                return;

            if (!(statements[0] is IfStatementSyntax ifStatement))
                return;

            if (!(statements[1] is ReturnStatementSyntax returnStatement))
                return;

            ExpressionSyntax returnExpression = returnStatement.Expression;

            if (returnExpression?.IsKind(SyntaxKind.IdentifierName, SyntaxKind.SimpleMemberAccessExpression) != true)
                return;

            if (ifStatement.SpanOrTrailingTriviaContainsDirectives())
                return;

            if (returnStatement.SpanOrLeadingTriviaContainsDirectives())
                return;

            SimpleIfStatementInfo simpleIf = SyntaxInfo.SimpleIfStatementInfo(ifStatement);

            if (!simpleIf.Success)
                return;

            StatementSyntax statement = simpleIf.Statement.SingleNonBlockStatementOrDefault();

            if (statement == null)
                return;

            SimpleAssignmentStatementInfo assignmentInfo = SyntaxInfo.SimpleAssignmentStatementInfo(statement);

            if (!assignmentInfo.Success)
                return;

            if (!assignmentInfo.Left.IsKind(SyntaxKind.IdentifierName, SyntaxKind.SimpleMemberAccessExpression))
                return;

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(simpleIf.Condition, allowedKinds: NullCheckKind.IsNull, semanticModel: semanticModel, cancellationToken: cancellationToken);

            if (!nullCheck.Success)
                return;

            ExpressionSyntax expression = nullCheck.Expression;

            if (!expression.IsKind(SyntaxKind.IdentifierName, SyntaxKind.SimpleMemberAccessExpression))
                return;

            if (!(semanticModel.GetSymbol(expression, cancellationToken) is IFieldSymbol fieldSymbol))
                return;

            if (!ExpressionEquals(expression, assignmentInfo.Left))
                return;

            if (fieldSymbol.Type.IsConstructedFrom(SpecialType.System_Nullable_T)
                && returnExpression.Kind() == SyntaxKind.SimpleMemberAccessExpression)
            {
                var memberAccessExpression = (MemberAccessExpressionSyntax)returnExpression;

                if (memberAccessExpression.Name is IdentifierNameSyntax identifierName
                    && string.Equals(identifierName.Identifier.ValueText, "Value", StringComparison.Ordinal))
                {
                    returnExpression = memberAccessExpression.Expression;
                }
            }

            if (!ExpressionEquals(expression, returnExpression))
                return;

            context.ReportDiagnostic(
                DiagnosticDescriptors.SimplifyLazyInitialization,
                Location.Create(node.SyntaxTree, TextSpan.FromBounds(ifStatement.SpanStart, returnStatement.Span.End)));
        }

        private static bool ExpressionEquals(ExpressionSyntax expression1, ExpressionSyntax expression2)
        {
            SyntaxKind kind = expression1.Kind();

            if (kind == expression2.Kind())
            {
                if (kind == SyntaxKind.IdentifierName)
                {
                    return IdentifierNameEquals((IdentifierNameSyntax)expression1, (IdentifierNameSyntax)expression2);
                }
                else if (kind == SyntaxKind.SimpleMemberAccessExpression)
                {
                    var memberAccessExpression1 = (MemberAccessExpressionSyntax)expression1;
                    var memberAccessExpression2 = (MemberAccessExpressionSyntax)expression2;

                    return IdentifierNameEquals(memberAccessExpression1.Name as IdentifierNameSyntax, memberAccessExpression2.Name as IdentifierNameSyntax)
                        && IdentifierNameEquals(memberAccessExpression1.Expression as IdentifierNameSyntax, memberAccessExpression2.Expression as IdentifierNameSyntax);
                }
            }

            return false;
        }

        private static bool IdentifierNameEquals(IdentifierNameSyntax identifierName1, IdentifierNameSyntax identifierName2)
        {
            return string.Equals(identifierName1?.Identifier.ValueText, identifierName2?.Identifier.ValueText, StringComparison.Ordinal);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            BlockSyntax block,
            CancellationToken cancellationToken)
        {
            SyntaxList<StatementSyntax> statements = block.Statements;

            var ifStatement = (IfStatementSyntax)statements[0];

            var returnStatement = (ReturnStatementSyntax)statements[1];

            var expressionStatement = (ExpressionStatementSyntax)ifStatement.GetSingleStatementOrDefault();

            var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

            ExpressionSyntax expression = returnStatement.Expression;

            IdentifierNameSyntax valueName = null;

            if (expression.Kind() == SyntaxKind.SimpleMemberAccessExpression)
            {
                var memberAccess = (MemberAccessExpressionSyntax)expression;

                if ((memberAccess.Name is IdentifierNameSyntax identifierName)
                    && string.Equals(identifierName.Identifier.ValueText, "Value", StringComparison.Ordinal))
                {
                    expression = memberAccess.Expression;
                    valueName = identifierName;
                }
            }

            expression = expression.WithoutTrivia();

            ExpressionSyntax right = SimpleAssignmentExpression(expression, assignment.Right.WithoutTrivia()).Parenthesize();

            if (valueName != null)
                right = SimpleMemberAccessExpression(right.Parenthesize(), valueName);

            BinaryExpressionSyntax coalesceExpression = CoalesceExpression(expression, right);

            ReturnStatementSyntax newReturnStatement = returnStatement
                .WithExpression(coalesceExpression)
                .WithLeadingTrivia(ifStatement.GetLeadingTrivia());

            SyntaxList<StatementSyntax> newStatements = statements
                .Replace(returnStatement, newReturnStatement)
                .RemoveAt(0);

            BlockSyntax newBlock = block.WithStatements(newStatements);

            return document.ReplaceNodeAsync(block, newBlock, cancellationToken);
        }
    }
}
