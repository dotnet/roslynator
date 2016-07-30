// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
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
using Microsoft.CodeAnalysis.Text;
using Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers;
using Pihrtsoft.CodeAnalysis.CSharp.Removers;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FormatAccessorListCodeFixProvider))]
    [Shared]
    public class FormatAccessorListCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.FormatAccessorList);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            AccessorListSyntax accessorList = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<AccessorListSyntax>();

            if (accessorList == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Format accessor list",
                cancellationToken => FormatAccessorListAsync(context.Document, accessorList, cancellationToken),
                DiagnosticIdentifiers.FormatAccessorList + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> FormatAccessorListAsync(
            Document document,
            AccessorListSyntax accessorList,
            CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            if (accessorList.Accessors.All(f => f.Body == null))
            {
                var propertyDeclaration = (PropertyDeclarationSyntax)accessorList.Parent;

                TextSpan span = TextSpan.FromBounds(
                    propertyDeclaration.Identifier.Span.End,
                    accessorList.CloseBraceToken.Span.Start);

                PropertyDeclarationSyntax newPropertyDeclaration =
                    WhitespaceOrEndOfLineRemover.RemoveFrom(propertyDeclaration, span);

                newPropertyDeclaration = newPropertyDeclaration
                    .WithFormatterAnnotation();

                root = root.ReplaceNode(propertyDeclaration, newPropertyDeclaration);
            }
            else
            {
                AccessorListSyntax newAccessorList = GetNewAccessorList(accessorList)
                    .WithFormatterAnnotation();

                root = root.ReplaceNode(accessorList, newAccessorList);
            }

            return document.WithSyntaxRoot(root);
        }

        private static AccessorListSyntax GetNewAccessorList(AccessorListSyntax accessorList)
        {
            if (accessorList.IsSingleLine(includeExteriorTrivia: false))
            {
                SyntaxTriviaList triviaList = accessorList.CloseBraceToken.LeadingTrivia
                    .Add(CSharpFactory.NewLine);

                return WhitespaceOrEndOfLineRemover.RemoveFrom(accessorList)
                    .WithCloseBraceToken(accessorList.CloseBraceToken.WithLeadingTrivia(triviaList));
            }
            else
            {
                return AccessorSyntaxRewriter.VisitNode(accessorList);
            }
        }

        private class AccessorSyntaxRewriter : CSharpSyntaxRewriter
        {
            private static readonly AccessorSyntaxRewriter _instance = new AccessorSyntaxRewriter();

            private AccessorSyntaxRewriter()
            {
            }

            public static AccessorListSyntax VisitNode(AccessorListSyntax accessorList)
            {
                return (AccessorListSyntax)_instance.Visit(accessorList);
            }

            public override SyntaxNode VisitAccessorDeclaration(AccessorDeclarationSyntax node)
            {
                if (node == null)
                    throw new ArgumentNullException(nameof(node));

                if (AccessorListDiagnosticAnalyzer.ShouldBeFormatted(node))
                    return WhitespaceOrEndOfLineRemover.RemoveFrom(node, node.Span);

                return base.VisitAccessorDeclaration(node);
            }
        }
    }
}
