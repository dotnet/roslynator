// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseConditionalAccessToAvoidNullReferenceExceptionCodeFixProvider))]
    [Shared]
    public class UseConditionalAccessToAvoidNullReferenceExceptionCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.UseConditionalAccessToAvoidNullReferenceException); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            //TODO: TryFindFirstAncestorOrSelf
            SyntaxNode node = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf(SyntaxKind.SimpleMemberAccessExpression, SyntaxKind.ElementAccessExpression);

            if (node == null)
                return;

            ExpressionSyntax expression = null;

            switch (node.Kind())
            {
                case SyntaxKind.SimpleMemberAccessExpression:
                    {
                        expression = ((MemberAccessExpressionSyntax)node).Expression;
                        break;
                    }
                case SyntaxKind.ElementAccessExpression:
                    {
                        expression = ((ElementAccessExpressionSyntax)node).Expression;
                        break;
                    }
            }

            CodeAction codeAction = CodeAction.Create(
                "Use conditional access",
                cancellationToken => UseConditionalAccessToAvoidNullReferenceExceptionRefactoring.RefactorAsync(context.Document, expression, cancellationToken),
                DiagnosticIdentifiers.UseConditionalAccessToAvoidNullReferenceException + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}
