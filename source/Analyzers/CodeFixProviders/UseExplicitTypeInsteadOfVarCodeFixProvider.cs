// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseExplicitTypeInsteadOfVarCodeFixProvider))]
    [Shared]
    public class UseExplicitTypeInsteadOfVarCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.UseExplicitTypeInsteadOfVarWhenTypeIsNotObvious,
                    DiagnosticIdentifiers.UseExplicitTypeInsteadOfVarWhenTypeIsObvious);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            VariableDeclarationSyntax variableDeclaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<VariableDeclarationSyntax>();

            Debug.Assert(variableDeclaration != null, $"{nameof(variableDeclaration)} is null");

            if (variableDeclaration == null)
                return;

            TypeSyntax type = variableDeclaration.Type;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, context.CancellationToken);

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                CodeAction codeAction = CodeAction.Create(
                    $"Change type to '{SymbolDisplay.GetMinimalString(typeSymbol, semanticModel, type.Span.Start)}'",
                    cancellationToken => ChangeTypeRefactoring.ChangeTypeAsync(context.Document, type, typeSymbol, cancellationToken),
                    diagnostic.Id + EquivalenceKeySuffix);

                context.RegisterCodeFix(codeAction, context.Diagnostics);
            }
        }
    }
}