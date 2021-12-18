// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
                        if (context.IsRefactoringEnabled(RefactoringDescriptors.RemoveMemberDeclaration))
                        {
                            context.RegisterRefactoring(CodeActionFactory.RemoveStatement(context.Document, localFunctionStatement, equivalenceKey: EquivalenceKey.Create(RefactoringDescriptors.RemoveMemberDeclaration)));
                        }

                        if (context.IsRefactoringEnabled(RefactoringDescriptors.CopyMemberDeclaration))
                        {
                            context.RegisterRefactoring(
                                "Copy local function",
                                ct => CopyMemberDeclarationRefactoring.RefactorAsync(
                                    context.Document,
                                    localFunctionStatement,
                                    copyAfter: body.CloseBraceToken.Span.Contains(context.Span),
                                    ct),
                                RefactoringDescriptors.CopyMemberDeclaration);
                        }

                        if (context.IsRefactoringEnabled(RefactoringDescriptors.CommentOutMemberDeclaration))
                            CommentOutRefactoring.RegisterRefactoring(context, localFunctionStatement);
                    }
                }
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ChangeMethodReturnTypeToVoid)
                && context.Span.IsEmptyAndContainedInSpan(localFunctionStatement))
            {
                await ChangeMethodReturnTypeToVoidRefactoring.ComputeRefactoringAsync(context, localFunctionStatement).ConfigureAwait(false);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.AddGenericParameterToDeclaration))
                AddGenericParameterToDeclarationRefactoring.ComputeRefactoring(context, localFunctionStatement);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertBlockBodyToExpressionBody)
                && ConvertBlockBodyToExpressionBodyRefactoring.CanRefactor(localFunctionStatement, context.Span))
            {
                context.RegisterRefactoring(
                    ConvertBlockBodyToExpressionBodyRefactoring.Title,
                    ct => ConvertBlockBodyToExpressionBodyRefactoring.RefactorAsync(context.Document, localFunctionStatement, ct),
                    RefactoringDescriptors.ConvertBlockBodyToExpressionBody);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.MoveUnsafeContextToContainingDeclaration))
                MoveUnsafeContextToContainingDeclarationRefactoring.ComputeRefactoring(context, localFunctionStatement);
        }
    }
}
