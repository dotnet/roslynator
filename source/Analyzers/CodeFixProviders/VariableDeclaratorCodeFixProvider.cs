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

            VariableDeclaratorSyntax declarator = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<VariableDeclaratorSyntax>();

            Debug.Assert(declarator != null, $"{nameof(declarator)} is null");

            if (declarator == null)
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
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.RemoveRedundantFieldInitialization:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Remove redundant field initialization",
                                cancellationToken => RemoveRedundantFieldInitializationRefactoring.RefactorAsync(context.Document, declarator, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
