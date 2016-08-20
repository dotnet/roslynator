// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
{
    internal static class RemoveStatementsFromSwitchSectionsRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, SwitchStatementSyntax switchStatement)
        {
            SyntaxList<SwitchSectionSyntax> sections = switchStatement.Sections;

            if (sections.Any())
            {
                var info = new SelectedNodesInfo<SwitchSectionSyntax>(sections, context.Span);

                if (info.IsAnySelected)
                {
                    string title = "Remove statements from section";

                    if (info.AreManySelected)
                        title += "s";

                    context.RegisterRefactoring(
                        title,
                        cancellationToken =>
                        {
                            return RefactorAsync(
                                context.Document,
                                switchStatement,
                                info.SelectedNodes().ToImmutableArray(),
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
    }
}
