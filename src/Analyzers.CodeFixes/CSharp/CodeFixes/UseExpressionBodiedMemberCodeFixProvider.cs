// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseExpressionBodiedMemberCodeFixProvider))]
    [Shared]
    public class UseExpressionBodiedMemberCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.UseExpressionBodiedMember); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out SyntaxNode node, predicate: f => CSharpFacts.CanHaveExpressionBody(f.Kind())))
                return;

            CodeAction codeAction = CodeAction.Create(
                "Use expression-bodied member",
                cancellationToken => UseExpressionBodiedMemberRefactoring.RefactorAsync(context.Document, node, cancellationToken),
                GetEquivalenceKey(DiagnosticIdentifiers.UseExpressionBodiedMember));

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}
