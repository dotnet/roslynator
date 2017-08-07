// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
                                cancellationToken => CSharpFormatter.ToMultiLineAsync(context.Document, accessor, cancellationToken));
                        }
                    }
                    else
                    {
                        SyntaxList<StatementSyntax> statements = body.Statements;

                        if (statements.Count == 1
                            && statements[0].IsSingleLine())
                        {
                            context.RegisterRefactoring(
                                "Format braces on a single line",
                                cancellationToken => CSharpFormatter.ToSingleLineAsync(context.Document, accessor, cancellationToken));
                        }
                    }
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseExpressionBodiedMember)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(accessor)
                && context.SupportsCSharp6
                && UseExpressionBodiedMemberRefactoring.CanRefactor(accessor))
            {
                SyntaxNode node = accessor;

                var accessorList = accessor.Parent as AccessorListSyntax;

                if (accessorList != null)
                {
                    SyntaxList<AccessorDeclarationSyntax> accessors = accessorList.Accessors;

                    if (accessors.Count == 1
                        && accessors.First().IsKind(SyntaxKind.GetAccessorDeclaration))
                    {
                        var parent = accessorList.Parent as MemberDeclarationSyntax;

                        if (parent != null)
                            node = parent;
                    }
                }

                context.RegisterRefactoring(
                    "Use expression-bodied member",
                    cancellationToken => UseExpressionBodiedMemberRefactoring.RefactorAsync(context.Document, node, cancellationToken));
            }
        }
    }
}
