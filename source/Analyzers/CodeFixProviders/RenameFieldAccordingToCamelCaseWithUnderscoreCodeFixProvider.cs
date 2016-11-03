// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RenameFieldAccordingToCamelCaseWithUnderscoreCodeFixProvider))]
    [Shared]
    public class RenameFieldAccordingToCamelCaseWithUnderscoreCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.RenamePrivateFieldAccordingToCamelCaseWithUnderscore);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            VariableDeclaratorSyntax declarator = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<VariableDeclaratorSyntax>();

            if (declarator == null)
                return;

            if (context.Document.SupportsSemanticModel)
            {
                SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

                ISymbol symbol = semanticModel.GetDeclaredSymbol(declarator, context.CancellationToken);

                string newName = TextUtility.ToCamelCaseWithUnderscore(declarator.Identifier.ValueText);

                CodeAction codeAction = CodeAction.Create(
                    $"Rename field to '{newName}'",
                    cancellationToken => SymbolRenamer.RenameAsync(context.Document, symbol, newName, cancellationToken),
                    DiagnosticIdentifiers.RenamePrivateFieldAccordingToCamelCaseWithUnderscore);

                context.RegisterCodeFix(codeAction, context.Diagnostics);
            }
        }
    }
}