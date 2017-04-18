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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AnonymousMethodCodeFixProvider))]
    [Shared]
    public class AnonymousMethodCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.UseLambdaExpressionInsteadOfAnonymousMethod); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            AnonymousMethodExpressionSyntax anonymousMethod = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<AnonymousMethodExpressionSyntax>();

            if (anonymousMethod == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Replace anonymous method with lambda expression",
                cancellationToken => UseLambdaExpressionInsteadOfAnonymousMethodRefactoring.RefactorAsync(context.Document, anonymousMethod, cancellationToken),
                DiagnosticIdentifiers.UseLambdaExpressionInsteadOfAnonymousMethod + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}
