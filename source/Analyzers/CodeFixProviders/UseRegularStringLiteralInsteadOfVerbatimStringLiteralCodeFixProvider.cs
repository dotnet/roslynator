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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseRegularStringLiteralInsteadOfVerbatimStringLiteralCodeFixProvider))]
    [Shared]
    public class UseRegularStringLiteralInsteadOfVerbatimStringLiteralCodeFixProvider : BaseCodeFixProvider
    {
        private const string Title = "Remove @";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.UseRegularStringLiteralInsteadOfVerbatimStringLiteral); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            SyntaxNode node = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf(SyntaxKind.StringLiteralExpression, SyntaxKind.InterpolatedStringExpression);

            Debug.Assert(node != null, $"{nameof(node)} is null");

            if (node == null)
                return;

            switch (node.Kind())
            {
                case SyntaxKind.StringLiteralExpression:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            Title,
                            cancellationToken => UseRegularStringLiteralInsteadOfVerbatimStringLiteralRefactoring.RefactorAsync(context.Document, (LiteralExpressionSyntax)node, cancellationToken),
                            DiagnosticIdentifiers.UseRegularStringLiteralInsteadOfVerbatimStringLiteral + EquivalenceKeySuffix);

                        context.RegisterCodeFix(codeAction, context.Diagnostics);
                        break;
                    }
                case SyntaxKind.InterpolatedStringExpression:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            Title,
                            cancellationToken => UseRegularStringLiteralInsteadOfVerbatimStringLiteralRefactoring.RefactorAsync(context.Document, (InterpolatedStringExpressionSyntax)node, cancellationToken),
                            DiagnosticIdentifiers.UseRegularStringLiteralInsteadOfVerbatimStringLiteral + EquivalenceKeySuffix);

                        context.RegisterCodeFix(codeAction, context.Diagnostics);
                        break;
                    }
            }
        }
    }
}