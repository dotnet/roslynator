// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseConditionalAccessAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.UseConditionalAccess); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(startContext =>
            {
                INamedTypeSymbol expressionOfTSymbol = startContext.Compilation.GetTypeByMetadataName(MetadataNames.System_Linq_Expressions_Expression_1);

                startContext.RegisterSyntaxNodeAction(nodeContext => AnalyzeIfStatement(nodeContext, expressionOfTSymbol), SyntaxKind.IfStatement);
                startContext.RegisterSyntaxNodeAction(nodeContext => AnalyzeLogicalAndExpression(nodeContext, expressionOfTSymbol), SyntaxKind.LogicalAndExpression);
            });
        }

        public static void AnalyzeIfStatement(SyntaxNodeAnalysisContext context, INamedTypeSymbol expressionOfTSymbol)
        {
            var ifStatement = (IfStatementSyntax)context.Node;

            if (!ifStatement.IsSimpleIf())
                return;

            if (ifStatement.ContainsDiagnostics)
                return;

            if (ifStatement.SpanContainsDirectives())
                return;

            NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(ifStatement.Condition, allowedStyles: NullCheckStyles.NotEqualsToNull);

            ExpressionSyntax expression = nullCheck.Expression;

            if (expression == null)
                return;

            SimpleMemberInvocationStatementInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationStatementInfo(ifStatement.SingleNonBlockStatementOrDefault());

            ExpressionSyntax expression2 = invocationInfo.Expression;

            if (expression2 == null)
                return;

            ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(expression);

            if (typeSymbol == null)
                return;

            if (typeSymbol.IsNullableType())
            {
                if (!expression2.IsKind(SyntaxKind.SimpleMemberAccessExpression))
                    return;

                var memberAccess = (MemberAccessExpressionSyntax)expression2;

                if (!(memberAccess.Name is IdentifierNameSyntax identifierName))
                    return;

                if (!string.Equals(identifierName.Identifier.ValueText, "Value", StringComparison.Ordinal))
                    return;

                expression2 = memberAccess.Expression;
            }

            if (!CSharpFactory.AreEquivalent(expression, expression2))
                return;

            if (ifStatement.IsInExpressionTree(expressionOfTSymbol, context.SemanticModel, context.CancellationToken))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UseConditionalAccess, ifStatement);
        }

        public static void AnalyzeLogicalAndExpression(SyntaxNodeAnalysisContext context, INamedTypeSymbol expressionOfTSymbol)
        {
            var logicalAndExpression = (BinaryExpressionSyntax)context.Node;

            if (logicalAndExpression.ContainsDiagnostics)
                return;

            NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(logicalAndExpression.Left, allowedStyles: NullCheckStyles.NotEqualsToNull);

            ExpressionSyntax expression = nullCheck.Expression;

            if (expression == null)
                return;

            ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(expression, context.CancellationToken);

            if (typeSymbol == null)
                return;

            if (!typeSymbol.IsReferenceTypeOrNullableType())
                return;

            ExpressionSyntax right = logicalAndExpression.Right?.WalkDownParentheses();

            if (right == null)
                return;

            if (!ValidateRightExpression(right, context.SemanticModel, context.CancellationToken))
                return;

            if (RefactoringUtility.ContainsOutArgumentWithLocal(right, context.SemanticModel, context.CancellationToken))
                return;

            ExpressionSyntax expression2 = FindExpressionThatCanBeConditionallyAccessed(expression, right, isNullable: !typeSymbol.IsReferenceType);

            if (expression2?.SpanContainsDirectives() != false)
                return;

            if (logicalAndExpression.IsInExpressionTree(expressionOfTSymbol, context.SemanticModel, context.CancellationToken))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UseConditionalAccess, logicalAndExpression);
        }

        internal static ExpressionSyntax FindExpressionThatCanBeConditionallyAccessed(ExpressionSyntax expressionToFind, ExpressionSyntax expression, bool isNullable = false)
        {
            if (expression.IsKind(SyntaxKind.LogicalNotExpression))
                expression = ((PrefixUnaryExpressionSyntax)expression).Operand;

            SyntaxKind kind = expressionToFind.Kind();

            SyntaxToken firstToken = expression.GetFirstToken();

            int start = firstToken.SpanStart;

            SyntaxNode node = firstToken.Parent;

            while (node?.SpanStart == start)
            {
                if (kind == node.Kind()
                    && node.IsParentKind(SyntaxKind.SimpleMemberAccessExpression, SyntaxKind.ElementAccessExpression)
                    && CSharpFactory.AreEquivalent(expressionToFind, node))
                {
                    if (!isNullable)
                        return (ExpressionSyntax)node;

                    if (node.IsParentKind(SyntaxKind.SimpleMemberAccessExpression)
                        && (((MemberAccessExpressionSyntax)node.Parent).Name is IdentifierNameSyntax identifierName)
                        && string.Equals(identifierName.Identifier.ValueText, "Value", StringComparison.Ordinal)
                        && node.Parent.IsParentKind(SyntaxKind.SimpleMemberAccessExpression, SyntaxKind.ElementAccessExpression))
                    {
                        return (ExpressionSyntax)node.Parent;
                    }

                    return null;
                }

                node = node.Parent;
            }

            return null;
        }

        private static bool ValidateRightExpression(ExpressionSyntax expression, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            switch (expression.Kind())
            {
                case SyntaxKind.LessThanExpression:
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.LessThanOrEqualExpression:
                case SyntaxKind.GreaterThanOrEqualExpression:
                case SyntaxKind.EqualsExpression:
                    {
                        expression = ((BinaryExpressionSyntax)expression)
                            .Right?
                            .WalkDownParentheses();

                        if (expression == null)
                            return false;

                        Optional<object> optional = semanticModel.GetConstantValue(expression, cancellationToken);

                        return optional.HasValue
                            && optional.Value != null;
                    }
                case SyntaxKind.NotEqualsExpression:
                    {
                        return ((BinaryExpressionSyntax)expression)
                            .Right?
                            .WalkDownParentheses()
                            .Kind() == SyntaxKind.NullLiteralExpression;
                    }
                case SyntaxKind.SimpleMemberAccessExpression:
                case SyntaxKind.InvocationExpression:
                case SyntaxKind.ElementAccessExpression:
                case SyntaxKind.LogicalNotExpression:
                case SyntaxKind.IsExpression:
                case SyntaxKind.IsPatternExpression:
                case SyntaxKind.AsExpression:
                case SyntaxKind.LogicalAndExpression:
                    {
                        return true;
                    }
                default:
                    {
                        return false;
                    }
            }
        }
    }
}
