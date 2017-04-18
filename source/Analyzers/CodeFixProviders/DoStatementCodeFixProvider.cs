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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DoStatementCodeFixProvider))]
    [Shared]
    public class DoStatementCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.AvoidUsageOfDoStatementToCreateInfiniteLoop);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            DoStatementSyntax doStatement = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<DoStatementSyntax>();

            if (doStatement == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Use while to create an infinite loop",
                cancellationToken =>
                {
                    return ReplaceDoStatementWithWhileStatementRefactoring.RefactorAsync(
                        context.Document,
                        doStatement,
                        cancellationToken);
                },
                DiagnosticIdentifiers.AvoidUsageOfDoStatementToCreateInfiniteLoop + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}
