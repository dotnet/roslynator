// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceInterpolatedStringWithConcatenationRefactoring
    {
        public static void AnalyzeInterpolatedStringExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.ContainsDiagnostics)
                return;

            if (context.Node.ContainsDirectives)
                return;

            var interpolatedString = (InterpolatedStringExpressionSyntax)context.Node;

            SyntaxList<InterpolatedStringContentSyntax> contents = interpolatedString.Contents;

            if (contents.Count <= 1)
                return;

            if (contents.Any(f => f.Kind() != SyntaxKind.Interpolation))
                return;

            foreach (InterpolatedStringContentSyntax content in contents)
            {
                var interpolation = (InterpolationSyntax)content;

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
            }

            context.ReportDiagnostic(DiagnosticDescriptors.ReplaceInterpolatedStringWithConcatenation, interpolatedString);

            context.ReportToken(DiagnosticDescriptors.ReplaceInterpolatedStringWithConcatenationFadeOut, interpolatedString.StringStartToken);

            foreach (InterpolatedStringContentSyntax content in contents)
            {
                var interpolation = (InterpolationSyntax)content;

                context.ReportToken(DiagnosticDescriptors.ReplaceInterpolatedStringWithConcatenationFadeOut, interpolation.OpenBraceToken);
                context.ReportToken(DiagnosticDescriptors.ReplaceInterpolatedStringWithConcatenationFadeOut, interpolation.CloseBraceToken);
            }

            context.ReportToken(DiagnosticDescriptors.ReplaceInterpolatedStringWithConcatenationFadeOut, interpolatedString.StringEndToken);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            InterpolatedStringExpressionSyntax interpolatedString,
            CancellationToken cancellationToken)
        {
            SyntaxList<InterpolatedStringContentSyntax> contents = interpolatedString.Contents;

            ExpressionSyntax newNode = AddExpression(
                ((InterpolationSyntax)contents[0]).Expression.Parenthesize(),
                ((InterpolationSyntax)contents[1]).Expression.Parenthesize());

            for (int i = 2; i < contents.Count; i++)
            {
                newNode = AddExpression(
                    newNode,
                    ((InterpolationSyntax)contents[i]).Expression.Parenthesize());
            }

            newNode = newNode
                .WithTriviaFrom(interpolatedString)
                .Parenthesize()
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(interpolatedString, newNode, cancellationToken);
        }
    }
}
