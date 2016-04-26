// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DeclareEachAttributeSeparatelyCodeFixProvider))]
    [Shared]
    public class DeclareEachAttributeSeparatelyCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.DeclareEachAttributeSeparately);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            AttributeListSyntax attributeList = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<AttributeListSyntax>();

            if (attributeList == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Declare each attribute separately",
                c => DeclareEachAttributeSeparatelyAsync(context.Document, attributeList, c),
                DiagnosticIdentifiers.DeclareEachAttributeSeparately + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> DeclareEachAttributeSeparatelyAsync(
            Document document,
            AttributeListSyntax attributeList,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            AttributeListSyntax[] attributeLists = attributeList
                .Attributes
                .Select(f => SyntaxFactory
                    .AttributeList()
                    .AddAttributes(f)
                    .WithAdditionalAnnotations(Formatter.Annotation))
                .ToArray();

            attributeLists[0] = attributeLists[0]
                .WithLeadingTrivia(attributeList.GetLeadingTrivia());

            attributeLists[attributeLists.Length - 1] = attributeLists[attributeLists.Length - 1]
                .WithTrailingTrivia(attributeList.GetTrailingTrivia());

            SyntaxNode newRoot = oldRoot.ReplaceNode(attributeList, attributeLists);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}