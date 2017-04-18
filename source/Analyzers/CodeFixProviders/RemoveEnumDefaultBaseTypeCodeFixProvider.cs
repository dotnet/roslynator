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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveEnumDefaultBaseTypeCodeFixProvider))]
    [Shared]
    public class RemoveEnumDefaultBaseTypeCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.RemoveEnumDefaultUnderlyingType); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            SimpleBaseTypeSyntax baseType = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<SimpleBaseTypeSyntax>();

            if (baseType == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Remove default underlying type",
                cancellationToken => RemoveEnumDefaultUnderlyingTypeRefactoring.RefactorAsync(context.Document, baseType, cancellationToken),
                DiagnosticIdentifiers.RemoveEnumDefaultUnderlyingType + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}
