// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Analysis
{
    internal static class SimplifyBooleanComparisonAnalysis
    {
        public static void ReportDiagnostic(
            SyntaxNodeAnalysisContext context,
            BinaryExpressionSyntax binaryExpression,
            ExpressionSyntax left,
            ExpressionSyntax right,
            bool fadeOut)
        {
            if (binaryExpression.SpanContainsDirectives())
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.SimplifyBooleanComparison, binaryExpression);

            if (!fadeOut)
                return;

            DiagnosticDescriptor fadeOutDescriptor = DiagnosticDescriptors.SimplifyBooleanComparisonFadeOut;

            context.ReportToken(fadeOutDescriptor, binaryExpression.OperatorToken);

            switch (binaryExpression.Kind())
            {
                case SyntaxKind.EqualsExpression:
                    {
                        if (left.IsKind(SyntaxKind.FalseLiteralExpression))
                        {
                            context.ReportNode(fadeOutDescriptor, left);

                            if (right.IsKind(SyntaxKind.LogicalNotExpression))
                                context.ReportToken(fadeOutDescriptor, ((PrefixUnaryExpressionSyntax)right).OperatorToken);
                        }
                        else if (right.IsKind(SyntaxKind.FalseLiteralExpression))
                        {
                            context.ReportNode(fadeOutDescriptor, right);

                            if (left.IsKind(SyntaxKind.LogicalNotExpression))
                                context.ReportToken(fadeOutDescriptor, ((PrefixUnaryExpressionSyntax)left).OperatorToken);
                        }

                        break;
                    }
                case SyntaxKind.NotEqualsExpression:
                    {
                        if (left.IsKind(SyntaxKind.TrueLiteralExpression))
                        {
                            context.ReportNode(fadeOutDescriptor, left);

                            if (right.IsKind(SyntaxKind.LogicalNotExpression))
                                context.ReportToken(fadeOutDescriptor, ((PrefixUnaryExpressionSyntax)right).OperatorToken);
                        }
                        else if (right.IsKind(SyntaxKind.TrueLiteralExpression))
                        {
                            context.ReportNode(fadeOutDescriptor, right);

                            if (left.IsKind(SyntaxKind.LogicalNotExpression))
                                context.ReportToken(fadeOutDescriptor, ((PrefixUnaryExpressionSyntax)left).OperatorToken);
                        }

                        break;
                    }
            }
        }
    }
}
