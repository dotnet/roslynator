// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AvoidInterpolatedStringWithNoInterpolatedTextRefactoring
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

            foreach (InterpolatedStringContentSyntax content in contents)
            {
                SyntaxKind kind = content.Kind();

                if (kind == SyntaxKind.Interpolation)
                    continue;

                if (kind == SyntaxKind.InterpolatedStringText)
                    return;

                Debug.Fail(content.Kind().ToString());
                return;
            }

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

            context.ReportDiagnostic(DiagnosticDescriptors.AvoidInterpolatedStringWithNoInterpolatedText, interpolatedString);

            context.ReportToken(DiagnosticDescriptors.AvoidInterpolatedStringWithNoInterpolatedTextFadeOut, interpolatedString.StringStartToken);

            foreach (InterpolatedStringContentSyntax content in contents)
            {
                var interpolation = (InterpolationSyntax)content;

                context.ReportToken(DiagnosticDescriptors.AvoidInterpolatedStringWithNoInterpolatedTextFadeOut, interpolation.OpenBraceToken);
                context.ReportToken(DiagnosticDescriptors.AvoidInterpolatedStringWithNoInterpolatedTextFadeOut, interpolation.CloseBraceToken);
            }

            context.ReportDiagnostic(DiagnosticDescriptors.AvoidInterpolatedStringWithNoInterpolatedTextFadeOut, interpolatedString.StringEndToken);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            InterpolatedStringExpressionSyntax interpolatedString,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax newNode = null;

            SyntaxList<InterpolatedStringContentSyntax> contents = interpolatedString.Contents;

            if (contents.Count == 1)
            {
                newNode = ((InterpolationSyntax)contents[0]).Expression;
            }
            else
            {
                newNode = AddExpression(
                    ((InterpolationSyntax)contents[0]).Expression.Parenthesize(),
                    ((InterpolationSyntax)contents[1]).Expression.Parenthesize());

                for (int i = 2; i < contents.Count; i++)
                {
                    newNode = AddExpression(
                        newNode,
                        ((InterpolationSyntax)contents[i]).Expression.Parenthesize());
                }
            }

            newNode = newNode
                .WithTriviaFrom(interpolatedString)
                .Parenthesize()
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(interpolatedString, newNode, cancellationToken);
        }
    }
}
