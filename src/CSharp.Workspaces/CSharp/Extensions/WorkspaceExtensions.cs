// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Documentation;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp
{
    /// <summary>
    /// A set of extension methods for the workspace layer.
    /// </summary>
    public static class WorkspaceExtensions
    {
        internal static Task<Document> RemoveNodeAsync(
            this Document document,
            SyntaxNode node,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return document.RemoveNodeAsync(node, SyntaxRemover.GetRemoveOptions(node), cancellationToken);
        }

        /// <summary>
        /// Create a new document with the specified member declaration removed.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="member"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal static Task<Document> RemoveMemberAsync(
            this Document document,
            MemberDeclarationSyntax member,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            SyntaxNode parent = member.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.CompilationUnit:
                    {
                        var compilationUnit = (CompilationUnitSyntax)parent;

                        return document.ReplaceNodeAsync(compilationUnit, SyntaxRemover.RemoveMember(compilationUnit, member), cancellationToken);
                    }
                case SyntaxKind.NamespaceDeclaration:
                    {
                        var namespaceDeclaration = (NamespaceDeclarationSyntax)parent;

                        return document.ReplaceNodeAsync(namespaceDeclaration, SyntaxRemover.RemoveMember(namespaceDeclaration, member), cancellationToken);
                    }
                case SyntaxKind.ClassDeclaration:
                    {
                        var classDeclaration = (ClassDeclarationSyntax)parent;

                        return document.ReplaceNodeAsync(classDeclaration, SyntaxRemover.RemoveMember(classDeclaration, member), cancellationToken);
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var structDeclaration = (StructDeclarationSyntax)parent;

                        return document.ReplaceNodeAsync(structDeclaration, SyntaxRemover.RemoveMember(structDeclaration, member), cancellationToken);
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var interfaceDeclaration = (InterfaceDeclarationSyntax)parent;

                        return document.ReplaceNodeAsync(interfaceDeclaration, SyntaxRemover.RemoveMember(interfaceDeclaration, member), cancellationToken);
                    }
                default:
                    {
                        Debug.Assert(parent == null, parent.Kind().ToString());

                        return document.RemoveNodeAsync(member, SyntaxRemover.DefaultRemoveOptions, cancellationToken);
                    }
            }
        }

        internal static Task<Document> RemoveStatementAsync(
            this Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            return document.RemoveNodeAsync(statement, cancellationToken);
        }

        /// <summary>
        /// Creates a new document with comments of the specified kind removed.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="kinds"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<Document> RemoveCommentsAsync(
            this Document document,
            CommentKinds kinds,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = SyntaxRemover.RemoveComments(root, kinds)
                .WithFormatterAnnotation();

            return document.WithSyntaxRoot(newRoot);
        }

        /// <summary>
        /// Creates a new document with comments of the specified kind removed.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="span"></param>
        /// <param name="kinds"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<Document> RemoveCommentsAsync(
            this Document document,
            TextSpan span,
            CommentKinds kinds,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = SyntaxRemover.RemoveComments(root, span, kinds)
                .WithFormatterAnnotation();

            return document.WithSyntaxRoot(newRoot);
        }

        /// <summary>
        /// Creates a new document with trivia inside the specified span removed.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="span"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<Document> RemoveTriviaAsync(
            this Document document,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = SyntaxRemover.RemoveTrivia(root, span);

            return document.WithSyntaxRoot(newRoot);
        }

        /// <summary>
        /// Creates a new document with preprocessor directives of the specified kind removed.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="directiveKinds"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<Document> RemovePreprocessorDirectivesAsync(
            this Document document,
            PreprocessorDirectiveKinds directiveKinds,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            SourceText newSourceText = RemovePreprocessorDirectives(sourceText, root.DescendantPreprocessorDirectives(), directiveKinds);

            return document.WithText(newSourceText);
        }

        /// <summary>
        /// Creates a new document with preprocessor directives of the specified kind removed.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="span"></param>
        /// <param name="directiveKinds"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<Document> RemovePreprocessorDirectivesAsync(
            this Document document,
            TextSpan span,
            PreprocessorDirectiveKinds directiveKinds,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            SourceText newSourceText = RemovePreprocessorDirectives(sourceText, root.DescendantPreprocessorDirectives(span), directiveKinds);

            return document.WithText(newSourceText);
        }

        internal static async Task<Document> RemovePreprocessorDirectivesAsync(
            this Document document,
            IEnumerable<DirectiveTriviaSyntax> directives,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (directives == null)
                throw new ArgumentNullException(nameof(directives));

            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            SourceText newSourceText = sourceText.WithChanges(GetTextChanges());

            return document.WithText(newSourceText);

            IEnumerable<TextChange> GetTextChanges()
            {
                TextLineCollection lines = sourceText.Lines;

                foreach (DirectiveTriviaSyntax directive in directives)
                {
                    int startLine = directive.GetSpanStartLine();

                    yield return new TextChange(lines[startLine].SpanIncludingLineBreak, "");
                }
            }
        }

        private static SourceText RemovePreprocessorDirectives(
            SourceText sourceText,
            IEnumerable<DirectiveTriviaSyntax> directives,
            PreprocessorDirectiveKinds directiveKinds)
        {
            return sourceText.WithChanges(GetTextChanges());

            IEnumerable<TextChange> GetTextChanges()
            {
                TextLineCollection lines = sourceText.Lines;

                foreach (DirectiveTriviaSyntax directive in directives)
                {
                    if (ShouldRemoveDirective(directive))
                    {
                        int startLine = directive.GetSpanStartLine();

                        yield return new TextChange(lines[startLine].SpanIncludingLineBreak, "");
                    }
                }
            }

            bool ShouldRemoveDirective(DirectiveTriviaSyntax directive)
            {
                switch (directive.Kind())
                {
                    case SyntaxKind.IfDirectiveTrivia:
                        return (directiveKinds & PreprocessorDirectiveKinds.If) != 0;
                    case SyntaxKind.ElifDirectiveTrivia:
                        return (directiveKinds & PreprocessorDirectiveKinds.Elif) != 0;
                    case SyntaxKind.ElseDirectiveTrivia:
                        return (directiveKinds & PreprocessorDirectiveKinds.Else) != 0;
                    case SyntaxKind.EndIfDirectiveTrivia:
                        return (directiveKinds & PreprocessorDirectiveKinds.EndIf) != 0;
                    case SyntaxKind.RegionDirectiveTrivia:
                        return (directiveKinds & PreprocessorDirectiveKinds.Region) != 0;
                    case SyntaxKind.EndRegionDirectiveTrivia:
                        return (directiveKinds & PreprocessorDirectiveKinds.EndRegion) != 0;
                    case SyntaxKind.DefineDirectiveTrivia:
                        return (directiveKinds & PreprocessorDirectiveKinds.Define) != 0;
                    case SyntaxKind.UndefDirectiveTrivia:
                        return (directiveKinds & PreprocessorDirectiveKinds.Undef) != 0;
                    case SyntaxKind.ErrorDirectiveTrivia:
                        return (directiveKinds & PreprocessorDirectiveKinds.Error) != 0;
                    case SyntaxKind.WarningDirectiveTrivia:
                        return (directiveKinds & PreprocessorDirectiveKinds.Warning) != 0;
                    case SyntaxKind.LineDirectiveTrivia:
                        return (directiveKinds & PreprocessorDirectiveKinds.Line) != 0;
                    case SyntaxKind.PragmaWarningDirectiveTrivia:
                        return (directiveKinds & PreprocessorDirectiveKinds.PragmaWarning) != 0;
                    case SyntaxKind.PragmaChecksumDirectiveTrivia:
                        return (directiveKinds & PreprocessorDirectiveKinds.PragmaChecksum) != 0;
                    case SyntaxKind.ReferenceDirectiveTrivia:
                        return (directiveKinds & PreprocessorDirectiveKinds.Reference) != 0;
                    case SyntaxKind.BadDirectiveTrivia:
                        return (directiveKinds & PreprocessorDirectiveKinds.Bad) != 0;
                    case SyntaxKind.ShebangDirectiveTrivia:
                        return (directiveKinds & PreprocessorDirectiveKinds.Shebang) != 0;
                    case SyntaxKind.LoadDirectiveTrivia:
                        return (directiveKinds & PreprocessorDirectiveKinds.Load) != 0;
                }

                Debug.Fail(directive.Kind().ToString());
                return false;
            }
        }

        /// <summary>
        /// Creates a new document with the specified region removed.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="region"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<Document> RemoveRegionAsync(
            this Document document,
            RegionInfo region,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (!region.Success)
                throw new ArgumentException($"{nameof(RegionInfo)} is not initialized.", nameof(region));

            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            int startLine = region.Directive.GetSpanStartLine();
            int endLine = region.EndDirective.GetSpanEndLine();

            TextLineCollection lines = sourceText.Lines;

            TextSpan span = TextSpan.FromBounds(
                lines[startLine].Start,
                lines[endLine].EndIncludingLineBreak);

            var textChange = new TextChange(span, "");

            SourceText newSourceText = sourceText.WithChanges(textChange);

            return document.WithText(newSourceText);
        }

        internal static Task<Document> ReplaceStatementsAsync(
            this Document document,
            StatementListInfo statementsInfo,
            IEnumerable<StatementSyntax> newStatements,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return ReplaceStatementsAsync(document, statementsInfo, List(newStatements), cancellationToken);
        }

        internal static Task<Document> ReplaceStatementsAsync(
            this Document document,
            StatementListInfo statementsInfo,
            SyntaxList<StatementSyntax> newStatements,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return document.ReplaceNodeAsync(statementsInfo.Parent, statementsInfo.WithStatements(newStatements).Parent, cancellationToken);
        }

        internal static Task<Document> ReplaceMembersAsync(
            this Document document,
            MemberDeclarationListInfo info,
            IEnumerable<MemberDeclarationSyntax> newMembers,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return document.ReplaceNodeAsync(
                info.Parent,
                info.WithMembers(newMembers).Parent,
                cancellationToken);
        }

        internal static Task<Document> ReplaceMembersAsync(
            this Document document,
            MemberDeclarationListInfo info,
            SyntaxList<MemberDeclarationSyntax> newMembers,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return document.ReplaceNodeAsync(
                info.Parent,
                info.WithMembers(newMembers).Parent,
                cancellationToken);
        }

        internal static Task<Document> ReplaceModifiersAsync(
            this Document document,
            ModifierListInfo modifiersInfo,
            IEnumerable<SyntaxToken> newModifiers,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return ReplaceModifiersAsync(document, modifiersInfo, TokenList(newModifiers), cancellationToken);
        }

        internal static Task<Document> ReplaceModifiersAsync(
            this Document document,
            ModifierListInfo modifiersInfo,
            SyntaxTokenList newModifiers,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return document.ReplaceNodeAsync(modifiersInfo.Parent, modifiersInfo.WithModifiers(newModifiers).Parent, cancellationToken);
        }

        internal static async Task<Document> AddNewDocumentationCommentsAsync(
            Document document,
            DocumentationCommentGeneratorSettings settings = null,
            bool skipNamespaceDeclaration = true,
            CancellationToken cancellationToken = default(CancellationToken))
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
