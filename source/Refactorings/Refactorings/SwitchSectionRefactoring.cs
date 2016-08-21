// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Analysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
{
    internal static class SwitchSectionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, SwitchSectionSyntax switchSection)
        {
            if (context.IsAnyRefactoringEnabled(
                RefactoringIdentifiers.AddBracesToSwitchSection,
                RefactoringIdentifiers.AddBracesToSwitchSections,
                RefactoringIdentifiers.RemoveBracesFromSwitchSection,
                RefactoringIdentifiers.RemoveBracesFromSwitchSections))
            {
                var switchStatement = (SwitchStatementSyntax)switchSection.Parent;

                SyntaxList<SwitchSectionSyntax> sections = switchStatement.Sections;

                switch (SwitchStatementAnalysis.AnalyzeSection(switchSection))
                {
                    case SwitchSectionAnalysisResult.AddBraces:
                        {
                            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddBracesToSwitchSection))
                            {
                                context.RegisterRefactoring(
                                    "Add braces to section",
                                    cancellationToken => AddBracesToSwitchSectionRefactoring.RefactorAsync(context.Document, switchSection, cancellationToken));
                            }

                            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddBracesToSwitchSections)
                                && sections.Any(f => f != switchSection && SwitchStatementAnalysis.CanAddBraces(f)))
                            {
                                context.RegisterRefactoring(
                                    AddBracesToSwitchSectionsRefactoring.Title,
                                    cancellationToken => AddBracesToSwitchSectionsRefactoring.RefactorAsync(context.Document, switchStatement, null, cancellationToken));
                            }

                            break;
                        }
                    case SwitchSectionAnalysisResult.RemoveBraces:
                        {
                            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveBracesFromSwitchSection))
                            {
                                context.RegisterRefactoring(
                                    "Remove braces from section",
                                    cancellationToken => RemoveBracesFromSwitchSectionRefactoring.RefactorAsync(context.Document, switchSection, cancellationToken));
                            }

                            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveBracesFromSwitchSections)
                                && sections.Any(f => f != switchSection && SwitchStatementAnalysis.CanRemoveBraces(f)))
                            {
                                context.RegisterRefactoring(
                                    RemoveBracesFromSwitchSectionsRefactoring.Title,
                                    cancellationToken => RemoveBracesFromSwitchSectionsRefactoring.RefactorAsync(context.Document, switchStatement, null, cancellationToken));
                            }

                            break;
                        }
                }
            }
        }
    }
}