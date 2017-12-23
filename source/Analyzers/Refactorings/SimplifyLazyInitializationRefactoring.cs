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

            TextSpan span = TextSpan.FromBounds(ifStatement.SpanStart, returnStatement.Span.End);

            if (body.ContainsDirectives(span))
                return;

            SimpleIfStatementInfo simpleIf = SyntaxInfo.SimpleIfStatementInfo(ifStatement);

            if (!simpleIf.Success)
                return;

            StatementSyntax statement = simpleIf.Statement.SingleNonBlockStatementOrDefault();

            if (statement == null)
                return;

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(simpleIf.Condition, allowedKinds: NullCheckKind.IsNull, semanticModel: semanticModel, cancellationToken: cancellationToken);

            if (!nullCheck.Success)
                return;

            IdentifierNameSyntax identifierName = GetIdentifierName(nullCheck.Expression);

            if (identifierName == null)
                return;

            if (!(semanticModel.GetSymbol(identifierName, cancellationToken) is IFieldSymbol fieldSymbol))
                return;

            SimpleAssignmentStatementInfo assignmentInfo = SyntaxInfo.SimpleAssignmentStatementInfo(statement);

            if (!assignmentInfo.Success)
                return;

            string name = identifierName.Identifier.ValueText;

            IdentifierNameSyntax identifierName2 = GetIdentifierName(assignmentInfo.Left);

            if (!IsBackingField(identifierName2, name, fieldSymbol, semanticModel, cancellationToken))
                return;

            IdentifierNameSyntax identifierName3 = GetIdentifierName(returnStatement.Expression, semanticModel, cancellationToken);

            if (!IsBackingField(identifierName3, name, fieldSymbol, semanticModel, cancellationToken))
                return;

            context.ReportDiagnostic(
                DiagnosticDescriptors.SimplifyLazyInitialization,
                Location.Create(node.SyntaxTree, span));
        }

        private static bool IsBackingField(
            IdentifierNameSyntax identifierName,
            string name,
            IFieldSymbol fieldSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            return identifierName != null
                && string.Equals(identifierName.Identifier.ValueText, name, StringComparison.Ordinal)
                && semanticModel.GetSymbol(identifierName, cancellationToken)?.Equals(fieldSymbol) == true;
        }

        private static IdentifierNameSyntax GetIdentifierName(ExpressionSyntax expression)
        {
            if (expression == null)
                return null;

            if (expression is IdentifierNameSyntax identifierName)
                return identifierName;

            if (expression.Kind() == SyntaxKind.SimpleMemberAccessExpression)
            {
                var memberAccess = (MemberAccessExpressionSyntax)expression;

                if (memberAccess.Expression?.Kind() == SyntaxKind.ThisExpression)
                    return memberAccess.Name as IdentifierNameSyntax;
            }

            return null;
        }

        private static IdentifierNameSyntax GetIdentifierName(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (expression == null)
                return null;

            if (expression is IdentifierNameSyntax identifierName)
                return identifierName;

            if (expression.Kind() != SyntaxKind.SimpleMemberAccessExpression)
                return null;

            var memberAccess = (MemberAccessExpressionSyntax)expression;

            ExpressionSyntax expression2 = memberAccess.Expression;

            switch (expression2?.Kind())
            {
                case SyntaxKind.SimpleMemberAccessExpression:
                    {
                        var memberAccess2 = (MemberAccessExpressionSyntax)expression2;

                        if (memberAccess2.Expression?.Kind() != SyntaxKind.ThisExpression)
                            return null;

                        if (!SyntaxUtility.IsPropertyOfNullableOfT(memberAccess.Name as IdentifierNameSyntax, "Value", semanticModel, cancellationToken))
                            return null;

                        return memberAccess2.Name as IdentifierNameSyntax;
                    }
                case SyntaxKind.ThisExpression:
                    {
                        return memberAccess.Name as IdentifierNameSyntax;
                    }
                case SyntaxKind.IdentifierName:
                    {
                        if (!SyntaxUtility.IsPropertyOfNullableOfT(memberAccess.Name as IdentifierNameSyntax, "Value", semanticModel, cancellationToken))
                            return null;

                        return (IdentifierNameSyntax)expression2;
                    }
            }

            return null;
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
