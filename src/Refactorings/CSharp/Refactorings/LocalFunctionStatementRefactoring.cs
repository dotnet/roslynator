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
                        if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveMemberDeclaration))
                        {
                            context.RegisterRefactoring(CodeActionFactory.RemoveStatement(context.Document, localFunctionStatement, equivalenceKey: RefactoringIdentifiers.RemoveMemberDeclaration));
                        }

                        if (context.IsRefactoringEnabled(RefactoringIdentifiers.DuplicateMember))
                        {
                            context.RegisterRefactoring(
                                "Duplicate local function",
                                ct => DuplicateMemberDeclarationRefactoring.RefactorAsync(context.Document, localFunctionStatement, ct),
                                RefactoringIdentifiers.DuplicateMember);
                        }

                        if (context.IsRefactoringEnabled(RefactoringIdentifiers.CommentOutMemberDeclaration))
                            CommentOutRefactoring.RegisterRefactoring(context, localFunctionStatement);
                    }
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ChangeMethodReturnTypeToVoid)
                && context.Span.IsEmptyAndContainedInSpan(localFunctionStatement))
            {
                await ChangeMethodReturnTypeToVoidRefactoring.ComputeRefactoringAsync(context, localFunctionStatement).ConfigureAwait(false);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddTypeParameter))
                AddTypeParameterRefactoring.ComputeRefactoring(context, localFunctionStatement);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ConvertBlockBodyToExpressionBody)
                && ConvertBlockBodyToExpressionBodyRefactoring.CanRefactor(localFunctionStatement, context.Span))
            {
                context.RegisterRefactoring(
                    ConvertBlockBodyToExpressionBodyRefactoring.Title,
                    ct => ConvertBlockBodyToExpressionBodyRefactoring.RefactorAsync(context.Document, localFunctionStatement, ct),
                    RefactoringIdentifiers.ConvertBlockBodyToExpressionBody);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.MoveUnsafeContextToContainingDeclaration))
                MoveUnsafeContextToContainingDeclarationRefactoring.ComputeRefactoring(context, localFunctionStatement);
        }
    }
}
