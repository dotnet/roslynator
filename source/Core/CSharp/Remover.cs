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
using Roslynator.CSharp.Extensions;
using Roslynator.CSharp.Refactorings;
using Roslynator.CSharp.SyntaxRewriters;
using Roslynator.Extensions;

namespace Roslynator.CSharp
{
    public static class Remover
    {
        public static SyntaxRemoveOptions DefaultRemoveOptions
        {
            get { return SyntaxRemoveOptions.KeepExteriorTrivia | SyntaxRemoveOptions.KeepUnbalancedDirectives; }
        }

        public static async Task<Document> RemoveMemberAsync(
            Document document,
            MemberDeclarationSyntax member,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            if (member.Parent.IsKind(SyntaxKind.CompilationUnit))
            {
                var compilationUnit = (CompilationUnitSyntax)member.Parent;

                return await document.ReplaceNodeAsync(compilationUnit, compilationUnit.RemoveMember(member)).ConfigureAwait(false);
            }
            else
            {
                var parentMember = (MemberDeclarationSyntax)member.Parent;

                if (parentMember != null)
                {
                    return await document.ReplaceNodeAsync(parentMember, parentMember.RemoveMember(member)).ConfigureAwait(false);
                }
                else
                {
                    return await document.RemoveNodeAsync(member, DefaultRemoveOptions).ConfigureAwait(false);
                }
            }
        }

