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
using Roslynator;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EnumDeclarationCodeFixProvider))]
    [Shared]
    public class EnumDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.FormatEachEnumMemberOnSeparateLine);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            EnumDeclarationSyntax enumDeclaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<EnumDeclarationSyntax>();

            if (enumDeclaration == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Format each enum member on a separate line",
                cancellationToken => FormatEachEnumMemberOnSeparateLineAsync(context.Document, enumDeclaration, cancellationToken),
                DiagnosticIdentifiers.FormatEachEnumMemberOnSeparateLine + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> FormatEachEnumMemberOnSeparateLineAsync(
            Document document,
            EnumDeclarationSyntax enumDeclaration,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var rewriter = new EnumDeclarationSyntaxRewriter(enumDeclaration);

            SyntaxNode newNode = rewriter.Visit(enumDeclaration)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(enumDeclaration, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private class EnumDeclarationSyntaxRewriter : CSharpSyntaxRewriter
        {
            private readonly SyntaxToken[] _separators;

            public EnumDeclarationSyntaxRewriter(EnumDeclarationSyntax enumDeclaration)
            {
                _separators = enumDeclaration.Members.GetSeparators().ToArray();
            }

            public override SyntaxToken VisitToken(SyntaxToken token)
            {
                if (_separators.Contains(token))
                {
                    SyntaxTriviaList triviaList = token.TrailingTrivia;

                    if (!triviaList.Contains(SyntaxKind.EndOfLineTrivia))
                        return token.WithTrailingTrivia(triviaList.TrimEnd().Add(CSharpFactory.NewLineTrivia()));
                }

                return base.VisitToken(token);
            }
        }
    }
}
