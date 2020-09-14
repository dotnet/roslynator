// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Roslynator.CSharp.Analysis.ConvertHasFlagCallToBitwiseOperationAnalysis;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConvertBitwiseOperationToHasFlagCallAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.ConvertHasFlagCallToBitwiseOperationOrViceVersa); }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterCompilationStartAction(startContext =>
            {
                if (!startContext.IsAnalyzerSuppressed(AnalyzerOptions.ConvertBitwiseOperationToHasFlagCall))
                {
                    startContext.RegisterSyntaxNodeAction(f => AnalyzeBitwiseAndExpression(f), SyntaxKind.BitwiseAndExpression);
                }
            });
        }

        private static void AnalyzeBitwiseAndExpression(SyntaxNodeAnalysisContext context)
        {
            var bitwiseAnd = (BinaryExpressionSyntax)context.Node;

            ExpressionSyntax expression = bitwiseAnd.WalkUpParentheses();

            if (!expression.IsParentKind(SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression))
                return;

            var equalsOrNotEquals = (BinaryExpressionSyntax)expression.Parent;

            ExpressionSyntax otherExpression = (ReferenceEquals(equalsOrNotEquals.Left, expression))
                ? equalsOrNotEquals.Right
                : equalsOrNotEquals.Left;

            if (!otherExpression.WalkDownParentheses().IsNumericLiteralExpression("0"))
                return;

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;
            IMethodSymbol methodSymbol = semanticModel.GetMethodSymbol(bitwiseAnd, cancellationToken);

            if (methodSymbol?.MethodKind != MethodKind.BuiltinOperator
                || methodSymbol.Name != WellKnownMemberNames.BitwiseAndOperatorName
                || methodSymbol.ContainingType?.TypeKind != TypeKind.Enum)
            {
                return;
            }

            if (IsSuitableAsArgumentOfHasFlag(bitwiseAnd.Right, semanticModel, cancellationToken))
            {
                if (!IsSuitableAsExpressionOfHasFlag(bitwiseAnd.Left))
                    return;
            }
            else if (IsSuitableAsArgumentOfHasFlag(bitwiseAnd.Left, semanticModel, cancellationToken))
            {
                if (!IsSuitableAsExpressionOfHasFlag(bitwiseAnd.Right))
                    return;
            }
            else
            {
                return;
            }

            DiagnosticHelpers.ReportDiagnostic(
                context,
                DiagnosticDescriptors.ReportOnly.ConvertBitwiseOperationToHasFlagCall,
                equalsOrNotEquals);

            bool IsSuitableAsExpressionOfHasFlag(ExpressionSyntax expression)
            {
                expression = expression.WalkDownParentheses();

                if (expression.IsKind(
                    SyntaxKind.IdentifierName,
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxKind.InvocationExpression,
                    SyntaxKind.ElementAccessExpression))
                {
                    return semanticModel.GetTypeSymbol(expression, cancellationToken)?.TypeKind == TypeKind.Enum;
                }

                return false;
            }
        }
    }
}
