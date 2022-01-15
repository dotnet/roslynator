// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AccessorDeclarationCodeFixProvider))]
    [Shared]
    public sealed class AccessorDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.PutFullAccessorOnItsOwnLine,
                    DiagnosticIdentifiers.FormatAccessorBraces,
                    DiagnosticIdentifiers.FormatAccessorBracesOnSingleLineWhenExpressionIsOnSingleLine);
            }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out AccessorDeclarationSyntax accessorDeclaration))
                return;

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            switch (diagnostic.Id)
            {
                case DiagnosticIdentifiers.FormatAccessorBraces:
                case DiagnosticIdentifiers.FormatAccessorBracesOnSingleLineWhenExpressionIsOnSingleLine:
                    {
                        bool isSingleLine = accessorDeclaration.IsSingleLine(includeExteriorTrivia: false);
                        string title = (isSingleLine)
                            ? "Format braces on multiple lines"
                            : "Format braces on a single line";

                        CodeAction codeAction = CodeAction.Create(
                            title,
                            ct => (isSingleLine)
                                ? FormatAccessorBracesOnMultipleLinesAsync(document, accessorDeclaration, ct)
                                : FormatAccessorBracesOnSingleLineAsync(document, accessorDeclaration, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case DiagnosticIdentifiers.PutFullAccessorOnItsOwnLine:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            "Put accessor on its own line",
                            ct => PutAccessorOnItsOwnLineAsync(accessorDeclaration, document, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
            }
        }

        private static Task<Document> FormatAccessorBracesOnSingleLineAsync(
            Document document,
            AccessorDeclarationSyntax accessor,
            CancellationToken cancellationToken)
        {
            AccessorDeclarationSyntax newAccessorDeclaration = accessor
                .RemoveWhitespace(accessor.Span)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(accessor, newAccessorDeclaration, cancellationToken);
        }

        private static Task<Document> FormatAccessorBracesOnMultipleLinesAsync(
            Document document,
            AccessorDeclarationSyntax accessor,
            CancellationToken cancellationToken)
        {
            BlockSyntax body = accessor.Body;

            AccessorDeclarationSyntax newAccessorDeclaration = accessor
                .WithBody(body.WithCloseBraceToken(body.CloseBraceToken.AppendEndOfLineToLeadingTrivia()))
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(accessor, newAccessorDeclaration, cancellationToken);
        }

        private static Task<Document> PutAccessorOnItsOwnLineAsync(
            AccessorDeclarationSyntax accessorDeclaration,
            Document document,
            CancellationToken cancellationToken)
        {
            AccessorDeclarationSyntax newAccessorDeclaration = accessorDeclaration.AppendEndOfLineToLeadingTrivia();

            var accessorList = (AccessorListSyntax)accessorDeclaration.Parent;

            AccessorListSyntax newAccessorList = accessorList
                .WithAccessors(accessorList.Accessors.Replace(accessorDeclaration, newAccessorDeclaration))
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(accessorList, newAccessorList, cancellationToken);
        }
    }
}
