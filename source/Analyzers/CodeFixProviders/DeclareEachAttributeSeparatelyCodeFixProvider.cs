// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
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
                "Split attributes",
                c => DeclareEachAttributeSeparatelyAsync(context.Document, attributeList, c),
                DiagnosticIdentifiers.DeclareEachAttributeSeparately + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> DeclareEachAttributeSeparatelyAsync(
            Document document,
            AttributeListSyntax attributeList,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = oldRoot.ReplaceNode(
                attributeList,
                AttributeRefactoring.SplitAttributes(attributeList)
                    .Select(f => f.WithFormatterAnnotation()));

            return document.WithSyntaxRoot(newRoot);
        }
    }
}