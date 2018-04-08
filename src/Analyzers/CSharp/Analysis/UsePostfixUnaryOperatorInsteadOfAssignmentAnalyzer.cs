// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UsePostfixUnaryOperatorInsteadOfAssignmentAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.UsePostfixUnaryOperatorInsteadOfAssignment,
                    DiagnosticDescriptors.UsePostfixUnaryOperatorInsteadOfAssignmentFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeSimpleAssignmentExpression, SyntaxKind.SimpleAssignmentExpression);
            context.RegisterSyntaxNodeAction(AnalyzeAddAssignmentExpression, SyntaxKind.AddAssignmentExpression);
            context.RegisterSyntaxNodeAction(AnalyzeSubtractAssignmentExpression, SyntaxKind.SubtractAssignmentExpression);
        }

        public static void AnalyzeSimpleAssignmentExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.SpanContainsDirectives())
                return;

            var assignment = (AssignmentExpressionSyntax)context.Node;

            if (assignment.IsParentKind(SyntaxKind.ObjectInitializerExpression))
                return;

            ExpressionSyntax left = assignment.Left;
            ExpressionSyntax right = assignment.Right;

            if (left?.IsMissing != false)
                return;

            if (right?.IsKind(SyntaxKind.AddExpression, SyntaxKind.SubtractExpression) != true)
                return;

            var binaryExpression = (BinaryExpressionSyntax)right;

            ExpressionSyntax binaryLeft = binaryExpression.Left;
            ExpressionSyntax binaryRight = binaryExpression.Right;

            if (binaryLeft?.IsMissing != false)
                return;

            if (binaryRight?.IsNumericLiteralExpression("1") != true)
                return;

            ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(left, context.CancellationToken);

            if (typeSymbol?.SupportsPrefixOrPostfixUnaryOperator() != true)
                return;

            if (!CSharpFactory.AreEquivalent(left, binaryLeft))
                return;

            string operatorText = GetOperatorText(assignment);

            ReportDiagnostic(context, assignment, operatorText);

            context.ReportToken(DiagnosticDescriptors.UsePostfixUnaryOperatorInsteadOfAssignmentFadeOut, assignment.OperatorToken, operatorText);
            context.ReportNode(DiagnosticDescriptors.UsePostfixUnaryOperatorInsteadOfAssignmentFadeOut, binaryLeft, operatorText);
            context.ReportNode(DiagnosticDescriptors.UsePostfixUnaryOperatorInsteadOfAssignmentFadeOut, binaryRight, operatorText);
        }

        public static void AnalyzeAddAssignmentExpression(SyntaxNodeAnalysisContext context)
        {
            AnalyzeAddOrSubtractAssignmentExpression(context);
        }

        public static void AnalyzeSubtractAssignmentExpression(SyntaxNodeAnalysisContext context)
        {
            AnalyzeAddOrSubtractAssignmentExpression(context);
        }

        private static void AnalyzeAddOrSubtractAssignmentExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.SpanContainsDirectives())
                return;

            var assignment = (AssignmentExpressionSyntax)context.Node;

            ExpressionSyntax left = assignment.Left;
            ExpressionSyntax right = assignment.Right;

            if (left?.IsMissing != false)
                return;

            if (right?.IsNumericLiteralExpression("1") != true)
                return;

            ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(left, context.CancellationToken);

            if (typeSymbol?.SupportsPrefixOrPostfixUnaryOperator() != true)
                return;

            string operatorText = GetOperatorText(assignment);

            ReportDiagnostic(context, assignment, operatorText);

            SyntaxToken operatorToken = assignment.OperatorToken;

            context.ReportDiagnostic(DiagnosticDescriptors.UsePostfixUnaryOperatorInsteadOfAssignmentFadeOut, Location.Create(assignment.SyntaxTree, new TextSpan(operatorToken.SpanStart, 1)), operatorText);
            context.ReportNode(DiagnosticDescriptors.UsePostfixUnaryOperatorInsteadOfAssignmentFadeOut, assignment.Right, operatorText);
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, AssignmentExpressionSyntax assignment, string operatorText)
        {
            context.ReportDiagnostic(
                DiagnosticDescriptors.UsePostfixUnaryOperatorInsteadOfAssignment,
                assignment,
                operatorText);
        }

        public static SyntaxKind GetPostfixUnaryOperatorKind(AssignmentExpressionSyntax assignment)
        {
            if (assignment == null)
                throw new ArgumentNullException(nameof(assignment));

            switch (assignment.Kind())
            {
                case SyntaxKind.AddAssignmentExpression:
                    return SyntaxKind.PostIncrementExpression;
                case SyntaxKind.SubtractAssignmentExpression:
                    return SyntaxKind.PostDecrementExpression;
            }

            switch (assignment.Right?.Kind())
            {
                case SyntaxKind.AddExpression:
                    return SyntaxKind.PostIncrementExpression;
                case SyntaxKind.SubtractExpression:
                    return SyntaxKind.PostDecrementExpression;
            }

            Debug.Fail(assignment.Kind().ToString());

            return SyntaxKind.None;
        }

        public static string GetOperatorText(AssignmentExpressionSyntax assignment)
        {
            return GetOperatorText(GetPostfixUnaryOperatorKind(assignment));
        }

        private static string GetOperatorText(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PostIncrementExpression:
                    return "++";
                case SyntaxKind.PostDecrementExpression:
                    return "--";
            }

            Debug.Fail(kind.ToString());

            return "";
        }
    }
}
