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
            if (await GenerateSwitchSectionsRefactoring.CanRefactorAsync(context, switchStatement))
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

            if (switchStatement.Sections.Count > 0
                && switchStatement.SwitchKeyword.Span.Contains(context.Span))
            {
                SwitchStatementAnalysisResult result = SwitchStatementAnalysis.Analyze(switchStatement);

                if (result.CanAddBraces)
                {
                    context.RegisterRefactoring(
                        "Add braces to switch sections",
                        cancellationToken => AddBracesToSwitchSectionsRefactoring.RefactorAsync(context.Document, switchStatement, cancellationToken));
                }

                if (result.CanRemoveBraces)
                {
                    context.RegisterRefactoring(
                        "Remove braces from switch sections",
                        cancellationToken => RemoveBracesFromSwitchSectionsRefactoring.RefactorAsync(context.Document, switchStatement, cancellationToken));
                }

                if (switchStatement.Sections
                    .Any(section => !section.Labels.Contains(SyntaxKind.DefaultSwitchLabel)))
                {
                    context.RegisterRefactoring(
                        "Convert to if-else chain",
                        cancellationToken => ConvertSwitchToIfElseRefactoring.RefactorAsync(context.Document, switchStatement, cancellationToken));
                }
#if DEBUG
                if (switchStatement.Sections.Count > 1
                    && switchStatement.SwitchKeyword.Span.Contains(context.Span))
                {
                    context.RegisterRefactoring(
                        "Sort switch sections",
                        cancellationToken => SortSwitchSectionsRefactoring.RefactorAsync(context.Document, switchStatement, cancellationToken));
                }
#endif
            }
        }
    }
}