// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis.CSharp.Analysis;
using Pihrtsoft.CodeAnalysis.CSharp.Comparers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class SwitchStatementRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, SwitchStatementSyntax switchStatement)
        {
            if (switchStatement.Sections.Count > 0
                && switchStatement.SwitchKeyword.Span.Contains(context.Span))
            {
                SwitchStatementAnalysisResult result = SwitchStatementAnalysis.Analyze(switchStatement);

                if (result.CanAddBraces)
                {
                    context.RegisterRefactoring(
                        "Add braces to switch sections",
                        cancellationToken => AddBracesToSwitchSectionsAsync(context.Document, switchStatement, cancellationToken));
                }

                if (result.CanRemoveBraces)
                {
                    context.RegisterRefactoring(
                        "Remove braces from switch sections",
                        cancellationToken => RemoveBracesFromSwitchSectionsAsync(context.Document, switchStatement, cancellationToken));
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
                        cancellationToken => SortSwitchSectionsAsync(context.Document, switchStatement, cancellationToken));
                }
#endif
            }
        }

        private static async Task<Document> AddBracesToSwitchSectionsAsync(
            Document document,
            SwitchStatementSyntax switchStatement,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            IEnumerable<SwitchSectionSyntax> newSections = switchStatement
                .Sections
                .Select(section =>
                {
                    if (SwitchStatementAnalysis.CanAddBracesToSection(section))
                    {
                        return section.WithStatements(SingletonList<StatementSyntax>(Block(section.Statements)));
                    }
                    else
                    {
                        return section;
                    }
                });

            SwitchStatementSyntax newSwitchStatement = switchStatement
                .WithSections(List(newSections))
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(switchStatement, newSwitchStatement);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> RemoveBracesFromSwitchSectionsAsync(
            Document document,
            SwitchStatementSyntax switchStatement,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            IEnumerable<SwitchSectionSyntax> newSections = switchStatement
                .Sections
                .Select(section =>
                {
                    if (SwitchStatementAnalysis.CanRemoveBracesFromSection(section))
                    {
                        var block = (BlockSyntax)section.Statements[0];
                        return section.WithStatements(block.Statements);
                    }
                    else
                    {
                        return section;
                    }
                });

            SwitchStatementSyntax newSwitchStatement = switchStatement
                .WithSections(List(newSections))
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(switchStatement, newSwitchStatement);

            return document.WithSyntaxRoot(newRoot);
        }

#if DEBUG
        private static async Task<Document> SortSwitchSectionsAsync(
            Document document,
            SwitchStatementSyntax switchStatement,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SwitchStatementSyntax newSwitchStatement = SortSections(switchStatement)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(switchStatement, newSwitchStatement);

            return document.WithSyntaxRoot(newRoot);
        }

        private static SwitchStatementSyntax SortSections(SwitchStatementSyntax switchStatement)
        {
            if (switchStatement == null)
                throw new ArgumentNullException(nameof(switchStatement));

            return switchStatement.WithSections(
                List(switchStatement.Sections
                    .Select(f => SortLabels(f))
                    .OrderBy(f => f, _switchSectionSorter)));
        }

        private static SwitchSectionSyntax SortLabels(SwitchSectionSyntax switchSection)
        {
            if (switchSection == null)
                throw new ArgumentNullException(nameof(switchSection));

            return SwitchSection(
                List(switchSection.Labels.OrderBy(f => f, _switchLabelSorter)),
                switchSection.Statements);
        }

        private static readonly SwitchLabelSorter _switchLabelSorter = new SwitchLabelSorter();
        private static readonly SwitchSectionSorter _switchSectionSorter = new SwitchSectionSorter();
#endif
    }
}