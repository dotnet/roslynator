// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.Refactorings;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConvertHasFlagCallToBitwiseOperationCodeFixProvider))]
    [Shared]
    public class ConvertHasFlagCallToBitwiseOperationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.ConvertHasFlagCallToBitwiseOperationOrViceVersa); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out ExpressionSyntax expression))
                return;

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];

            if (expression is InvocationExpressionSyntax invocationExpression)
            {
                CodeAction codeAction = CodeAction.Create(
                    ConvertHasFlagCallToBitwiseOperationRefactoring.Title,
                    ct => ConvertHasFlagCallToBitwiseOperationRefactoring.RefactorAsync(document, invocationExpression, ct),
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
            else
            {
                Debug.Assert(expression.IsKind(SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression), expression.Kind().ToString());

                CodeAction codeAction = CodeAction.Create(
                    "Call 'HasFlag'",
                    ct => ConvertBitwiseOperationToHasFlagCallAsync(document, (BinaryExpressionSyntax)expression, ct),
                    GetEquivalenceKey(diagnostic));

                context.RegisterCodeFix(codeAction, diagnostic);
            }
        }

        private async Task<Document> ConvertBitwiseOperationToHasFlagCallAsync(
            Document document,
            BinaryExpressionSyntax equalsOrNotEquals,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax left = equalsOrNotEquals.Left.WalkDownParentheses();
            ExpressionSyntax right = equalsOrNotEquals.Right.WalkDownParentheses();

            BinaryExpressionSyntax bitwiseAnd = (left.IsKind(SyntaxKind.BitwiseAndExpression))
                ? (BinaryExpressionSyntax)left
                : (BinaryExpressionSyntax)right;

            ExpressionSyntax expression;
            ExpressionSyntax argumentExpression;

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            if (ConvertHasFlagCallToBitwiseOperationAnalysis.IsSuitableAsArgumentOfHasFlag(bitwiseAnd.Right, semanticModel, cancellationToken))
            {
                expression = bitwiseAnd.Left;
                argumentExpression = bitwiseAnd.Right;
            }
            else
            {
                expression = bitwiseAnd.Right;
                argumentExpression = bitwiseAnd.Left;
            }

            ExpressionSyntax newExpression = SimpleMemberInvocationExpression(
                expression,
                IdentifierName("HasFlag"),
                ArgumentList(Argument(argumentExpression.WalkDownParentheses()))).Parenthesize();

            if (equalsOrNotEquals.IsKind(SyntaxKind.EqualsExpression))
                newExpression = LogicalNotExpression(newExpression).Parenthesize();

            newExpression = newExpression.WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(equalsOrNotEquals, newExpression, cancellationToken).ConfigureAwait(false);
        }
    }
}
