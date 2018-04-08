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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AvoidNullReferenceExceptionCodeFixProvider))]
    [Shared]
    public class AvoidNullReferenceExceptionCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.AvoidNullReferenceException); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out ExpressionSyntax expression, predicate: f => f.IsKind(SyntaxKind.SimpleMemberAccessExpression, SyntaxKind.ElementAccessExpression)))
                return;

            if (!IsPartOfLeftSideOfAssignment(expression))
            {
                SyntaxKind kind = expression.Kind();

                if (kind == SyntaxKind.SimpleMemberAccessExpression)
                {
                    expression = ((MemberAccessExpressionSyntax)expression).Expression;
                }
                else if (kind == SyntaxKind.ElementAccessExpression)
                {
                    expression = ((ElementAccessExpressionSyntax)expression).Expression;
                }

                CodeAction codeAction = CodeAction.Create(
                    "Use conditional access",
                    cancellationToken => AvoidNullReferenceExceptionRefactoring.RefactorAsync(context.Document, expression, cancellationToken),
                    GetEquivalenceKey(DiagnosticIdentifiers.AvoidNullReferenceException));

                context.RegisterCodeFix(codeAction, context.Diagnostics);
            }
        }

        private static bool IsPartOfLeftSideOfAssignment(ExpressionSyntax expression)
        {
            for (SyntaxNode node = expression; node != null; node = node.Parent)
            {
                var assignmentExpression = node.Parent as AssignmentExpressionSyntax;

                if (assignmentExpression?.Left == node)
                    return true;
            }

            return false;
        }
    }
}
