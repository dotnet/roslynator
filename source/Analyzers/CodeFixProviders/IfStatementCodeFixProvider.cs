// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IfStatementCodeFixProvider))]
    [Shared]
    public class IfStatementCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.MergeIfStatementWithNestedIfStatement,
                    DiagnosticIdentifiers.SimplifyIfElseStatement);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            IfStatementSyntax ifStatement = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<IfStatementSyntax>();

            if (ifStatement == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.MergeIfStatementWithNestedIfStatement:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Merge if with nested if",
                                cancellationToken =>
                                {
                                    return MergeIfStatementWithNestedIfStatementRefactoring.RefactorAsync(
                                        context.Document,
                                        ifStatement,
                                        cancellationToken);
                                },
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.SimplifyIfElseStatement:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Simplify if-else",
                                cancellationToken =>
                                {
                                    return SimplifyIfElseStatementRefactoring.RefactorAsync(
                                        context.Document,
                                        ifStatement,
                                        cancellationToken);
                                },
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
