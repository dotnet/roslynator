// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SimplifyBooleanExpressionAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.SimplifyBooleanExpression); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeLogicalAndExpression, SyntaxKind.LogicalAndExpression);
        }

        internal static void AnalyzeLogicalAndExpression(SyntaxNodeAnalysisContext context)
        {
            var logicalAnd = (BinaryExpressionSyntax)context.Node;

            if (logicalAnd.SpanContainsDirectives())
                return;

            BinaryExpressionInfo logicalAndInfo = SyntaxInfo.BinaryExpressionInfo(logicalAnd);

            if (!logicalAndInfo.Success)
                return;

            ExpressionSyntax left = logicalAndInfo.Left;

            if (!IsPropertyOfNullableOfT(left, "HasValue", context.SemanticModel, context.CancellationToken))
                return;

            ExpressionSyntax right = logicalAndInfo.Right;

            switch (right.Kind())
            {
                case SyntaxKind.LogicalNotExpression:
                    {
                        var logicalNot = (PrefixUnaryExpressionSyntax)right;

                        Analyze(context, logicalAnd, left, logicalNot.Operand?.WalkDownParentheses());
                        break;
                    }
                case SyntaxKind.EqualsExpression:
                    {
                        BinaryExpressionInfo equalsExpressionInfo = SyntaxInfo.BinaryExpressionInfo((BinaryExpressionSyntax)right);

                        if (equalsExpressionInfo.Success
                            && equalsExpressionInfo.Right.Kind() == SyntaxKind.FalseLiteralExpression)
                        {
                            Analyze(context, logicalAnd, left, equalsExpressionInfo.Left);
                        }

                        break;
                    }
                case SyntaxKind.SimpleMemberAccessExpression:
                    {
                        Analyze(context, logicalAnd, left, right);
                        break;
                    }
            }
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            BinaryExpressionSyntax logicalAnd,
            ExpressionSyntax expression1,
            ExpressionSyntax expression2)
        {
            if (IsPropertyOfNullableOfT(expression2, "Value", context.SemanticModel, context.CancellationToken))
            {
                expression1 = ((MemberAccessExpressionSyntax)expression1).Expression;
                expression2 = ((MemberAccessExpressionSyntax)expression2).Expression;

                if (expression1 != null
                    && expression2 != null
                    && AreEquivalent(expression1, expression2))
                {
                    context.ReportDiagnostic(DiagnosticDescriptors.SimplifyBooleanExpression, logicalAnd);
                }
            }
        }

        private static bool IsPropertyOfNullableOfT(ExpressionSyntax expression, string name, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (expression?.Kind() == SyntaxKind.SimpleMemberAccessExpression)
            {
                var memberAccessExpression = (MemberAccessExpressionSyntax)expression;

                SimpleNameSyntax simpleName = memberAccessExpression.Name;

                if (simpleName?.Kind() == SyntaxKind.IdentifierName)
                {
                    var identifierName = (IdentifierNameSyntax)simpleName;

                    return string.Equals(identifierName.Identifier.ValueText, name, StringComparison.Ordinal)
                        && SyntaxUtility.IsPropertyOfNullableOfT(expression, name, semanticModel, cancellationToken);
                }
            }

            return false;
        }
    }
}
