// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
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

            if (!TryFindNode(root, context.Span, out SyntaxNode node, findInsideTrivia: true, predicate: f => f.IsKind(
                SyntaxKind.QualifiedName,
                SyntaxKind.IdentifierName,
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxKind.NameMemberCref,
                SyntaxKind.QualifiedCref)))
            {
                return;
            }

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            var typeSymbol = semanticModel.GetSymbol(node, context.CancellationToken) as INamedTypeSymbol;

            CodeAction codeAction = CodeAction.Create(
                $"Use predefined type '{SymbolDisplay.ToDisplayString(typeSymbol, SymbolDisplayFormats.Default)}'",
                cancellationToken => UsePredefinedTypeRefactoring.RefactorAsync(context.Document, node, typeSymbol, cancellationToken),
                GetEquivalenceKey(DiagnosticIdentifiers.UsePredefinedType));

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}
