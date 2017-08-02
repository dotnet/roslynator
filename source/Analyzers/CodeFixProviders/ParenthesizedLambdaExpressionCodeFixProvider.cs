// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ParenthesizedLambdaExpressionCodeFixProvider))]
    [Shared]
    public class ParenthesizedLambdaExpressionCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.SimplifyLambdaExpressionParameterList); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out ParenthesizedLambdaExpressionSyntax lambda))
                return;

            CodeAction codeAction = CodeAction.Create(
                "Simplify lambda expression parameter list",
                cancellationToken => SimplifyLambdaExpressionParameterListRefactoring.RefactorAsync(context.Document, lambda, cancellationToken),
                GetEquivalenceKey(DiagnosticIdentifiers.SimplifyLambdaExpressionParameterList));

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}
