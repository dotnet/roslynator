// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(SwitchSectionCodeRefactoringProvider))]
    public class SwitchSectionCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            SwitchSectionSyntax switchSection = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<SwitchSectionSyntax>();

            if (switchSection == null)
                return;

            AddBraces(context, switchSection);

            RemoveBraces(context, switchSection);
        }

        private static void AddBraces(CodeRefactoringContext context, SwitchSectionSyntax switchSection)
        {
            if (switchSection.Statements.All(f => !f.IsKind(SyntaxKind.Block)))
            {
                context.RegisterRefactoring(
                    "Add braces to switch section",
                    cancellationToken => AddBracesToSwitchSectionAsync(context.Document, switchSection, cancellationToken));
            }
        }

        private static void RemoveBraces(CodeRefactoringContext context, SwitchSectionSyntax switchSection)
        {
            if (switchSection.ContainsSingleBlock())
            {
                context.RegisterRefactoring(
                    "Remove braces from switch section",
                    cancellationToken => RemoveBracesAsync(context.Document, switchSection, cancellationToken));
            }
        }

        private static async Task<Document> RemoveBracesAsync(
            Document document,
            SwitchSectionSyntax switchSection,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            var block = (BlockSyntax)switchSection.Statements[0];

            SwitchSectionSyntax newSwitchSection = switchSection
                .WithStatements(block.Statements)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(switchSection, newSwitchSection);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> AddBracesToSwitchSectionAsync(
            Document document,
            SwitchSectionSyntax switchSection,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SwitchSectionSyntax newNode = switchSection
                .WithStatements(List<StatementSyntax>(SingletonList(Block(switchSection.Statements))))
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(switchSection, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}