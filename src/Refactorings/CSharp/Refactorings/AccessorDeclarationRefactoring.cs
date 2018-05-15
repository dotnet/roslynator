// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AccessorDeclarationRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, AccessorDeclarationSyntax accessor)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.FormatAccessorBraces))
            {
                BlockSyntax body = accessor.Body;

                if (body?.Span.Contains(context.Span) == true
                    && !body.OpenBraceToken.IsMissing
                    && !body.CloseBraceToken.IsMissing)
                {
                    if (body.IsSingleLine())
                    {
                        if (accessor.Parent?.IsMultiLine() == true)
                        {
                            context.RegisterRefactoring(
                                "Format braces on separate lines",
                                ct => SyntaxFormatter.ToMultiLineAsync(context.Document, accessor, ct),
                                RefactoringIdentifiers.FormatAccessorBraces);
                        }
                    }
                    else if (body.Statements.SingleOrDefault(shouldThrow: false)?.IsSingleLine() == true
                        && accessor.DescendantTrivia(accessor.Span).All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                    {
                        context.RegisterRefactoring(
                            "Format braces on a single line",
                            ct => SyntaxFormatter.ToSingleLineAsync(context.Document, accessor, ct),
                            RefactoringIdentifiers.FormatAccessorBraces);
                    }
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseExpressionBodiedMember)
                && context.SupportsCSharp6)
            {
                BlockSyntax body = accessor.Body;

                if (body != null
                    && (context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(accessor)
                        || context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(body))
                    && !accessor.AttributeLists.Any()
                    && ((accessor.IsKind(SyntaxKind.GetAccessorDeclaration))
                        ? UseExpressionBodiedMemberAnalysis.GetReturnExpression(body) != null
                        : UseExpressionBodiedMemberAnalysis.GetExpression(body) != null)
                    && (accessor.Parent as AccessorListSyntax)?
                        .Accessors
                        .SingleOrDefault(shouldThrow: false)?
                        .Kind() != SyntaxKind.GetAccessorDeclaration)
                {
                    context.RegisterRefactoring(
                        UseExpressionBodiedMemberRefactoring.Title,
                        ct => UseExpressionBodiedMemberRefactoring.RefactorAsync(context.Document, accessor, ct),
                        RefactoringIdentifiers.UseExpressionBodiedMember);
                }
            }
        }
    }
}
