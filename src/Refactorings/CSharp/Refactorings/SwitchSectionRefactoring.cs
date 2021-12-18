// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SwitchSectionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, SwitchSectionSyntax switchSection)
        {
            if (SelectedStatementsRefactoring.IsAnyRefactoringEnabled(context)
                && StatementListSelection.TryCreate(switchSection, context.Span, out StatementListSelection selectedStatements))
            {
                await SelectedStatementsRefactoring.ComputeRefactoringAsync(context, selectedStatements).ConfigureAwait(false);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.SortCaseLabels)
                && SyntaxListSelection<SwitchLabelSyntax>.TryCreate(switchSection.Labels, context.Span, out SyntaxListSelection<SwitchLabelSyntax> selectedLabels)
                && selectedLabels.Count > 1)
            {
                SortCaseLabelsRefactoring.ComputeRefactoring(context, selectedLabels);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.SplitSwitchLabels))
                SplitSwitchLabelsRefactoring.ComputeRefactoring(context, switchSection);

            if (context.IsAnyRefactoringEnabled(
                RefactoringDescriptors.AddBracesToSwitchSection,
                RefactoringDescriptors.AddBracesToSwitchSections,
                RefactoringDescriptors.RemoveBracesFromSwitchSection,
                RefactoringDescriptors.RemoveBracesFromSwitchSections)
                && context.Span.IsEmpty
                && IsContainedInCaseOrDefaultKeyword(context.Span))
            {
                var switchStatement = (SwitchStatementSyntax)switchSection.Parent;

                SyntaxList<SwitchSectionSyntax> sections = switchStatement.Sections;

                BracesAnalysis analysis = BracesAnalysis.AnalyzeBraces(switchSection);

                if (analysis.AddBraces)
                {
                    if (context.IsRefactoringEnabled(RefactoringDescriptors.AddBracesToSwitchSection))
                    {
                        context.RegisterRefactoring(
                            AddBracesToSwitchSectionRefactoring.Title,
                            ct => AddBracesToSwitchSectionRefactoring.RefactorAsync(context.Document, switchSection, ct),
                            RefactoringDescriptors.AddBracesToSwitchSection);
                    }

                    if (context.IsRefactoringEnabled(RefactoringDescriptors.AddBracesToSwitchSections)
                        && sections.Any(f => f != switchSection && AddBracesToSwitchSectionAnalysis.CanAddBraces(f)))
                    {
                        context.RegisterRefactoring(
                            AddBracesToSwitchSectionsRefactoring.Title,
                            ct => AddBracesToSwitchSectionsRefactoring.RefactorAsync(context.Document, switchStatement, null, ct),
                            RefactoringDescriptors.AddBracesToSwitchSections);
                    }
                }
                else if (analysis.RemoveBraces)
                {
                    if (context.IsRefactoringEnabled(RefactoringDescriptors.RemoveBracesFromSwitchSection))
                    {
                        context.RegisterRefactoring(
                            RemoveBracesFromSwitchSectionRefactoring.Title,
                            ct => RemoveBracesFromSwitchSectionRefactoring.RefactorAsync(context.Document, switchSection, ct),
                            RefactoringDescriptors.RemoveBracesFromSwitchSection);
                    }

                    if (context.IsRefactoringEnabled(RefactoringDescriptors.RemoveBracesFromSwitchSections)
                        && sections.Any(f => f != switchSection && RemoveBracesFromSwitchSectionRefactoring.CanRemoveBraces(f)))
                    {
                        context.RegisterRefactoring(
                            RemoveBracesFromSwitchSectionsRefactoring.Title,
                            ct => RemoveBracesFromSwitchSectionsRefactoring.RefactorAsync(context.Document, switchStatement, null, ct),
                            RefactoringDescriptors.RemoveBracesFromSwitchSections);
                    }
                }
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.CopySwitchSection))
                CopySwitchSectionRefactoring.ComputeRefactoring(context, switchSection);

            bool IsContainedInCaseOrDefaultKeyword(TextSpan span)
            {
                foreach (SwitchLabelSyntax label in switchSection.Labels)
                {
                    if (label.Keyword.Span.Contains(span))
                        return true;
                }

                return false;
            }
        }
    }
}
