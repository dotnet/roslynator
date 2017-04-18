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
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseConditionalAccessCodeFixProvider))]
    [Shared]
    public class UseConditionalAccessCodeFixProvider : BaseCodeFixProvider
    {
        private const string Title = "Use conditional access";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.UseConditionalAccess); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            SyntaxNode node = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf(SyntaxKind.LogicalAndExpression, SyntaxKind.IfStatement);

            Debug.Assert(node != null, $"{nameof(node)} is null");

            if (node == null)
                return;

            switch (node.Kind())
            {
                case SyntaxKind.LogicalAndExpression:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            Title,
                            cancellationToken => UseConditionalAccessRefactoring.RefactorAsync(context.Document, (BinaryExpressionSyntax)node, cancellationToken),
                            DiagnosticDescriptors.UseConditionalAccess + EquivalenceKeySuffix);

                        context.RegisterCodeFix(codeAction, context.Diagnostics);
                        break;
                    }
                case SyntaxKind.IfStatement:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            Title,
                            cancellationToken => UseConditionalAccessRefactoring.RefactorAsync(context.Document, (IfStatementSyntax)node, cancellationToken),
                            DiagnosticDescriptors.UseConditionalAccess + EquivalenceKeySuffix);

                        context.RegisterCodeFix(codeAction, context.Diagnostics);
                        break;
                    }
            }
        }
    }
}