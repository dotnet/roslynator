// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AccessorListCodeFixProvider))]
    [Shared]
    public sealed class AccessorListCodeFixProvider : BaseCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.PutAutoAccessorsOnSingleLine); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out AccessorListSyntax accessorList))
                return;

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            switch (diagnostic.Id)
            {
                case DiagnosticIdentifiers.PutAutoAccessorsOnSingleLine:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            (accessorList.Accessors.Count == 1) ? "Put accessor on a single line" : "Put accessors on a single line",
                            ct => PutAccessorsOnSingleLine(document, accessorList, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
            }
        }

        private static Task<Document> PutAccessorsOnSingleLine(
            Document document,
            AccessorListSyntax accessorList,
            CancellationToken cancellationToken)
        {
            TextSpan span = TextSpan.FromBounds(
                accessorList.GetFirstToken().GetPreviousToken().Span.End,
                accessorList.CloseBraceToken.SpanStart);

            SyntaxNode newNode = accessorList.Parent
                .RemoveWhitespace(span)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(accessorList.Parent, newNode, cancellationToken);
        }
    }
}
