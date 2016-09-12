// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
{
    internal static class InterpolatedStringRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, InterpolatedStringExpressionSyntax interpolatedString)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.InsertStringInterpolation)
                && context.Span.IsEmpty
                && CanRefactor(context, interpolatedString))
            {
                context.RegisterRefactoring("Insert interpolation",
                    cancellationToken =>
                    {
                        return InsertInterpolationRefactoring.RefactorAsync(
                            context.Document,
                            interpolatedString,
                            context.Span,
                            cancellationToken);
                    });
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceInterpolatedStringWithStringLiteral)
                && ReplaceInterpolatedStringWithStringLiteralRefactoring.CanRefactor(interpolatedString))
            {
                context.RegisterRefactoring("Remove '$'",
                    cancellationToken =>
                    {
                        return ReplaceInterpolatedStringWithStringLiteralRefactoring.RefactorAsync(
                            context.Document,
                            interpolatedString,
                            cancellationToken);
                    });
            }
        }

        private static bool CanRefactor(RefactoringContext context, InterpolatedStringExpressionSyntax interpolatedString)
        {
            int i = 0;
            SyntaxList<InterpolatedStringContentSyntax> contents = interpolatedString.Contents;

            foreach (InterpolatedStringContentSyntax content in contents)
            {
                SyntaxKind kind = content.Kind();
                TextSpan span = content.Span;

                if (kind == SyntaxKind.InterpolatedStringText)
                {
                    if (span.End == context.Span.End)
                        return true;
                }
                else if (kind == SyntaxKind.Interpolation)
                {
                    if (span.Start == context.Span.End)
                        return true;

                    if (span.End == context.Span.Start
                        && (i == contents.Count - 1 || !contents[i + 1].IsKind(SyntaxKind.InterpolatedStringText)))
                    {
                        return true;
                    }
                }

                i++;
            }

            return false;
        }
    }
}
