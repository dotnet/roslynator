// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UsePredefinedTypeCodeFixProvider))]
    [Shared]
    public class UsePredefinedTypeCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.UsePredefinedType); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            SyntaxNode node = root.FindNode(context.Span, findInsideTrivia: true, getInnermostNodeForTie: true);

            if (node?.IsKind(
                    SyntaxKind.QualifiedName,
                    SyntaxKind.IdentifierName,
                    SyntaxKind.SimpleMemberAccessExpression) == true)
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                var typeSymbol = semanticModel.GetSymbol(node, context.CancellationToken) as INamedTypeSymbol;

                if (typeSymbol?.SupportsPredefinedType() == true)
                {
                    CodeAction codeAction = CodeAction.Create(
                        $"Use predefined type '{SymbolDisplay.GetString(typeSymbol)}'",
                        cancellationToken => UsePredefinedTypeRefactoring.RefactorAsync(context.Document, node, typeSymbol, cancellationToken),
                        DiagnosticIdentifiers.UsePredefinedType + EquivalenceKeySuffix);

                    context.RegisterCodeFix(codeAction, context.Diagnostics);
                }
            }
        }
    }
}
