// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
                                ct => SyntaxFormatter.ToMultiLineAsync(context.Document, accessor, ct));
                        }
                    }
                    else
                    {
                        SyntaxList<StatementSyntax> statements = body.Statements;

                        if (body.Statements.SingleOrDefault(shouldThrow: false)?.IsSingleLine() == true)
                        {
                            context.RegisterRefactoring(
                                "Format braces on a single line",
                                ct => SyntaxFormatter.ToSingleLineAsync(context.Document, accessor, ct));
                        }
                    }
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseExpressionBodiedMember)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(accessor)
                && context.SupportsCSharp6
                && UseExpressionBodiedMemberAnalysis.IsFixable(accessor))
            {
                SyntaxNode node = accessor;

                if (accessor.Parent is AccessorListSyntax accessorList
                    && accessorList.Accessors.SingleOrDefault(shouldThrow: false)?.Kind() == SyntaxKind.GetAccessorDeclaration
                    && (accessorList.Parent is MemberDeclarationSyntax parent))
                {
                    node = parent;
                }

                context.RegisterRefactoring(
                    "Use expression-bodied member",
                    ct => UseExpressionBodiedMemberRefactoring.RefactorAsync(context.Document, node, ct));
            }
        }
    }
}
