// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
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

            SyntaxNode node = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf(SyntaxKind.VariableDeclaration, SyntaxKind.DeclarationExpression);

            Debug.Assert(node != null, $"{nameof(node)} is null");

            if (node == null)
                return;

            if (node.IsKind(SyntaxKind.VariableDeclaration))
            {
                var variableDeclaration = (VariableDeclarationSyntax)node;

                TypeSyntax type = variableDeclaration.Type;

                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, context.CancellationToken);

                RegisterCodeFix(context, type, typeSymbol, semanticModel);
            }
            else
            {
                var declarationExpression = (DeclarationExpressionSyntax)node;

                TypeSyntax type = declarationExpression.Type;

                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                var localSymbol = semanticModel.GetDeclaredSymbol(declarationExpression.Designation, context.CancellationToken) as ILocalSymbol;

                RegisterCodeFix(context, type, localSymbol.Type, semanticModel);
            }
        }

        private static void RegisterCodeFix(CodeFixContext context, TypeSyntax type, ITypeSymbol typeSymbol, SemanticModel semanticModel)
        {
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