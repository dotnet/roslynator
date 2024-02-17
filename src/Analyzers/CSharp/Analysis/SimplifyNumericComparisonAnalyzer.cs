using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SimplifyNumericComparisonAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
            {
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.SimplifyNumericComparison);
            }

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(
            f => AnalyzeComparison(f),
            SyntaxKind.EqualsExpression,
            SyntaxKind.GreaterThanExpression,
            SyntaxKind.GreaterThanOrEqualExpression,
            SyntaxKind.LessThanExpression,
            SyntaxKind.LessThanOrEqualExpression);
    }

    private static void AnalyzeComparison(SyntaxNodeAnalysisContext context)
    {
        var binaryExpression = (BinaryExpressionSyntax)context.Node;

        BinaryExpressionInfo info = SyntaxInfo.BinaryExpressionInfo(binaryExpression);

        if (!info.Success)
            return;

        ExpressionSyntax leftExpression = info.Left;
        ExpressionSyntax rightExpression = info.Right;

        if ((leftExpression.IsNumericLiteralExpression("0") && rightExpression.Kind() == SyntaxKind.SubtractExpression)
            || (leftExpression.Kind() == SyntaxKind.SubtractExpression && rightExpression.IsNumericLiteralExpression("0")))
        {
            DiagnosticHelpers.ReportDiagnostic(
                context,
                DiagnosticRules.SimplifyNumericComparison,
                binaryExpression);
        }
    }
}
