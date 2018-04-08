// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
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

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out SyntaxNode node, predicate: f => f.IsKind(SyntaxKind.LogicalAndExpression, SyntaxKind.IfStatement)))
                return;

            switch (node.Kind())
            {
                case SyntaxKind.LogicalAndExpression:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            Title,
                            cancellationToken => UseConditionalAccessRefactoring.RefactorAsync(context.Document, (BinaryExpressionSyntax)node, cancellationToken),
                            GetEquivalenceKey(DiagnosticIdentifiers.UseConditionalAccess));

                        context.RegisterCodeFix(codeAction, context.Diagnostics);
                        break;
                    }
                case SyntaxKind.IfStatement:
                    {
                        CodeAction codeAction = CodeAction.Create(
                            Title,
                            cancellationToken => UseConditionalAccessRefactoring.RefactorAsync(context.Document, (IfStatementSyntax)node, cancellationToken),
                            GetEquivalenceKey(DiagnosticIdentifiers.UseConditionalAccess));

                        context.RegisterCodeFix(codeAction, context.Diagnostics);
                        break;
                    }
            }
        }
    }
}