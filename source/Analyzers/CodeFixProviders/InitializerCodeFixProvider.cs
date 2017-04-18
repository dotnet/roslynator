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
                    DiagnosticIdentifiers.UseCSharp6DictionaryInitializer);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            InitializerExpressionSyntax initializer = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<InitializerExpressionSyntax>();

            Debug.Assert(initializer != null, $"{nameof(initializer)} is null");

            if (initializer == null)
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
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.UseCSharp6DictionaryInitializer:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Use C# 6.0 dictionary initializer",
                                cancellationToken => UseCSharp6DictionaryInitializerRefactoring.RefactorAsync(context.Document, initializer, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
