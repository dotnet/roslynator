// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
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
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterCompilationStartAction(startContext =>
            {
                if (startContext.IsAnalyzerSuppressed(DiagnosticDescriptors.UseCompoundAssignment))
                    return;

                startContext.RegisterSyntaxNodeAction(AnalyzeSimpleAssignment, SyntaxKind.SimpleAssignmentExpression);
            });
        }

        private static void AnalyzeSimpleAssignment(SyntaxNodeAnalysisContext context)
        {
            var assignmentExpression = (AssignmentExpressionSyntax)context.Node;

            SimpleAssignmentExpressionInfo assignmentInfo = SyntaxInfo.SimpleAssignmentExpressionInfo(assignmentExpression);

            if (!assignmentInfo.Success)
                return;

            if (assignmentExpression.IsParentKind(SyntaxKind.ObjectInitializerExpression))
                return;

            ExpressionSyntax right = assignmentInfo.Right;

            if (!CanBeReplacedWithCompoundAssignment(right.Kind()))
                return;

            BinaryExpressionInfo binaryInfo = SyntaxInfo.BinaryExpressionInfo((BinaryExpressionSyntax)right);

            if (!binaryInfo.Success)
                return;

            if (!CSharpFactory.AreEquivalent(assignmentInfo.Left, binaryInfo.Left))
                return;

            var binaryExpression = (BinaryExpressionSyntax)right;

            context.ReportDiagnostic(DiagnosticDescriptors.UseCompoundAssignment, assignmentExpression, GetCompoundAssignmentOperatorText(binaryExpression));
            context.ReportNode(DiagnosticDescriptors.UseCompoundAssignmentFadeOut, binaryExpression.Left);
        }

        private static bool CanBeReplacedWithCompoundAssignment(SyntaxKind kind)
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
                default:
                    return false;
            }
        }

        internal static string GetCompoundAssignmentOperatorText(BinaryExpressionSyntax binaryExpression)
        {
            SyntaxKind compoundAssignmentKind = CSharpFacts.GetCompoundAssignmentKind(binaryExpression.Kind());

            SyntaxKind compoundAssignmentOperatorKind = CSharpFacts.GetCompoundAssignmentOperatorKind(compoundAssignmentKind);

            return SyntaxFacts.GetText(compoundAssignmentOperatorKind);
        }
    }
}
