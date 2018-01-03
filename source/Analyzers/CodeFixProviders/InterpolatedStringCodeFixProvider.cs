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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InterpolatedStringCodeFixProvider))]
    [Shared]
    public class InterpolatedStringCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.AvoidInterpolatedStringWithNoInterpolation,
                    DiagnosticIdentifiers.AvoidInterpolatedStringWithNoInterpolatedText);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out InterpolatedStringExpressionSyntax interpolatedString))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.AvoidInterpolatedStringWithNoInterpolation:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Remove $",
                                cancellationToken => ReplaceInterpolatedStringWithStringLiteralRefactoring.RefactorAsync(context.Document, interpolatedString, cancellationToken),
                                GetEquivalenceKey(diagnostic.Id));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.AvoidInterpolatedStringWithNoInterpolatedText:
                        {
                            string title = (interpolatedString.Contents.Count == 1)
                                ? "Extract expression from interpolated string"
                                : "Extract expressions from interpolated string";

                            CodeAction codeAction = CodeAction.Create(
                                title,
                                cancellationToken => AvoidInterpolatedStringWithNoInterpolatedTextRefactoring.RefactorAsync(context.Document, interpolatedString, cancellationToken),
                                GetEquivalenceKey(diagnostic.Id));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
