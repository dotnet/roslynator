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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseNameOfOperatorCodeFixProvider))]
    [Shared]
    public class UseNameOfOperatorCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.UseNameOfOperator); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out LiteralExpressionSyntax node))
                return;

            string identifier = context.Diagnostics[0].Properties["Identifier"];

            CodeAction codeAction = CodeAction.Create(
                "Use nameof operator",
                cancellationToken => UseNameOfOperatorRefactoring.RefactorAsync(context.Document, node, identifier, cancellationToken),
                GetEquivalenceKey(DiagnosticIdentifiers.UseNameOfOperator));

            context.RegisterCodeFix(codeAction, context.Diagnostics[0]);
        }
    }
}
