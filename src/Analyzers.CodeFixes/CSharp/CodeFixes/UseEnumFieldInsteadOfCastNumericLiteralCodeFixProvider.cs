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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseEnumFieldInsteadOfCastNumericLiteralCodeFixProvider))]
    [Shared]
    public class UseEnumFieldInsteadOfCastNumericLiteralCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(DiagnosticIdentifiers.UseEnumFieldInsteadOfCastNumericLiteral);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.GetSyntaxRootAsync().ConfigureAwait(false);
            if (!TryFindFirstAncestorOrSelf(root, context.Span, out CastExpressionSyntax castExpression))
            {
                return;
            }

            var codeAction = CodeAction.Create(
                "Use enumeration field",
                cancellationToken => UseEnumFieldInsteadOfCastNumericLiteralRefactoring.RefactorAsync(context.Document, castExpression, cancellationToken),
                GetEquivalenceKey(DiagnosticIdentifiers.UseEnumFieldInsteadOfCastNumericLiteral));

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}
