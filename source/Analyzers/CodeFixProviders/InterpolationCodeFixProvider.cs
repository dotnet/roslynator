// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InterpolationCodeFixProvider))]
    [Shared]
    public class InterpolationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.MergeInterpolationIntoInterpolatedString); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            InterpolationSyntax interpolation = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<InterpolationSyntax>();

            Debug.Assert(interpolation != null, $"{nameof(interpolation)} is null");

            if (interpolation == null)
                return;

            string innerText = ((LiteralExpressionSyntax)interpolation.Expression).GetStringLiteralInnerText();

            CodeAction codeAction = CodeAction.Create(
                $"Merge '{innerText}' into interpolated string",
                cancellationToken => MergeInterpolationIntoInterpolatedStringRefactoring.RefactorAsync(context.Document, interpolation, cancellationToken),
                DiagnosticIdentifiers.MergeInterpolationIntoInterpolatedString + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}
