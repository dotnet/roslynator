// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseExplicitTypeInsteadOfVarInForEachCodeFixProvider))]
    [Shared]
    public class UseExplicitTypeInsteadOfVarInForEachCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.UseExplicitTypeInsteadOfVarInForEach); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out ForEachStatementSyntax forEachStatement))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            TypeSyntax type = forEachStatement.Type;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, context.CancellationToken);

            CodeAction codeAction = CodeAction.Create(
                $"Change type to '{SymbolDisplay.ToMinimalDisplayString(typeSymbol, semanticModel, type.SpanStart, SymbolDisplayFormats.Default)}'",
                cancellationToken => ChangeTypeRefactoring.ChangeTypeAsync(context.Document, type, typeSymbol, cancellationToken),
                GetEquivalenceKey(DiagnosticIdentifiers.UseExplicitTypeInsteadOfVarInForEach));

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}