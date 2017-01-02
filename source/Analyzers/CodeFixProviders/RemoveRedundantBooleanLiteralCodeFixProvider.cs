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

            LiteralExpressionSyntax literalExpression = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<LiteralExpressionSyntax>();

            Debug.Assert(literalExpression != null, $"{nameof(literalExpression)} is null");

            if (literalExpression == null)
                return;

            Debug.Assert(literalExpression.IsBooleanLiteralExpression(), literalExpression.Kind().ToString());

            if (!literalExpression.IsBooleanLiteralExpression())
                return;

            SyntaxNode parent = literalExpression.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.ForStatement:
                    {
                        RegisterCodeFix(
                            context,
                            cancellationToken =>
                            {
                                return RemoveRedundantBooleanLiteralRefactoring.RefactorAsync(
                                    context.Document,
                                    (ForStatementSyntax)parent,
                                    cancellationToken);
                            });

                        break;
                    }
                case SyntaxKind.LogicalAndExpression:
                case SyntaxKind.LogicalOrExpression:
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                    {
                        RegisterCodeFix(
                            context,
                            cancellationToken =>
                            {
                                return RemoveRedundantBooleanLiteralRefactoring.RefactorAsync(
                                    context.Document,
                                    (BinaryExpressionSyntax)parent,
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