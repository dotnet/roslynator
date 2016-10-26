// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Pihrtsoft.CodeAnalysis.CSharp.SyntaxRewriters;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class SyntaxRemover
    {
        public static TNode RemoveComment<TNode>(TNode node, CommentRemoveOptions removeOptions) where TNode : SyntaxNode
        {
            CommentRemover remover = CommentRemover.Create(node, removeOptions);

            return (TNode)remover.Visit(node);
        }

        public static TNode RemoveComment<TNode>(TNode node, CommentRemoveOptions removeOptions, TextSpan span) where TNode : SyntaxNode
        {
            CommentRemover remover = CommentRemover.Create(node, removeOptions, span);

            return (TNode)remover.Visit(node);
        }

        public static ArgumentListSyntax RemoveNameColon(ArgumentListSyntax argumentList)
        {
            return RemoveNameColon(argumentList, ImmutableArray<ArgumentSyntax>.Empty);
        }

        public static ArgumentListSyntax RemoveNameColon(
            ArgumentListSyntax argumentList,
            ImmutableArray<ArgumentSyntax> arguments)
        {
            if (argumentList == null)
                throw new ArgumentNullException(nameof(argumentList));

            if (arguments == null)
            {
                return (ArgumentListSyntax)NameColonRemover.Instance.Visit(argumentList);
            }
            else
            {
                var remover = new NameColonRemover(arguments);

                return (ArgumentListSyntax)remover.Visit(argumentList);
            }
        }

        public static TNode RemoveTrivia<TNode>(TNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return (TNode)TriviaRemover.Instance.Visit(node);
        }

        public static TNode RemoveWhitespaceOrEndOfLine<TNode>(TNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return (TNode)WhitespaceOrEndOfLineRemover.Instance.Visit(node);
        }

        public static TNode RemoveWhitespaceOrEndOfLine<TNode>(TNode node, TextSpan span) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var remover = new WhitespaceOrEndOfLineRemover(span);

            return (TNode)remover.Visit(node);
        }

        public static async Task<SyntaxTree> RemoveDirectivesAsync(
            SyntaxTree syntaxTree,
            ImmutableArray<DirectiveTriviaSyntax> directives,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (syntaxTree == null)
                throw new ArgumentNullException(nameof(syntaxTree));

            SourceText sourceText = await syntaxTree.GetTextAsync(cancellationToken).ConfigureAwait(false);

            SourceText newSourceText = RemoveDirectives(sourceText, directives);

            return syntaxTree.WithChangedText(newSourceText);
        }

        public static async Task<Document> RemoveDirectivesAsync(
            Document document,
            ImmutableArray<DirectiveTriviaSyntax> directives,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            SourceText newSourceText = RemoveDirectives(sourceText, directives);

            return document.WithText(newSourceText);
        }

        public static async Task<Document> RemoveRegionAsync(
            Document document,
            RegionDirectiveTriviaSyntax regionDirective,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (regionDirective == null)
                throw new ArgumentNullException(nameof(regionDirective));

            List<DirectiveTriviaSyntax> list = regionDirective.GetRelatedDirectives();

            if (list.Count == 2
                && list[1].IsKind(SyntaxKind.EndRegionDirectiveTrivia))
            {
                var endRegionDirective = (EndRegionDirectiveTriviaSyntax)list[1];

                SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

                int startLine = regionDirective.GetSpanStartLine();
                int endLine = endRegionDirective.GetSpanEndLine();

                TextLineCollection lines = sourceText.Lines;

                TextSpan span = TextSpan.FromBounds(
                    lines[startLine].Start,
                    lines[endLine].EndIncludingLineBreak);

                var textChange = new TextChange(span, string.Empty);

                SourceText newSourceText = sourceText.WithChanges(textChange);

                return document.WithText(newSourceText);
            }

            return document;
        }

        public static SourceText RemoveDirectives(
            SourceText sourceText,
            ImmutableArray<DirectiveTriviaSyntax> directives)
        {
            if (sourceText == null)
                throw new ArgumentNullException(nameof(sourceText));

            TextLineCollection lines = sourceText.Lines;

            var changes = new List<TextChange>();

            foreach (DirectiveTriviaSyntax directive in directives)
            {
                int startLine = directive.GetSpanStartLine();

                changes.Add(new TextChange(lines[startLine].SpanIncludingLineBreak, string.Empty));
            }

            return sourceText.WithChanges(changes);
        }

        public static async Task<SyntaxTree> RemoveRegionDirectivesAsync(
            SyntaxTree syntaxTree,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (syntaxTree == null)
                throw new ArgumentNullException(nameof(syntaxTree));

            SyntaxNode root = await syntaxTree.GetRootAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<DirectiveTriviaSyntax> directives = SyntaxUtility.GetRegionDirectives(root).ToImmutableArray();

            return await RemoveDirectivesAsync(syntaxTree, directives, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<Document> RemoveRegionDirectivesAsync(
            Document document,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<DirectiveTriviaSyntax> directives = SyntaxUtility.GetRegionDirectives(root).ToImmutableArray();

            return await RemoveDirectivesAsync(document, directives, cancellationToken).ConfigureAwait(false);
        }

        public static SyntaxNode RemoveEmptyNamespaces(SyntaxNode node, SyntaxRemoveOptions removeOptions)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            IEnumerable<NamespaceDeclarationSyntax> emptyNamespaces = node
                .DescendantNodes()
                .Where(f => f.IsKind(SyntaxKind.NamespaceDeclaration))
                .Cast<NamespaceDeclarationSyntax>()
                .Where(f => f.Members.Count == 0);

            return node.RemoveNodes(emptyNamespaces, removeOptions);
        }
    }
}
