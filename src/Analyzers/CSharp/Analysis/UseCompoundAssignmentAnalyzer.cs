// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseCompoundAssignmentAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.UseCompoundAssignment,
                    DiagnosticDescriptors.UseCompoundAssignmentFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterCompilationStartAction(startContext =>
            {
                if (startContext.IsAnalyzerSuppressed(DiagnosticDescriptors.UseCompoundAssignment))
                    return;

                startContext.RegisterSyntaxNodeAction(f => AnalyzeSimpleAssignment(f), SyntaxKind.SimpleAssignmentExpression);

                if (((CSharpCompilation)startContext.Compilation).LanguageVersion >= LanguageVersion.CSharp8)
                    startContext.RegisterSyntaxNodeAction(f => AnalyzeCoalesceExpression(f), SyntaxKind.CoalesceExpression);
            });
        }

        private static void AnalyzeSimpleAssignment(SyntaxNodeAnalysisContext context)
        {
            var assignmentExpression = (AssignmentExpressionSyntax)context.Node;

            SimpleAssignmentExpressionInfo assignmentInfo = SyntaxInfo.SimpleAssignmentExpressionInfo(assignmentExpression);

            if (!assignmentInfo.Success)
                return;

            //TODO: SyntaxKind.WithInitializerExpression
            if (assignmentExpression.Parent is InitializerExpressionSyntax
                && !assignmentExpression.IsParentKind(
                    SyntaxKind.CollectionInitializerExpression,
                    SyntaxKind.ArrayInitializerExpression,
                    SyntaxKind.ComplexElementInitializerExpression))
            {
                return;
            }

            ExpressionSyntax right = assignmentInfo.Right;

            if (!CanBeReplacedWithCompoundAssignment(right.Kind()))
                return;

            BinaryExpressionInfo binaryInfo = SyntaxInfo.BinaryExpressionInfo((BinaryExpressionSyntax)right);

            if (!binaryInfo.Success)
                return;

            if (!CSharpFactory.AreEquivalent(assignmentInfo.Left, binaryInfo.Left))
                return;

            var binaryExpression = (BinaryExpressionSyntax)right;

            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.UseCompoundAssignment, assignmentExpression, GetCompoundAssignmentOperatorText(binaryExpression));
            DiagnosticHelpers.ReportNode(context, DiagnosticDescriptors.UseCompoundAssignmentFadeOut, binaryExpression.Left);

            bool CanBeReplacedWithCompoundAssignment(SyntaxKind kind)
            {
                switch (kind)
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
                    case SyntaxKind.CoalesceExpression:
                        return ((CSharpCompilation)context.Compilation).LanguageVersion >= LanguageVersion.CSharp8;
                    default:
                        return false;
                }
            }
        }

        internal static string GetCompoundAssignmentOperatorText(BinaryExpressionSyntax binaryExpression)
        {
            SyntaxKind compoundAssignmentKind = CSharpFacts.GetCompoundAssignmentKind(binaryExpression.Kind());

            SyntaxKind compoundAssignmentOperatorKind = CSharpFacts.GetCompoundAssignmentOperatorKind(compoundAssignmentKind);

            return SyntaxFacts.GetText(compoundAssignmentOperatorKind);
        }

        private static void AnalyzeCoalesceExpression(SyntaxNodeAnalysisContext context)
        {
            var coalesceExpression = (BinaryExpressionSyntax)context.Node;

            BinaryExpressionInfo binaryExpressionInfo = SyntaxInfo.BinaryExpressionInfo(coalesceExpression, walkDownParentheses: false);

            if (!binaryExpressionInfo.Success)
                return;

            ExpressionSyntax right = binaryExpressionInfo.Right;

            if (!right.IsKind(SyntaxKind.ParenthesizedExpression))
                return;

            var parenthesizedExpression = (ParenthesizedExpressionSyntax)right;

            ExpressionSyntax expression = parenthesizedExpression.Expression;

            if (!expression.IsKind(SyntaxKind.SimpleAssignmentExpression))
                return;

            SimpleAssignmentExpressionInfo assignmentInfo = SyntaxInfo.SimpleAssignmentExpressionInfo((AssignmentExpressionSyntax)expression);

            if (!assignmentInfo.Success)
                return;

            if (!CSharpFactory.AreEquivalent(binaryExpressionInfo.Left, assignmentInfo.Left))
                return;

            DiagnosticHelpers.ReportDiagnostic(context, DiagnosticDescriptors.UseCompoundAssignment, coalesceExpression);

            DiagnosticHelpers.ReportToken(context, DiagnosticDescriptors.UseCompoundAssignmentFadeOut, parenthesizedExpression.OpenParenToken);
            DiagnosticHelpers.ReportNode(context, DiagnosticDescriptors.UseCompoundAssignmentFadeOut, assignmentInfo.Left);
            DiagnosticHelpers.ReportToken(context, DiagnosticDescriptors.UseCompoundAssignmentFadeOut, parenthesizedExpression.CloseParenToken);
        }
    }
}
