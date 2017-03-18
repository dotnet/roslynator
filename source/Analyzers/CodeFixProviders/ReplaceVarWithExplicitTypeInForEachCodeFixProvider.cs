// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes.Extensions;
using Roslynator.CSharp.Extensions;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ReplaceVarWithExplicitTypeInForEachCodeFixProvider))]
    [Shared]
    public class ReplaceVarWithExplicitTypeInForEachCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.UseExplicitTypeInsteadOfVarInForEach); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            ForEachStatementSyntax forEachStatement = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<ForEachStatementSyntax>();

            if (forEachStatement == null)
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            TypeSyntax type = forEachStatement.Type;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, context.CancellationToken);

            if (typeSymbol != null)
            {
                CodeAction codeAction = CodeAction.Create(
                    $"Change type to '{SymbolDisplay.GetMinimalString(typeSymbol, semanticModel, type.SpanStart)}'",
                    cancellationToken => UseExplicitTypeInsteadOfVarRefactoring.RefactorAsync(context.Document, type, typeSymbol, cancellationToken),
                    DiagnosticIdentifiers.UseExplicitTypeInsteadOfVarInForEach + EquivalenceKeySuffix);

                context.RegisterCodeFix(codeAction, context.Diagnostics);
            }
        }
    }
}