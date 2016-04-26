// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SpecifyExplicitTypeCodeFixProvider))]
    [Shared]
    public class SpecifyExplicitTypeCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.DeclareExplicitType,
                    DiagnosticIdentifiers.DeclareExplicitTypeEvenIfObvious);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            VariableDeclarationSyntax variableDeclaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<VariableDeclarationSyntax>();

            if (variableDeclaration == null)
                return;

            if (variableDeclaration.Type.IsVar)
            {
                SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

                if (semanticModel == null)
                    return;

                ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(variableDeclaration.Type).Type;

                CodeAction codeAction = CodeAction.Create(
                    $"Change type to '{typeSymbol.ToDisplayString(TypeSyntaxRefactoring.SymbolDisplayFormat)}'",
                    cancellationToken => TypeSyntaxRefactoring.ChangeTypeToExplicitAsync(context.Document, variableDeclaration.Type, typeSymbol, cancellationToken),
                    DiagnosticIdentifiers.DeclareExplicitType + EquivalenceKeySuffix);

                context.RegisterCodeFix(codeAction, context.Diagnostics);
            }
        }
    }
}