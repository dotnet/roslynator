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
using Roslynator.CSharp;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddParenthesesCodeFixProvider))]
    [Shared]
    public class AddParenthesesCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.AddParenthesesAccordingToOperatorPrecedence); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            var expression = (ExpressionSyntax)root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf(f => f.IsKind(SyntaxKind.ConditionalExpression) || f is BinaryExpressionSyntax);

            Debug.Assert(expression != null, $"{nameof(expression)} is null");

            if (expression == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                $"Parenthesize '{expression}'",
                cancellationToken => AddParenthesesAccordingToOperatorPrecedenceRefactoring.RefactorAsync(context.Document, expression, cancellationToken),
                DiagnosticIdentifiers.AddParenthesesAccordingToOperatorPrecedence + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}