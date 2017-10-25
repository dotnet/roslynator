// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.CSharp.Comparers;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ChangeOverridingMemberAccessibilityCodeFixProvider))]
    [Shared]
    public class ChangeOverridingMemberAccessibilityCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.CannotChangeAccessModifiersWhenOverridingInheritedMember); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.ChangeAccessibility))
                return;

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(
                root,
                context.Span,
                out SyntaxNode node,
                predicate: f => OverrideInfo.CanCreate(f)))
            {
                return;
            }

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.CannotChangeAccessModifiersWhenOverridingInheritedMember:
                        {
                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            OverrideInfo overrideInfo = OverrideInfo.Create(node, semanticModel, context.CancellationToken);

                            if (!overrideInfo.Success)
                                break;

                            Accessibility newAccessibility = overrideInfo.OverriddenSymbol.DeclaredAccessibility;

                            CodeAction codeAction = CodeAction.Create(
                                $"Change accessibility to '{newAccessibility.GetName()}'",
                                cancellationToken =>
                                {
                                    if (node.Kind() == SyntaxKind.VariableDeclarator)
                                        node = node.Parent.Parent;

                                    SyntaxNode newNode = node.WithAccessibility(newAccessibility, ModifierComparer.Instance);

                                    return context.Document.ReplaceNodeAsync(node, newNode, cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
