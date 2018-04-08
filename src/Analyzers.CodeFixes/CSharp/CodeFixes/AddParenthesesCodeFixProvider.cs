// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
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

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out ExpressionSyntax expression, predicate: f => f.IsKind(SyntaxKind.ConditionalExpression) || f is BinaryExpressionSyntax))
                return;

            CodeAction codeAction = CodeAction.Create(
                $"Parenthesize '{expression}'",
                cancellationToken => AddParenthesesAccordingToOperatorPrecedenceRefactoring.RefactorAsync(context.Document, expression, cancellationToken),
                GetEquivalenceKey(DiagnosticIdentifiers.AddParenthesesAccordingToOperatorPrecedence));

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}