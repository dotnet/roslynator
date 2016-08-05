// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Analysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class SwitchStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, SwitchStatementSyntax switchStatement)
        {
            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.GenerateSwitchSections)
                && await GenerateSwitchSectionsRefactoring.CanRefactorAsync(context, switchStatement).ConfigureAwait(false))
            {
                context.RegisterRefactoring(
                    "Generate switch sections",
                    cancellationToken =>
                    {
                        return GenerateSwitchSectionsRefactoring.RefactorAsync(
                            context.Document,
                            switchStatement,
                            cancellationToken);
                    });
            }

            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.RemoveStatementsFromSwitchSections))
                RemoveStatementsFromSwitchSectionsRefactoring.ComputeRefactoring(context, switchStatement);

            if (switchStatement.Sections.Count > 0
                && switchStatement.SwitchKeyword.Span.Contains(context.Span))
            {
                if (context.Settings.IsAnyRefactoringEnabled(
                    RefactoringIdentifiers.ReplaceStatementsWithBlockInEachSwitchSection,
                    RefactoringIdentifiers.ReplaceBlockWithStatementsInEachSwitchSection))
                {
                    SwitchStatementAnalysisResult result = SwitchStatementAnalysis.Analyze(switchStatement);

                    if (result.CanReplaceStatementsWithBlock
                        && context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceStatementsWithBlockInEachSwitchSection))
                    {
                        context.RegisterRefactoring(
                            "Replace statements with block (in each section)",
                            cancellationToken => ReplaceStatementsWithBlockInEachSectionRefactoring.RefactorAsync(context.Document, switchStatement, cancellationToken));
                    }

                    if (result.CanReplaceBlockWithStatements
                        && context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceBlockWithStatementsInEachSwitchSection))
                    {
                        context.RegisterRefactoring(
                            "Replace block with statements (in each section)",
                            cancellationToken => ReplaceBlockWithStatementsInEachSectionRefactoring.RefactorAsync(context.Document, switchStatement, cancellationToken));
                    }
                }

                if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceSwitchWithIfElse)
                    && switchStatement.Sections
                        .Any(section => !section.Labels.Contains(SyntaxKind.DefaultSwitchLabel)))
                {
                    context.RegisterRefactoring(
                        "Replace switch with if-else",
                        cancellationToken => ReplaceSwitchWithIfElseRefactoring.RefactorAsync(context.Document, switchStatement, cancellationToken));
                }
            }
        }
    }
}