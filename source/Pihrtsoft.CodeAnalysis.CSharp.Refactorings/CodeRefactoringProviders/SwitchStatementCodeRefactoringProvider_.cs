// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis.CSharp.Comparers;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
#if DEBUG
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(SwitchStatementCodeRefactoringProvider2))]
    public class SwitchStatementCodeRefactoringProvider2 : CodeRefactoringProvider
    {
        private static readonly SwitchLabelSorter _switchLabelSorter = new SwitchLabelSorter();
        private static readonly SwitchSectionSorter _switchSectionSorter = new SwitchSectionSorter();

        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            SwitchStatementSyntax switchStatement = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<SwitchStatementSyntax>();

            if (switchStatement == null)
                return;

            if (switchStatement.Sections.Count > 1
                && switchStatement.SwitchKeyword.Span.Contains(context.Span))
            {
                context.RegisterRefactoring(
                    "Sort switch sections",
                    cancellationToken => SortSwitchSectionsAsync(context.Document, switchStatement, cancellationToken));
            }
        }

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
                SyntaxFactory.List(switchStatement.Sections
                    .Select(f => SortLabels(f))
                    .OrderBy(f => f, _switchSectionSorter)));
        }

        private static SwitchSectionSyntax SortLabels(SwitchSectionSyntax switchSection)
        {
            if (switchSection == null)
                throw new ArgumentNullException(nameof(switchSection));

            return SyntaxFactory.SwitchSection(
                SyntaxFactory.List(switchSection.Labels.OrderBy(f => f, _switchLabelSorter)),
                switchSection.Statements);
        }
    }
#endif
}