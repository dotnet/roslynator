// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
{
    internal static class RemoveStatementsFromSwitchSectionsRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, SwitchStatementSyntax switchStatement)
        {
            if (switchStatement.Sections.Any())
            {
                ImmutableArray<SwitchSectionSyntax> sections = GetSelectedSwitchSections(switchStatement, context.Span)
                    .ToImmutableArray();

                if (sections.Any())
                {
                    string title = "Remove statements from section";

                    if (sections.Length > 1)
                        title += "s";

                    context.RegisterRefactoring(
                        title,
                        cancellationToken =>
                        {
                            return RefactorAsync(
                                context.Document,
                                switchStatement,
                                sections,
                                cancellationToken);
                        });
                }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            SwitchStatementSyntax switchStatement,
            ImmutableArray<SwitchSectionSyntax> sections,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            IEnumerable<SwitchSectionSyntax> newSections = switchStatement.Sections.Select(section =>
            {
                if (sections.Contains(section))
                    return section.WithStatements(List<StatementSyntax>());

                return section;
            });

            SwitchStatementSyntax newSwitchStatement = switchStatement.WithSections(List(newSections));

            root = root.ReplaceNode(switchStatement, newSwitchStatement);

            return document.WithSyntaxRoot(root);
        }

        public static IEnumerable<SwitchSectionSyntax> GetSelectedSwitchSections(SwitchStatementSyntax switchStatement, TextSpan span)
        {
            return switchStatement.Sections
                .SkipWhile(f => span.Start > f.Span.Start)
                .TakeWhile(f => span.End >= f.Span.End);
        }
    }
}
