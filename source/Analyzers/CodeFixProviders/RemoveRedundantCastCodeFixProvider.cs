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
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveRedundantCastCodeFixProvider))]
    [Shared]
    public class RemoveRedundantCastCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.RemoveRedundantCast); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            SyntaxNode node = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf(f => f.IsKind(SyntaxKind.CastExpression, SyntaxKind.InvocationExpression));

            Debug.Assert(node != null, $"{nameof(node)} is null");

            if (node == null)
                return;

            switch (node.Kind())
            {
                case SyntaxKind.CastExpression:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            "Remove redundant cast",
                            cancellationToken => RemoveRedundantCastRefactoring.RefactorAsync(context.Document, (CastExpressionSyntax)node, cancellationToken),
                            DiagnosticIdentifiers.RemoveRedundantCast + EquivalenceKeySuffix);

                        context.RegisterCodeFix(codeAction, context.Diagnostics);
                        break;
                    }
                case SyntaxKind.InvocationExpression:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            "Remove redundant cast",
                            cancellationToken => RemoveRedundantCastRefactoring.RefactorAsync(context.Document, (InvocationExpressionSyntax)node, cancellationToken),
                            DiagnosticIdentifiers.RemoveRedundantCast + EquivalenceKeySuffix);

                        context.RegisterCodeFix(codeAction, context.Diagnostics);
                        break;
                    }
            }
        }
    }
}