// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.CSharp.Refactorings;
using Roslynator.Extensions;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveRedundantBooleanLiteralCodeFixProvider))]
    [Shared]
    public class RemoveRedundantBooleanLiteralCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.RemoveRedundantBooleanLiteral); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            SyntaxNode node = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf(f => f.IsKind(
                    SyntaxKind.TrueLiteralExpression,
                    SyntaxKind.EqualsExpression,
                    SyntaxKind.NotEqualsExpression,
                    SyntaxKind.LogicalAndExpression,
                    SyntaxKind.LogicalOrExpression));

            Debug.Assert(node != null, $"{nameof(node)} is null");

            if (node == null)
                return;

            switch (node.Kind())
            {
                case SyntaxKind.TrueLiteralExpression:
                    {
                        RegisterCodeFix(
                            context,
                            cancellationToken =>
                            {
                                return RemoveRedundantBooleanLiteralRefactoring.RefactorAsync(
                                    context.Document,
                                    (ForStatementSyntax)node.Parent,
                                    cancellationToken);
                            });

                        break;
                    }
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                case SyntaxKind.LogicalAndExpression:
                case SyntaxKind.LogicalOrExpression:
                    {
                        RegisterCodeFix(
                            context,
                            cancellationToken =>
                            {
                                return RemoveRedundantBooleanLiteralRefactoring.RefactorAsync(
                                    context.Document,
                                    (BinaryExpressionSyntax)node,
                                    cancellationToken);
                            });

                        break;
                    }
            }
        }

        private static void RegisterCodeFix(CodeFixContext context, Func<CancellationToken, Task<Document>> createChangedDocument)
        {
            CodeAction codeAction = CodeAction.Create(
                "Remove redundant boolean literal",
                createChangedDocument,
                DiagnosticIdentifiers.RemoveRedundantBooleanLiteral + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}