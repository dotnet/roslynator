// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class SwitchSectionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, SwitchSectionSyntax switchSection)
        {
            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceSwitchSectionStatementsWithBlock)
                && ReplaceSwitchSectionStatementsWithBlockRefactoring.CanRefactor(switchSection))
            {
                context.RegisterRefactoring(
                    "Replace statements with block",
                    cancellationToken => ReplaceSwitchSectionStatementsWithBlockRefactoring.RefactorAsync(context.Document, switchSection, cancellationToken));
            }

            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceSwitchSectionBlockWithStatements)
                && ReplaceSwitchSectionBlockWithStatementsRefactoring.CanRefactor(context, switchSection))
            {
                context.RegisterRefactoring(
                    "Replace block with statements",
                    cancellationToken => ReplaceSwitchSectionBlockWithStatementsRefactoring.RefactorAsync(context.Document, switchSection, cancellationToken));
            }
        }
    }
}