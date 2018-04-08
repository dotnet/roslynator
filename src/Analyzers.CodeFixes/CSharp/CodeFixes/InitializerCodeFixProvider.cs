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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InitializerCodeFixProvider))]
    [Shared]
    public class InitializerCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.RemoveRedundantCommaInInitializer,
                    DiagnosticIdentifiers.FormatInitializerWithSingleExpressionOnSingleLine);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out InitializerExpressionSyntax initializer))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.RemoveRedundantCommaInInitializer:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Remove redundant comma",
                                cancellationToken => RemoveRedundantCommaInInitializerRefactoring.RefactorAsync(context.Document, initializer, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.FormatInitializerWithSingleExpressionOnSingleLine:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Format initializer on a single line",
                                cancellationToken => FormatInitializerWithSingleExpressionOnSingleLineRefactoring.RefactorAsync(context.Document, initializer, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
