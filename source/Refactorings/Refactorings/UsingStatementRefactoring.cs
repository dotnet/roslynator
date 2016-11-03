// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UsingStatementRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, UsingStatementSyntax usingStatement)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ExtractDeclarationFromUsingStatement))
            {
                VariableDeclarationSyntax declaration = usingStatement.Declaration;

                if (declaration != null
                    && context.Span.IsBetweenSpans(declaration)
                    && usingStatement.IsParentKind(SyntaxKind.Block))
                {
                    context.RegisterRefactoring(
                        "Extract local declaration",
                        cancellationToken =>
                        {
                            return ExtractDeclarationFromUsingStatementRefactoring.RefactorAsync(
                                context.Document,
                                usingStatement,
                                cancellationToken);
                        });
                }
            }
        }
    }
}