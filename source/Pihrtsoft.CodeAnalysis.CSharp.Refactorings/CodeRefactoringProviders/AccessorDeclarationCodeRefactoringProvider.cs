// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(AccessorDeclarationCodeRefactoringProvider))]
    public class AccessorDeclarationCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            AccessorDeclarationSyntax accessor = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<AccessorDeclarationSyntax>();

            if (accessor == null)
                return;

            if (accessor.Body?.Span.Contains(context.Span) == true
                && !accessor.Body.OpenBraceToken.IsMissing
                && !accessor.Body.CloseBraceToken.IsMissing
                && accessor.Body.IsSingleline())
            {
                CodeAction codeAction = CodeAction.Create(
                    "Format braces on multiple lines",
                    cancellationToken => CreateChangedDocumentAsync(context.Document, accessor, cancellationToken));

                context.RegisterRefactoring(codeAction);
            }
        }

        private static async Task<Document> CreateChangedDocumentAsync(
            Document document,
            AccessorDeclarationSyntax accessor,
            CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            SyntaxToken closeBrace = accessor.Body.CloseBraceToken;

            AccessorDeclarationSyntax newAccessor = accessor
                .WithBody(
                    accessor.Body.WithCloseBraceToken(
                        closeBrace.WithLeadingTrivia(
                            closeBrace.LeadingTrivia.Add(SyntaxHelper.NewLine))))
                .WithAdditionalAnnotations(Formatter.Annotation);

            root = root.ReplaceNode(accessor, newAccessor);

            return document.WithSyntaxRoot(root);
        }
    }
}
