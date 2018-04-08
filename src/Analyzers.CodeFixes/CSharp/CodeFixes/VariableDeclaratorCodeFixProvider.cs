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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(VariableDeclaratorCodeFixProvider))]
    [Shared]
    public class VariableDeclaratorCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.MergeLocalDeclarationWithAssignment,
                    DiagnosticIdentifiers.RemoveRedundantFieldInitialization);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out VariableDeclaratorSyntax declarator))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.MergeLocalDeclarationWithAssignment:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Merge local declaration with assignment",
                                cancellationToken => MergeLocalDeclarationWithAssignmentRefactoring.RefactorAsync(context.Document, declarator, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.RemoveRedundantFieldInitialization:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Remove redundant initialization",
                                cancellationToken => RemoveRedundantFieldInitializationRefactoring.RefactorAsync(context.Document, declarator, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
