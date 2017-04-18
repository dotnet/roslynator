// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveImplementationFromAbstractMemberCodeFixProvider))]
    [Shared]
    public class RemoveImplementationFromAbstractMemberCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.RemoveImplementationFromAbstractMember); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            SyntaxNode node = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf(f => f.IsKind(SyntaxKind.ArrowExpressionClause, SyntaxKind.AccessorList, SyntaxKind.Block));

            Debug.Assert(node != null, $"{nameof(node)} is null");

            if (node == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                $"Remove {GetName(node)}",
                cancellationToken => RemoveImplementationFromAbstractMemberRefactoring.RefactorAsync(context.Document, node.Parent, cancellationToken),
                DiagnosticIdentifiers.RemoveImplementationFromAbstractMember + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static string GetName(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.Block:
                    return "body";
                case SyntaxKind.ArrowExpressionClause:
                    return "expression body";
                case SyntaxKind.AccessorList:
                    return "accessors";
            }

            Debug.Assert(false, node.Kind().ToString());
            return "";
        }
    }
}