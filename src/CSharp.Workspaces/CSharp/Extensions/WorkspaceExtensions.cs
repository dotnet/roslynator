// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp
{
    /// <summary>
    /// A set of extension methods for the workspace layer.
    /// </summary>
    public static class WorkspaceExtensions
    {
        internal static bool SupportsLanguageFeature(this Document document, CSharpLanguageFeature feature)
        {
            switch (feature)
            {
                case CSharpLanguageFeature.Unknown:
                    return false;
                case CSharpLanguageFeature.NameOf:
                    return SupportsLanguageVersion(document, LanguageVersion.CSharp6);
                case CSharpLanguageFeature.AsyncMain:
                case CSharpLanguageFeature.DefaultLiteral:
                case CSharpLanguageFeature.InferredTupleElementNames:
                case CSharpLanguageFeature.PatternMatchingWithGenerics:
                    return SupportsLanguageVersion(document, LanguageVersion.CSharp7_1);
                case CSharpLanguageFeature.NullCoalescingAssignmentOperator:
                    return SupportsLanguageVersion(document, LanguageVersion.CSharp8);
                case CSharpLanguageFeature.NotPattern:
                    return SupportsLanguageVersion(document, LanguageVersion.CSharp9);
            }

            throw new ArgumentException($"Unknown enum value '{feature}'.", nameof(feature));
        }

        internal static bool SupportsLanguageVersion(this Document document, LanguageVersion languageVersion)
        {
            return ((CSharpParseOptions)document.Project.ParseOptions).LanguageVersion >= languageVersion;
        }

        internal static DefaultSyntaxOptions GetDefaultSyntaxOptions(this Document document, DefaultSyntaxOptions options = DefaultSyntaxOptions.None)
        {
            return (document.SupportsLanguageFeature(CSharpLanguageFeature.DefaultLiteral))
                ? options | DefaultSyntaxOptions.AllowDefaultLiteral
                : options;
        }

        internal static Task<Document> RemoveNodeAsync(
            this Document document,
            SyntaxNode node,
            CancellationToken cancellationToken = default)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return document.RemoveNodeAsync(node, SyntaxRefactorings.GetRemoveOptions(node), cancellationToken);
        }

        /// <summary>
        /// Create a new document with the specified member declaration removed.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="member"></param>
        /// <param name="cancellationToken"></param>
        internal static Task<Document> RemoveMemberAsync(
            this Document document,
            MemberDeclarationSyntax member,
            CancellationToken cancellationToken = default)
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

                        return document.ReplaceNodeAsync(compilationUnit, SyntaxRefactorings.RemoveMember(compilationUnit, member), cancellationToken);
                    }
                case SyntaxKind.NamespaceDeclaration:
                    {
                        var namespaceDeclaration = (NamespaceDeclarationSyntax)parent;

                        return document.ReplaceNodeAsync(namespaceDeclaration, SyntaxRefactorings.RemoveMember(namespaceDeclaration, member), cancellationToken);
                    }
                case SyntaxKind.ClassDeclaration:
                    {
                        var classDeclaration = (ClassDeclarationSyntax)parent;

                        return document.ReplaceNodeAsync(classDeclaration, SyntaxRefactorings.RemoveMember(classDeclaration, member), cancellationToken);
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var structDeclaration = (StructDeclarationSyntax)parent;

                        return document.ReplaceNodeAsync(structDeclaration, SyntaxRefactorings.RemoveMember(structDeclaration, member), cancellationToken);
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var interfaceDeclaration = (InterfaceDeclarationSyntax)parent;

                        return document.ReplaceNodeAsync(interfaceDeclaration, SyntaxRefactorings.RemoveMember(interfaceDeclaration, member), cancellationToken);
                    }
                case SyntaxKind.RecordDeclaration:
                case SyntaxKind.RecordStructDeclaration:
                    {
                        var recordDeclaration = (RecordDeclarationSyntax)parent;

                        return document.ReplaceNodeAsync(recordDeclaration, SyntaxRefactorings.RemoveMember(recordDeclaration, member), cancellationToken);
                    }
                default:
                    {
                        SyntaxDebug.Assert(parent == null, parent);

                        return document.RemoveNodeAsync(member, SyntaxRefactorings.DefaultRemoveOptions, cancellationToken);
                    }
            }
        }

        internal static Task<Document> RemoveStatementAsync(
            this Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken = default)
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
        /// <param name="comments"></param>
        /// <param name="cancellationToken"></param>
        public static async Task<Document> RemoveCommentsAsync(
            this Document document,
            CommentFilter comments,
            CancellationToken cancellationToken = default)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = SyntaxRefactorings.RemoveComments(root, comments)
                .WithFormatterAnnotation();

            return document.WithSyntaxRoot(newRoot);
        }

        /// <summary>
        /// Creates a new document with comments of the specified kind removed.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="span"></param>
        /// <param name="comments"></param>
        /// <param name="cancellationToken"></param>
        public static async Task<Document> RemoveCommentsAsync(
            this Document document,
            TextSpan span,
            CommentFilter comments,
            CancellationToken cancellationToken = default)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = SyntaxRefactorings.RemoveComments(root, span, comments)
                .WithFormatterAnnotation();

            return document.WithSyntaxRoot(newRoot);
        }

        /// <summary>
        /// Creates a new document with trivia inside the specified span removed.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="span"></param>
        /// <param name="cancellationToken"></param>
        public static async Task<Document> RemoveTriviaAsync(
            this Document document,
            TextSpan span,
            CancellationToken cancellationToken = default)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = SyntaxRefactorings.RemoveTrivia(root, span);

            return document.WithSyntaxRoot(newRoot);
        }

        /// <summary>
        /// Creates a new document with preprocessor directives of the specified kind removed.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="directiveFilter"></param>
        /// <param name="cancellationToken"></param>
        public static async Task<Document> RemovePreprocessorDirectivesAsync(
            this Document document,
            PreprocessorDirectiveFilter directiveFilter,
            CancellationToken cancellationToken = default)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            SourceText newSourceText = RemovePreprocessorDirectives(sourceText, root.DescendantPreprocessorDirectives(), directiveFilter);

            return document.WithText(newSourceText);
        }

        /// <summary>
        /// Creates a new document with preprocessor directives of the specified kind removed.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="span"></param>
        /// <param name="directiveFilter"></param>
        /// <param name="cancellationToken"></param>
        public static async Task<Document> RemovePreprocessorDirectivesAsync(
            this Document document,
            TextSpan span,
            PreprocessorDirectiveFilter directiveFilter,
            CancellationToken cancellationToken = default)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            SourceText newSourceText = RemovePreprocessorDirectives(sourceText, root.DescendantPreprocessorDirectives(span), directiveFilter);

            return document.WithText(newSourceText);
        }

        internal static async Task<Document> RemovePreprocessorDirectivesAsync(
            this Document document,
            IEnumerable<DirectiveTriviaSyntax> directives,
            CancellationToken cancellationToken = default)
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
            PreprocessorDirectiveFilter directiveFilter)
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
                        return (directiveFilter & PreprocessorDirectiveFilter.If) != 0;
                    case SyntaxKind.ElifDirectiveTrivia:
                        return (directiveFilter & PreprocessorDirectiveFilter.Elif) != 0;
                    case SyntaxKind.ElseDirectiveTrivia:
                        return (directiveFilter & PreprocessorDirectiveFilter.Else) != 0;
                    case SyntaxKind.EndIfDirectiveTrivia:
                        return (directiveFilter & PreprocessorDirectiveFilter.EndIf) != 0;
                    case SyntaxKind.RegionDirectiveTrivia:
                        return (directiveFilter & PreprocessorDirectiveFilter.Region) != 0;
                    case SyntaxKind.EndRegionDirectiveTrivia:
                        return (directiveFilter & PreprocessorDirectiveFilter.EndRegion) != 0;
                    case SyntaxKind.DefineDirectiveTrivia:
                        return (directiveFilter & PreprocessorDirectiveFilter.Define) != 0;
                    case SyntaxKind.UndefDirectiveTrivia:
                        return (directiveFilter & PreprocessorDirectiveFilter.Undef) != 0;
                    case SyntaxKind.ErrorDirectiveTrivia:
                        return (directiveFilter & PreprocessorDirectiveFilter.Error) != 0;
                    case SyntaxKind.WarningDirectiveTrivia:
                        return (directiveFilter & PreprocessorDirectiveFilter.Warning) != 0;
                    case SyntaxKind.LineDirectiveTrivia:
                        return (directiveFilter & PreprocessorDirectiveFilter.Line) != 0;
                    case SyntaxKind.PragmaWarningDirectiveTrivia:
                        return (directiveFilter & PreprocessorDirectiveFilter.PragmaWarning) != 0;
                    case SyntaxKind.PragmaChecksumDirectiveTrivia:
                        return (directiveFilter & PreprocessorDirectiveFilter.PragmaChecksum) != 0;
                    case SyntaxKind.ReferenceDirectiveTrivia:
                        return (directiveFilter & PreprocessorDirectiveFilter.Reference) != 0;
                    case SyntaxKind.BadDirectiveTrivia:
                        return (directiveFilter & PreprocessorDirectiveFilter.Bad) != 0;
                    case SyntaxKind.ShebangDirectiveTrivia:
                        return (directiveFilter & PreprocessorDirectiveFilter.Shebang) != 0;
                    case SyntaxKind.LoadDirectiveTrivia:
                        return (directiveFilter & PreprocessorDirectiveFilter.Load) != 0;
                    case SyntaxKind.NullableDirectiveTrivia:
                        return (directiveFilter & PreprocessorDirectiveFilter.Nullable) != 0;
                }

                SyntaxDebug.Fail(directive);
                return false;
            }
        }

        /// <summary>
        /// Creates a new document with the specified region removed.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="region"></param>
        /// <param name="cancellationToken"></param>
        public static async Task<Document> RemoveRegionAsync(
            this Document document,
            RegionInfo region,
            CancellationToken cancellationToken = default)
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

            SourceText newSourceText = sourceText.WithChange(span, "");

            return document.WithText(newSourceText);
        }

        internal static Task<Document> RemoveSingleLineDocumentationComment(
            this Document document,
            DocumentationCommentTriviaSyntax documentationComment,
            CancellationToken cancellationToken = default)
        {
            SyntaxNode node = documentationComment.ParentTrivia.Token.Parent;
            SyntaxNode newNode = SyntaxRefactorings.RemoveSingleLineDocumentationComment(node, documentationComment);

            return document.ReplaceNodeAsync(node, newNode, cancellationToken);
        }

        /// <summary>
        /// Creates a new document with the specified statements replaced with new statements.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="statementsInfo"></param>
        /// <param name="newStatements"></param>
        /// <param name="cancellationToken"></param>
        public static Task<Document> ReplaceStatementsAsync(
            this Document document,
            StatementListInfo statementsInfo,
            IEnumerable<StatementSyntax> newStatements,
            CancellationToken cancellationToken = default)
        {
            return ReplaceStatementsAsync(document, statementsInfo, List(newStatements), cancellationToken);
        }

        /// <summary>
        /// Creates a new document with the specified statements replaced with new statements.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="statementsInfo"></param>
        /// <param name="newStatements"></param>
        /// <param name="cancellationToken"></param>
        public static Task<Document> ReplaceStatementsAsync(
            this Document document,
            StatementListInfo statementsInfo,
            SyntaxList<StatementSyntax> newStatements,
            CancellationToken cancellationToken = default)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            return document.ReplaceNodeAsync(statementsInfo.Parent, statementsInfo.WithStatements(newStatements).Parent, cancellationToken);
        }

        internal static Task<Document> ReplaceStatementsAsync(
            this Document document,
            StatementListInfo statementsInfo,
            StatementListInfo newStatementsInfo,
            CancellationToken cancellationToken = default)
        {
            return document.ReplaceNodeAsync(statementsInfo.Parent, newStatementsInfo.Parent, cancellationToken);
        }

        /// <summary>
        /// Creates a new document with the specified members replaced with new members.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="info"></param>
        /// <param name="newMembers"></param>
        /// <param name="cancellationToken"></param>
        public static Task<Document> ReplaceMembersAsync(
            this Document document,
            MemberDeclarationListInfo info,
            IEnumerable<MemberDeclarationSyntax> newMembers,
            CancellationToken cancellationToken = default)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            return document.ReplaceNodeAsync(
                info.Parent,
                info.WithMembers(newMembers).Parent,
                cancellationToken);
        }

        /// <summary>
        /// Creates a new document with the specified members replaced with new members.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="info"></param>
        /// <param name="newMembers"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<Document> ReplaceMembersAsync(
            this Document document,
            MemberDeclarationListInfo info,
            SyntaxList<MemberDeclarationSyntax> newMembers,
            CancellationToken cancellationToken = default)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            return document.ReplaceNodeAsync(
                info.Parent,
                info.WithMembers(newMembers).Parent,
                cancellationToken);
        }

        /// <summary>
        /// Creates a new document with the specified modifiers replaced with new modifiers.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="modifiersInfo"></param>
        /// <param name="newModifiers"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<Document> ReplaceModifiersAsync(
            this Document document,
            ModifierListInfo modifiersInfo,
            IEnumerable<SyntaxToken> newModifiers,
            CancellationToken cancellationToken = default)
        {
            return ReplaceModifiersAsync(document, modifiersInfo, TokenList(newModifiers), cancellationToken);
        }

        /// <summary>
        /// Creates a new document with the specified modifiers replaced with new modifiers.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="modifiersInfo"></param>
        /// <param name="newModifiers"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<Document> ReplaceModifiersAsync(
            this Document document,
            ModifierListInfo modifiersInfo,
            SyntaxTokenList newModifiers,
            CancellationToken cancellationToken = default)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            return document.ReplaceNodeAsync(modifiersInfo.Parent, modifiersInfo.WithModifiers(newModifiers).Parent, cancellationToken);
        }
    }
}
