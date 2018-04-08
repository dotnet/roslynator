// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FormatBinaryOperatorOnNewLineCodeFixProvider))]
    [Shared]
    public class FormatBinaryOperatorOnNewLineCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.FormatBinaryOperatorOnNextLine); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out BinaryExpressionSyntax binaryExpression))
                return;

            CodeAction codeAction = CodeAction.Create(
                "Format binary operator on next line",
                cancellationToken => FormatBinaryOperatorOnNextLineRefactoring.RefactorAsync(context.Document, binaryExpression, cancellationToken),
                GetEquivalenceKey(DiagnosticIdentifiers.FormatBinaryOperatorOnNextLine));

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}