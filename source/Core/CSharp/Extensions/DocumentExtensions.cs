// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Documentation;
using Roslynator.CSharp.Helpers;
using Roslynator.CSharp.Helpers.ModifierHelpers;
using Roslynator.CSharp.SyntaxRewriters;

namespace Roslynator.CSharp
{
    public static class DocumentExtensions
    {
        internal static async Task<Document> InsertModifierAsync(
            this Document document,
            SyntaxNode node,
            SyntaxKind modifierKind,
            IModifierComparer comparer = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxTokenList modifiers = node.GetModifiers();

            if (!modifiers.Contains(modifierKind))
            {
                SyntaxNode newNode = node.InsertModifier(modifierKind, comparer);

                return await document.ReplaceNodeAsync(node, newNode, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                return document;
            }
        }

        internal static Task<Document> RemoveModifierAsync(
            this Document document,
            SyntaxNode node,
            SyntaxKind modifierKind,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode newNode = ModifierHelper.RemoveModifier(node, modifierKind);

            return document.ReplaceNodeAsync(node, newNode, cancellationToken);
        }

        internal static Task<Document> RemoveModifierAsync(
            this Document document,
            SyntaxNode node,
            SyntaxToken modifier,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode newNode = ModifierHelper.RemoveModifier(node, modifier);

            return document.ReplaceNodeAsync(node, newNode, cancellationToken);
        }

        public static async Task<Document> RemoveMemberAsync(
            this Document document,
            MemberDeclarationSyntax member,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            SyntaxNode parent = member.Parent;

            if (parent?.IsKind(SyntaxKind.CompilationUnit) == true)
            {
                var compilationUnit = (CompilationUnitSyntax)parent;

                return await document.ReplaceNodeAsync(compilationUnit, compilationUnit.RemoveMember(member), cancellationToken).ConfigureAwait(false);
            }
            else
            {
                var parentMember = parent as MemberDeclarationSyntax;

                if (parentMember != null)
                {
                    return await document.ReplaceNodeAsync(parentMember, parentMember.RemoveMember(member), cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    return await document.RemoveNodeAsync(member, RemoveHelper.DefaultRemoveOptions, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        internal static Task<Document> RemoveStatementAsync(this Document document, StatementSyntax statement, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            return document.RemoveNodeAsync(statement, RemoveHelper.GetRemoveOptions(statement), cancellationToken);
        }

        public static Task<Document> RemoveCommentAsync(
            this Document document,
            SyntaxTrivia comment,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return RemoveCommentHelper.RemoveCommentAsync(document, comment, cancellationToken);
        }

        public static async Task<Document> RemoveCommentsAsync(
            this Document document,
            CommentRemoveOptions removeOptions,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = root
                .RemoveComments(removeOptions, span)
                .WithFormatterAnnotation();

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> RemoveCommentsAsync(
            this Document document,
            CommentRemoveOptions removeOptions,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = root
                .RemoveComments(removeOptions)
                .WithFormatterAnnotation();

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> RemoveTriviaAsync(
            this Document document,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = TriviaRemover.RemoveTrivia(root, span);

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> RemoveDirectivesAsync(
            this Document document,
            DirectiveRemoveOptions removeOptions,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            SourceText newSourceText = RemoveDirectives(sourceText, GetDirectives(root, removeOptions));

            return document.WithText(newSourceText);
        }

        private static IEnumerable<DirectiveTriviaSyntax> GetDirectives(SyntaxNode root, DirectiveRemoveOptions removeOptions)
        {
            switch (removeOptions)
            {
                case DirectiveRemoveOptions.All:
                    {
                        return root.DescendantDirectives();
                    }
                case DirectiveRemoveOptions.AllExceptRegion:
                    {
                        return root
                            .DescendantDirectives()
                            .Where(f => !f.IsKind(SyntaxKind.RegionDirectiveTrivia, SyntaxKind.EndRegionDirectiveTrivia));
                    }
                case DirectiveRemoveOptions.Region:
                    {
                        return root.DescendantRegionDirectives();
                    }
                default:
                    {
                        throw new ArgumentException("", nameof(removeOptions));
                    }
            }
        }

        public static async Task<Document> RemoveDirectivesAsync(
            this Document document,
            IEnumerable<DirectiveTriviaSyntax> directives,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (directives == null)
                throw new ArgumentNullException(nameof(directives));

            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            SourceText newSourceText = RemoveDirectives(sourceText, directives);

            return document.WithText(newSourceText);
        }

        private static SourceText RemoveDirectives(
            SourceText sourceText,
            IEnumerable<DirectiveTriviaSyntax> directives)
        {
            TextLineCollection lines = sourceText.Lines;

            var changes = new List<TextChange>();

            foreach (DirectiveTriviaSyntax directive in directives)
            {
                int startLine = directive.GetSpanStartLine();

                changes.Add(new TextChange(lines[startLine].SpanIncludingLineBreak, ""));
            }

            return sourceText.WithChanges(changes);
        }

        public static async Task<Document> RemoveRegionAsync(
            this Document document,
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

                return await RemoveRegionAsync(document, regionDirective, endRegionDirective, cancellationToken).ConfigureAwait(false);
            }

            return document;
        }

        public static async Task<Document> RemoveRegionAsync(
            this Document document,
            EndRegionDirectiveTriviaSyntax endRegionDirective,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (endRegionDirective == null)
                throw new ArgumentNullException(nameof(endRegionDirective));

            List<DirectiveTriviaSyntax> list = endRegionDirective.GetRelatedDirectives();

            if (list.Count == 2
                && list[0].IsKind(SyntaxKind.RegionDirectiveTrivia))
            {
                var regionDirective = (RegionDirectiveTriviaSyntax)list[0];

                return await RemoveRegionAsync(document, regionDirective, endRegionDirective, cancellationToken).ConfigureAwait(false);
            }

            return document;
        }

        private static async Task<Document> RemoveRegionAsync(Document document, RegionDirectiveTriviaSyntax regionDirective, EndRegionDirectiveTriviaSyntax endRegionDirective, CancellationToken cancellationToken)
        {
            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            int startLine = regionDirective.GetSpanStartLine();
            int endLine = endRegionDirective.GetSpanEndLine();

            TextLineCollection lines = sourceText.Lines;

            TextSpan span = TextSpan.FromBounds(
                lines[startLine].Start,
                lines[endLine].EndIncludingLineBreak);

            var textChange = new TextChange(span, "");

            SourceText newSourceText = sourceText.WithChanges(textChange);

            return document.WithText(newSourceText);
        }

        internal static async Task<Document> AddNewDocumentationCommentsAsync(Document document, DocumentationCommentGeneratorSettings settings = null, bool skipNamespaceDeclaration = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var rewriter = new AddNewDocumentationCommentRewriter(settings, skipNamespaceDeclaration);

            SyntaxNode newRoot = rewriter.Visit(root);

            return document.WithSyntaxRoot(newRoot);
        }

        internal static async Task<Document> AddBaseOrNewDocumentationCommentsAsync(
            Document document,
            SemanticModel semanticModel,
            DocumentationCommentGeneratorSettings settings = null,
            bool skipNamespaceDeclaration = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var rewriter = new AddBaseOrNewDocumentationCommentRewriter(semanticModel, settings, skipNamespaceDeclaration, cancellationToken);

            SyntaxNode newRoot = rewriter.Visit(root);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
