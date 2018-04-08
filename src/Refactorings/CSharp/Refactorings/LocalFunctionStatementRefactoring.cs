// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class LocalFunctionStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, LocalFunctionStatementSyntax localFunctionStatement)
        {
            if (localFunctionStatement.IsParentKind(SyntaxKind.Block))
            {
                BlockSyntax body = localFunctionStatement.Body;

                if (body != null)
                {
                    if (body.OpenBraceToken.Span.Contains(context.Span)
                        || body.CloseBraceToken.Span.Contains(context.Span))
                    {
                        if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveMember))
                        {
                            context.RegisterRefactoring(
                                "Remove local function",
                                cancellationToken => context.Document.RemoveStatementAsync(localFunctionStatement, cancellationToken));
                        }

                        if (context.IsRefactoringEnabled(RefactoringIdentifiers.DuplicateMember))
                        {
                            context.RegisterRefactoring(
                                "Duplicate local function",
                                cancellationToken => DuplicateMemberDeclarationRefactoring.RefactorAsync(context.Document, localFunctionStatement, cancellationToken));
                        }

                        if (context.IsRefactoringEnabled(RefactoringIdentifiers.CommentOutMember))
                            CommentOutRefactoring.RegisterRefactoring(context, localFunctionStatement);
                    }
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddTypeParameter))
                AddTypeParameterRefactoring.ComputeRefactoring(context, localFunctionStatement);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseExpressionBodiedMember)
                && localFunctionStatement.Body?.Span.Contains(context.Span) == true
                && UseExpressionBodiedMemberAnalysis.IsFixable(localFunctionStatement))
            {
                context.RegisterRefactoring(
                    "Use expression-bodied member",
                    cancellationToken => UseExpressionBodiedMemberRefactoring.RefactorAsync(context.Document, localFunctionStatement, cancellationToken));
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseListInsteadOfYield)
                && localFunctionStatement.Identifier.Span.Contains(context.Span))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                UseListInsteadOfYieldRefactoring.ComputeRefactoring(context, localFunctionStatement, semanticModel);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.MoveUnsafeContextToContainingDeclaration))
                MoveUnsafeContextToContainingDeclarationRefactoring.ComputeRefactoring(context, localFunctionStatement);
        }
    }
}
