// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SimplifyLogicalNegationCodeFixProvider))]
    [Shared]
    public class SimplifyLogicalNegationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.SimplifyLogicalNegation); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out PrefixUnaryExpressionSyntax prefixUnaryExpression))
                return;

            Diagnostic diagnostic = context.Diagnostics[0];

            Document document = context.Document;

            CodeAction codeAction = CodeAction.Create(
                "Simplify logical negation",
                ct => SimplifyLogicalNegationAsync(document, prefixUnaryExpression, ct),
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        private static Task<Document> SimplifyLogicalNegationAsync(
            Document document,
            PrefixUnaryExpressionSyntax logicalNot,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionSyntax newNode = GetNewNode(logicalNot)
                .WithTriviaFrom(logicalNot)
                .WithSimplifierAnnotation();

            return document.ReplaceNodeAsync(logicalNot, newNode, cancellationToken);
        }

        private static ExpressionSyntax GetNewNode(PrefixUnaryExpressionSyntax logicalNot)
        {
            ExpressionSyntax operand = logicalNot.Operand;
            ExpressionSyntax expression = operand.WalkDownParentheses();

            switch (expression.Kind())
            {
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                    {
                        LiteralExpressionSyntax newNode = BooleanLiteralExpression(expression.Kind() == SyntaxKind.FalseLiteralExpression);

                        newNode = newNode.WithTriviaFrom(expression);

                        return operand.ReplaceNode(expression, newNode);
                    }
                case SyntaxKind.LogicalNotExpression:
                    {
                        return ((PrefixUnaryExpressionSyntax)expression).Operand;
                    }
                case SyntaxKind.EqualsExpression:
                    {
                        var equalsExpression = (BinaryExpressionSyntax)expression;

                        BinaryExpressionSyntax notEqualsExpression = NotEqualsExpression(
                            equalsExpression.Left,
                            SyntaxFactory.Token(SyntaxKind.ExclamationEqualsToken).WithTriviaFrom(equalsExpression.OperatorToken),
                            equalsExpression.Right);

                        return operand.ReplaceNode(equalsExpression, notEqualsExpression);
                    }
                case SyntaxKind.NotEqualsExpression:
                    {
                        var notEqualsExpression = (BinaryExpressionSyntax)expression;

                        BinaryExpressionSyntax equalsExpression = NotEqualsExpression(
                            notEqualsExpression.Left,
                            SyntaxFactory.Token(SyntaxKind.EqualsEqualsToken).WithTriviaFrom(notEqualsExpression.OperatorToken),
                            notEqualsExpression.Right);

                        return operand.ReplaceNode(notEqualsExpression, equalsExpression);
                    }
                case SyntaxKind.InvocationExpression:
                    {
                        var invocationExpression = (InvocationExpressionSyntax)expression;

                        var memberAccessExpression = (MemberAccessExpressionSyntax)invocationExpression.Expression;

                        ExpressionSyntax lambdaExpression = invocationExpression.ArgumentList.Arguments.First().Expression.WalkDownParentheses();

                        SingleParameterLambdaExpressionInfo lambdaInfo = SyntaxInfo.SingleParameterLambdaExpressionInfo(lambdaExpression);

                        var logicalNot2 = (PrefixUnaryExpressionSyntax)SimplifyLogicalNegationAnalyzer.GetReturnExpression(lambdaInfo.Body).WalkDownParentheses();

                        InvocationExpressionSyntax newNode = invocationExpression.ReplaceNode(logicalNot2, logicalNot2.Operand.WithTriviaFrom(logicalNot2));

                        return SyntaxRefactorings.ChangeInvokedMethodName(newNode, (memberAccessExpression.Name.Identifier.ValueText == "All") ? "Any" : "All");
                    }
            }

            return null;
        }
    }
}
