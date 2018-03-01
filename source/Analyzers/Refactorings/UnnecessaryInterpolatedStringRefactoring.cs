// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UnnecessaryInterpolatedStringRefactoring
    {
        public static void AnalyzeInterpolatedStringExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.ContainsDiagnostics)
                return;

            if (context.Node.ContainsDirectives)
                return;

            var interpolatedString = (InterpolatedStringExpressionSyntax)context.Node;

            SyntaxList<InterpolatedStringContentSyntax> contents = interpolatedString.Contents;

            if (!contents.Any())
                return;

            var interpolation = contents.SingleOrDefault(shouldThrow: false) as InterpolationSyntax;

            if (interpolation == null)
                return;

            ExpressionSyntax expression = interpolation.Expression;

            if (expression == null)
                return;

            if (interpolation.AlignmentClause != null)
                return;

            if (interpolation.FormatClause != null)
                return;

            ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(expression, context.CancellationToken);

            if (typeSymbol?.SpecialType != SpecialType.System_String)
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.UnnecessaryInterpolatedString, interpolatedString);

            context.ReportToken(DiagnosticDescriptors.UnnecessaryInterpolatedStringFadeOut, interpolatedString.StringStartToken);
            context.ReportToken(DiagnosticDescriptors.UnnecessaryInterpolatedStringFadeOut, interpolation.OpenBraceToken);
            context.ReportToken(DiagnosticDescriptors.UnnecessaryInterpolatedStringFadeOut, interpolation.CloseBraceToken);
            context.ReportToken(DiagnosticDescriptors.UnnecessaryInterpolatedStringFadeOut, interpolatedString.StringEndToken);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            InterpolatedStringExpressionSyntax interpolatedString,
            CancellationToken cancellationToken)
        {
            var interpolation = (InterpolationSyntax)interpolatedString.Contents[0];

            ExpressionSyntax newNode = interpolation.Expression
                .WithTriviaFrom(interpolatedString)
                .Parenthesize()
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(interpolatedString, newNode, cancellationToken);
        }
    }
}
