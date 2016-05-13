// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AssignmentExpressionDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.SimplifyAssignmentExpression,
                    DiagnosticDescriptors.SimplifyAssignmentExpressionFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeSimpleAssignment(f), SyntaxKind.SimpleAssignmentExpression);
        }

        private void AnalyzeSimpleAssignment(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var assignment = (AssignmentExpressionSyntax)context.Node;

            if (assignment.Left?.IsMissing == false
                && assignment.Right?.IsMissing == false
                && IsBinaryExpression(assignment.Right))
            {
                var binaryExpression = (BinaryExpressionSyntax)assignment.Right;

                if (binaryExpression.Left?.IsMissing == false
                    && binaryExpression.Right?.IsMissing == false
                    && AreExpressionsEqual(assignment.Left, binaryExpression.Left, context))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.SimplifyAssignmentExpression,
                        assignment.GetLocation());

                    DiagnosticHelper.FadeOutNode(context, binaryExpression.Left, DiagnosticDescriptors.SimplifyAssignmentExpressionFadeOut);
                }
            }
        }

        private static bool AreExpressionsEqual(
            ExpressionSyntax expression,
            ExpressionSyntax expression2,
            SyntaxNodeAnalysisContext context)
        {
            if (expression != null && expression2 != null)
            {
                if (expression.IsKind(SyntaxKind.IdentifierName)
                    && expression2.IsKind(SyntaxKind.IdentifierName))
                {
                    ISymbol symbol = context.SemanticModel.GetSymbolInfo(expression, context.CancellationToken).Symbol;

                    if (symbol != null && symbol.Kind != SymbolKind.ErrorType)
                    {
                        ISymbol symbol2 = context.SemanticModel.GetSymbolInfo(expression2, context.CancellationToken).Symbol;

                        return symbol.Equals(symbol2);
                    }
                }
                else if (expression.IsKind(SyntaxKind.ElementAccessExpression)
                    && expression2.IsKind(SyntaxKind.ElementAccessExpression))
                {
                    var elementAccess = (ElementAccessExpressionSyntax)expression;
                    var elementAccess2 = (ElementAccessExpressionSyntax)expression2;

                    return AreExpressionsEqual(elementAccess.Expression, elementAccess2.Expression, context)
                        && AreArgumentsEqual(elementAccess, elementAccess2, context);
                }
            }

            return false;
        }

        private static bool AreArgumentsEqual(ElementAccessExpressionSyntax elementAccess, ElementAccessExpressionSyntax elementAccess2, SyntaxNodeAnalysisContext context)
        {
            ExpressionSyntax expression = GetExpression(elementAccess);

            if (expression != null)
            {
                ExpressionSyntax expression2 = GetExpression(elementAccess2);

                if (expression2 != null)
                {
                    if (expression.IsKind(SyntaxKind.IdentifierName))
                    {
                        return AreExpressionsEqual(expression, expression2, context);
                    }
                    else if (expression2 is LiteralExpressionSyntax)
                    {
                        SyntaxToken token = ((LiteralExpressionSyntax)expression).Token;
                        SyntaxToken token2 = ((LiteralExpressionSyntax)expression2).Token;

                        if (!token.IsMissing
                            && (token.Kind() == token2.Kind()))
                        {
                            return token.Value.Equals(token2.Value);
                        }
                    }
                }
            }

            return false;
        }

        private static ExpressionSyntax GetExpression(ElementAccessExpressionSyntax elementAccess)
        {
            if (elementAccess.ArgumentList?.Arguments.Count == 1)
            {
                ArgumentSyntax argument = elementAccess.ArgumentList.Arguments[0];

                if (argument.NameColon == null
                    && argument.Expression != null
                    && (argument.Expression.IsKind(SyntaxKind.IdentifierName) || argument.Expression is LiteralExpressionSyntax))
                {
                    return argument.Expression;
                }
            }

            return null;
        }

        private static bool IsBinaryExpression(ExpressionSyntax expression)
        {
            switch (expression.Kind())
            {
                case SyntaxKind.AddExpression:
                case SyntaxKind.SubtractExpression:
                case SyntaxKind.MultiplyExpression:
                case SyntaxKind.DivideExpression:
                case SyntaxKind.ModuloExpression:
                case SyntaxKind.BitwiseAndExpression:
                case SyntaxKind.ExclusiveOrExpression:
                case SyntaxKind.BitwiseOrExpression:
                case SyntaxKind.LeftShiftExpression:
                case SyntaxKind.RightShiftExpression:
                    return true;
                default:
                    return false;
            }
        }
    }
}
