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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TypeCodeFixProvider))]
    [Shared]
    public class TypeCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.UseGenericEventHandler); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out TypeSyntax type))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            GenericNameSyntax newType = UseGenericEventHandlerRefactoring.CreateGenericEventHandler(type, semanticModel, context.CancellationToken);

            CodeAction codeAction = CodeAction.Create(
                $"Use '{newType}'",
                cancellationToken => UseGenericEventHandlerRefactoring.RefactorAsync(context.Document, type, cancellationToken),
                GetEquivalenceKey(DiagnosticIdentifiers.UseGenericEventHandler));

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}