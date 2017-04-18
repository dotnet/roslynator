// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseStringComparisonCodeFixProvider))]
    [Shared]
    public class UseStringComparisonCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.UseStringComparison); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            SyntaxNode node = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf(f => f.IsKind(
                    SyntaxKind.EqualsExpression,
                    SyntaxKind.NotEqualsExpression,
                    SyntaxKind.InvocationExpression));

            Debug.Assert(node != null, $"{nameof(node)} is null");

            if (node == null)
                return;

            switch (node.Kind())
            {
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                    {
                        var binaryExpression = (BinaryExpressionSyntax)node;

                        context.RegisterCodeFix(CreateCodeAction(context, binaryExpression, StringComparison.CurrentCultureIgnoreCase), context.Diagnostics);
                        context.RegisterCodeFix(CreateCodeAction(context, binaryExpression, StringComparison.OrdinalIgnoreCase), context.Diagnostics);
                        break;
                    }
                case SyntaxKind.InvocationExpression:
                    {
                        var invocation = (InvocationExpressionSyntax)node;

                        context.RegisterCodeFix(CreateCodeAction(context, invocation, StringComparison.CurrentCultureIgnoreCase), context.Diagnostics);
                        context.RegisterCodeFix(CreateCodeAction(context, invocation, StringComparison.OrdinalIgnoreCase), context.Diagnostics);
                        break;
                    }
            }
        }

        private static CodeAction CreateCodeAction(CodeFixContext context, BinaryExpressionSyntax binaryExpression, StringComparison stringComparison)
        {
            return CodeAction.Create(
                GetTitle(stringComparison),
                cancellationToken => UseStringComparisonRefactoring.RefactorAsync(context.Document, binaryExpression, stringComparison, cancellationToken),
                DiagnosticIdentifiers.UseStringComparison + stringComparison + EquivalenceKeySuffix);
        }

        private static CodeAction CreateCodeAction(CodeFixContext context, InvocationExpressionSyntax invocation, StringComparison stringComparison)
        {
            return CodeAction.Create(
                GetTitle(stringComparison),
                cancellationToken => UseStringComparisonRefactoring.RefactorAsync(context.Document, invocation, stringComparison, cancellationToken),
                DiagnosticIdentifiers.UseStringComparison + stringComparison + EquivalenceKeySuffix);
        }

        private static string GetTitle(StringComparison stringComparison)
        {
            return $"Use 'StringComparison.{stringComparison}'";
        }
    }
}