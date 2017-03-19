// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes.Extensions;
using Roslynator.CSharp.Extensions;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DeclarationExpressionCodeFixProvider))]
    [Shared]
    public class DeclarationExpressionCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.UseExplicitTypeInsteadOfVar,
                    DiagnosticIdentifiers.UseExplicitTypeInsteadOfVarEvenIfObvious);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            DeclarationExpressionSyntax declarationExpression = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<DeclarationExpressionSyntax>();

            if (declarationExpression == null)
                return;

            TypeSyntax type = declarationExpression.Type;

            if (type.IsVar)
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                var localSymbol = semanticModel.GetDeclaredSymbol(declarationExpression.Designation, context.CancellationToken) as ILocalSymbol;

                if (localSymbol != null)
                {
                    ITypeSymbol typeSymbol = localSymbol.Type;

                    if (typeSymbol != null)
                    {
                        CodeAction codeAction = CodeAction.Create(
                            $"Change type to '{SymbolDisplay.GetMinimalString(typeSymbol, semanticModel, type.SpanStart)}'",
                            cancellationToken => ChangeTypeRefactoring.ChangeTypeAsync(context.Document, type, typeSymbol, cancellationToken),
                            DiagnosticIdentifiers.UseExplicitTypeInsteadOfVar + EquivalenceKeySuffix);

                        context.RegisterCodeFix(codeAction, context.Diagnostics);
                    }
                }
            }
        }
    }
}