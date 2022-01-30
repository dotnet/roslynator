// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ChangeOverridingMemberAccessibilityCodeFixProvider))]
    [Shared]
    public sealed class ChangeOverridingMemberAccessibilityCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.CS0507_CannotChangeAccessModifiersWhenOverridingInheritedMember); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            Diagnostic diagnostic = context.Diagnostics[0];

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.ChangeAccessibility, context.Document, root.SyntaxTree))
                return;

            if (!TryFindFirstAncestorOrSelf(
                root,
                context.Span,
                out SyntaxNode node,
                predicate: f => CSharpOverriddenSymbolInfo.CanCreate(f)))
            {
                return;
            }

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            OverriddenSymbolInfo overrideInfo = CSharpOverriddenSymbolInfo.Create(node, semanticModel, context.CancellationToken);

            if (!overrideInfo.Success)
                return;

            Accessibility newAccessibility = overrideInfo.OverriddenSymbol.DeclaredAccessibility;

            CodeAction codeAction = CodeAction.Create(
                $"Change accessibility to '{SyntaxFacts.GetText(newAccessibility)}'",
                ct =>
                {
                    if (node.IsKind(SyntaxKind.VariableDeclarator))
                    {
                        node = node.Parent.Parent;
                    }

                    SyntaxNode newNode;

                    if (newAccessibility == Accessibility.Public
                        && node is AccessorDeclarationSyntax)
                    {
                        newNode = SyntaxAccessibility.WithoutExplicitAccessibility(node);
                    }
                    else
                    {
                        newNode = SyntaxAccessibility.WithExplicitAccessibility(node, newAccessibility);
                    }

                    return context.Document.ReplaceNodeAsync(node, newNode, ct);
                },
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }
    }
}
