// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

            ThrowStatementSyntax throwStatement = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<ThrowStatementSyntax>();

            Debug.Assert(throwStatement != null, $"{nameof(throwStatement)} is null");

            if (throwStatement == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Remove throw statement",
                cancellationToken => Remover.RemoveStatementAsync(context.Document, throwStatement, cancellationToken),
                DiagnosticIdentifiers.ThrowingOfNewNotImplementedException + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}