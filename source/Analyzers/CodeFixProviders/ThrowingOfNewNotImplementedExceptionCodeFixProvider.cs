// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ThrowingOfNewNotImplementedExceptionCodeFixProvider))]
    [Shared]
    public class ThrowingOfNewNotImplementedExceptionCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.ThrowingOfNewNotImplementedException); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            SyntaxNode node = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf(f => f.IsKind(SyntaxKind.ThrowStatement, SyntaxKind.ThrowExpression));

            Debug.Assert(node != null, $"{nameof(node)} is null");

            if (node == null)
                return;

            if (node.IsKind(SyntaxKind.ThrowStatement))
            {
                CodeAction codeAction = CodeAction.Create(
                    "Remove throw statement",
                    cancellationToken => Remover.RemoveStatementAsync(context.Document, (ThrowStatementSyntax)node, cancellationToken),
                    DiagnosticIdentifiers.ThrowingOfNewNotImplementedException + EquivalenceKeySuffix);

                context.RegisterCodeFix(codeAction, context.Diagnostics);
            }
        }
    }
}