        public static MemberDeclarationSyntax RemoveMemberAt(this MemberDeclarationSyntax containingMember, int index)
        {
            if (containingMember == null)
                throw new ArgumentNullException(nameof(containingMember));

            switch (containingMember.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    return ((NamespaceDeclarationSyntax)containingMember).RemoveMemberAt(index);
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)containingMember).RemoveMemberAt(index);
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)containingMember).RemoveMemberAt(index);
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)containingMember).RemoveMemberAt(index);
            }

            return containingMember;
        }

        public static MemberDeclarationSyntax RemoveMember(this MemberDeclarationSyntax containingMember, MemberDeclarationSyntax member)
        {
            if (containingMember == null)
                throw new ArgumentNullException(nameof(containingMember));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            switch (containingMember.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    return ((NamespaceDeclarationSyntax)containingMember).RemoveMember(member);
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)containingMember).RemoveMember(member);
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)containingMember).RemoveMember(member);
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)containingMember).RemoveMember(member);
            }

            return containingMember;
        }

        public static MemberDeclarationSyntax RemoveMemberAt(this ClassDeclarationSyntax containingMember, int index)
        {
            if (containingMember == null)
                throw new ArgumentNullException(nameof(containingMember));

            return RemoveMember(containingMember, containingMember.Members[index], index);
        }

        public static MemberDeclarationSyntax RemoveMember(this ClassDeclarationSyntax containingMember, MemberDeclarationSyntax member)
        {
            if (containingMember == null)
                throw new ArgumentNullException(nameof(containingMember));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return RemoveMember(containingMember, member, containingMember.Members.IndexOf(member));
        }

        private static MemberDeclarationSyntax RemoveMember(
            ClassDeclarationSyntax classDeclaration,
            MemberDeclarationSyntax member,
            int index)
        {
            MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

            classDeclaration = classDeclaration
                .WithMembers(classDeclaration.Members.Replace(member, newMember));

            return classDeclaration
                .RemoveNode(classDeclaration.Members[index], GetRemoveOptions(newMember));
        }

        public static MemberDeclarationSyntax RemoveMemberAt(this InterfaceDeclarationSyntax interfaceDeclaration, int index)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return RemoveMember(interfaceDeclaration, interfaceDeclaration.Members[index], index);
        }

        public static MemberDeclarationSyntax RemoveMember(this InterfaceDeclarationSyntax interfaceDeclaration, MemberDeclarationSyntax member)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return RemoveMember(interfaceDeclaration, member, interfaceDeclaration.Members.IndexOf(member));
        }

        private static MemberDeclarationSyntax RemoveMember(
            InterfaceDeclarationSyntax interfaceDeclaration,
            MemberDeclarationSyntax member,
            int index)
        {
            MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

            interfaceDeclaration = interfaceDeclaration
                .WithMembers(interfaceDeclaration.Members.Replace(member, newMember));

            return interfaceDeclaration
                .RemoveNode(interfaceDeclaration.Members[index], GetRemoveOptions(newMember));
        }

        public static MemberDeclarationSyntax RemoveMemberAt(this StructDeclarationSyntax structDeclaration, int index)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            return RemoveMember(structDeclaration, structDeclaration.Members[index], index);
        }

        public static MemberDeclarationSyntax RemoveMember(this StructDeclarationSyntax structDeclaration, MemberDeclarationSyntax member)
        {
            if (structDeclaration == null)
                throw new ArgumentNullException(nameof(structDeclaration));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return RemoveMember(structDeclaration, member, structDeclaration.Members.IndexOf(member));
        }

        private static MemberDeclarationSyntax RemoveMember(
            StructDeclarationSyntax structDeclaration,
            MemberDeclarationSyntax member,
            int index)
        {
            MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

            structDeclaration = structDeclaration
                .WithMembers(structDeclaration.Members.Replace(member, newMember));

            return structDeclaration
                .RemoveNode(structDeclaration.Members[index], GetRemoveOptions(newMember));
        }

        public static CompilationUnitSyntax RemoveMemberAt(this CompilationUnitSyntax compilationUnit, int index)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            return RemoveMember(compilationUnit, compilationUnit.Members[index], index);
        }

        public static CompilationUnitSyntax RemoveMember(this CompilationUnitSyntax compilationUnit, MemberDeclarationSyntax member)
        {
            if (compilationUnit == null)
                throw new ArgumentNullException(nameof(compilationUnit));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return RemoveMember(compilationUnit, member, compilationUnit.Members.IndexOf(member));
        }

        private static CompilationUnitSyntax RemoveMember(
            CompilationUnitSyntax compilationUnit,
            MemberDeclarationSyntax member,
            int index)
        {
            MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

            compilationUnit = compilationUnit
                .WithMembers(compilationUnit.Members.Replace(member, newMember));

            return compilationUnit
                .RemoveNode(compilationUnit.Members[index], GetRemoveOptions(newMember));
        }

        public static MemberDeclarationSyntax RemoveMemberAt(this NamespaceDeclarationSyntax namespaceDeclaration, int index)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            return RemoveMember(namespaceDeclaration, namespaceDeclaration.Members[index], index);
        }

        public static MemberDeclarationSyntax RemoveMember(this NamespaceDeclarationSyntax namespaceDeclaration, MemberDeclarationSyntax member)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return RemoveMember(namespaceDeclaration, member, namespaceDeclaration.Members.IndexOf(member));
        }

        private static MemberDeclarationSyntax RemoveMember(
            NamespaceDeclarationSyntax namespaceDeclaration,
            MemberDeclarationSyntax member,
            int index)
        {
            MemberDeclarationSyntax newMember = RemoveSingleLineDocumentationComment(member);

            namespaceDeclaration = namespaceDeclaration
                .WithMembers(namespaceDeclaration.Members.Replace(member, newMember));

            return namespaceDeclaration
                .RemoveNode(namespaceDeclaration.Members[index], GetRemoveOptions(newMember));
        }

        internal static MemberDeclarationSyntax RemoveSingleLineDocumentationComment(MemberDeclarationSyntax member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            SyntaxTriviaList leadingTrivia = member.GetLeadingTrivia();

            SyntaxTriviaList.Reversed.Enumerator en = leadingTrivia.Reverse().GetEnumerator();

            int i = 0;
            while (en.MoveNext())
            {
                if (en.Current.IsWhitespaceOrEndOfLineTrivia())
                {
                    i++;
                }
                else if (en.Current.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                {
                    return member.WithLeadingTrivia(leadingTrivia.Take(leadingTrivia.Count - (i + 1)));
                }
                else
                {
                    return member;
                }
            }

            return member;
        }

        public static TNode RemoveStatement<TNode>(TNode node, StatementSyntax statement) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            return node.RemoveNode(statement, GetRemoveOptions(statement));
        }

        public static Task<Document> RemoveCommentAsync(
            Document document,
            SyntaxTrivia comment,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return RemoveCommentRefactoring.RefactorAsync(document, comment, cancellationToken);
        }

        public static async Task<Document> RemoveCommentsAsync(
            Document document,
            CommentRemoveOptions removeOptions,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = RemoveComments(root, removeOptions, span)
                .WithFormatterAnnotation();

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> RemoveCommentsAsync(
            Document document,
            CommentRemoveOptions removeOptions,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = RemoveComments(root, removeOptions)
                .WithFormatterAnnotation();

            return document.WithSyntaxRoot(newRoot);
        }

        public static TNode RemoveComments<TNode>(TNode node, CommentRemoveOptions removeOptions) where TNode : SyntaxNode
        {
            return CommentRemover.RemoveComments(node, removeOptions);
        }

        public static TNode RemoveComments<TNode>(TNode node, CommentRemoveOptions removeOptions, TextSpan span) where TNode : SyntaxNode
        {
            return CommentRemover.RemoveComments(node, removeOptions, span);
        }

        public static TNode RemoveTrivia<TNode>(TNode node) where TNode : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return TriviaRemover.RemoveTrivia(node);
        }

        public static async Task<Document> RemoveTriviaAsync(
            Document document,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = TriviaRemover.RemoveTrivia(root, span);

            return document.WithSyntaxRoot(newRoot);
        }

        public static TNode RemoveWhitespaceOrEndOfLine<TNode>(TNode node) where TNode : SyntaxNode
        {
            return WhitespaceOrEndOfLineRemover.RemoveWhitespaceOrEndOfLine(node);
        }

        public static TNode RemoveWhitespaceOrEndOfLine<TNode>(TNode node, TextSpan span) where TNode : SyntaxNode
        {
            return WhitespaceOrEndOfLineRemover.RemoveWhitespaceOrEndOfLine(node, span);
        }

        public static async Task<SyntaxTree> RemoveDirectivesAsync(
            SyntaxTree syntaxTree,
            IEnumerable<DirectiveTriviaSyntax> directives,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (syntaxTree == null)
                throw new ArgumentNullException(nameof(syntaxTree));

            if (directives == null)
                throw new ArgumentNullException(nameof(directives));

            SourceText sourceText = await syntaxTree.GetTextAsync(cancellationToken).ConfigureAwait(false);

            SourceText newSourceText = RemoveDirectives(sourceText, directives);

            return syntaxTree.WithChangedText(newSourceText);
        }

        public static async Task<SyntaxTree> RemoveDirectivesAsync(
            SyntaxTree syntaxTree,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (syntaxTree == null)
                throw new ArgumentNullException(nameof(syntaxTree));

            SyntaxNode root = await syntaxTree.GetRootAsync(cancellationToken).ConfigureAwait(false);

            SourceText sourceText = await syntaxTree.GetTextAsync(cancellationToken).ConfigureAwait(false);

            SourceText newSourceText = RemoveDirectives(sourceText, root.DescendantDirectives());

            return syntaxTree.WithChangedText(newSourceText);
        }

        public static async Task<Document> RemoveDirectivesAsync(
            Document document,
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

        public static async Task<Document> RemoveDirectivesAsync(
            Document document,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            SourceText newSourceText = RemoveDirectives(sourceText, root.DescendantDirectives());

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

                return await RemoveRegionAsync(document, regionDirective, endRegionDirective, cancellationToken).ConfigureAwait(false);
            }

            return document;
        }

        public static async Task<Document> RemoveRegionAsync(
            Document document,
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

            var textChange = new TextChange(span, string.Empty);

            SourceText newSourceText = sourceText.WithChanges(textChange);

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

            return await RemoveDirectivesAsync(syntaxTree, root.DescendantRegionDirectives(), cancellationToken).ConfigureAwait(false);
        }

        public static async Task<Document> RemoveRegionDirectivesAsync(
            Document document,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            return await RemoveDirectivesAsync(document, root.DescendantRegionDirectives(), cancellationToken).ConfigureAwait(false);
        }

        public static SyntaxNode RemoveEmptyNamespaces(SyntaxNode node, SyntaxRemoveOptions removeOptions)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            IEnumerable<NamespaceDeclarationSyntax> emptyNamespaces = node
                .DescendantNodes()
                .Where(f => f.IsKind(SyntaxKind.NamespaceDeclaration))
                .Cast<NamespaceDeclarationSyntax>()
                .Where(f => !f.Members.Any());

            return node.RemoveNodes(emptyNamespaces, removeOptions);
        }

        private static SyntaxRemoveOptions GetRemoveOptions(CSharpSyntaxNode member)
        {
            SyntaxRemoveOptions removeOptions = DefaultRemoveOptions;

            if (member.GetLeadingTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                removeOptions &= ~SyntaxRemoveOptions.KeepLeadingTrivia;

            if (member.GetTrailingTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                removeOptions &= ~SyntaxRemoveOptions.KeepTrailingTrivia;

            return removeOptions;
        }
    }
}
