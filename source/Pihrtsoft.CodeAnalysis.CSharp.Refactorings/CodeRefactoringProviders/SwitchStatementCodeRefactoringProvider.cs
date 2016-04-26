// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis.CSharp.Analysis;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(SwitchStatementCodeRefactoringProvider))]
    public class SwitchStatementCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            SwitchStatementSyntax switchStatement = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<SwitchStatementSyntax>();

            if (switchStatement == null)
                return;

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

                context.RegisterRefactoring(
                    "Convert to if-else chain",
                    cancellationToken => ConvertSwitchToIfElseRefactoring.RefactorAsync(context.Document, switchStatement, cancellationToken));
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
    }
}