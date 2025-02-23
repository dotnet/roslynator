﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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

namespace Roslynator.CSharp.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddParenthesesWhenNecessaryCodeFixProvider))]
[Shared]
public sealed class AddParenthesesWhenNecessaryCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray.Create(DiagnosticIdentifiers.AddParenthesesWhenNecessary); }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(root, context.Span, out ExpressionSyntax expression, predicate: f => f.IsKind(SyntaxKind.ConditionalExpression, SyntaxKind.ConditionalAccessExpression) || f is BinaryExpressionSyntax))
            return;

        Document document = context.Document;

        CodeAction codeAction = CodeAction.Create(
            $"Parenthesize '{expression}'",
            ct =>
            {
                if (expression.IsKind(SyntaxKind.ConditionalAccessExpression))
                {
                    return document.ReplaceNodeAsync(expression, expression.Parenthesize(simplifiable: false), ct);
                }
                else
                {
                    return AddParenthesesAccordingToOperatorPrecedenceAsync(document, expression, ct);
                }
            },
            GetEquivalenceKey(DiagnosticIdentifiers.AddParenthesesWhenNecessary));

        context.RegisterCodeFix(codeAction, context.Diagnostics);
    }

    private static Task<Document> AddParenthesesAccordingToOperatorPrecedenceAsync(
        Document document,
        ExpressionSyntax expression,
        CancellationToken cancellationToken = default)
    {
        var newNode = (ExpressionSyntax)SyntaxRewriter.Instance.Visit(expression);

        newNode = newNode.Parenthesize(simplifiable: false);

        return document.ReplaceNodeAsync(expression, newNode, cancellationToken);
    }

    private class SyntaxRewriter : CSharpSyntaxRewriter
    {
        private SyntaxRewriter()
        {
        }

        public static SyntaxRewriter Instance { get; } = new();

        public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            ExpressionSyntax left = VisitExpression(node.Left);
            ExpressionSyntax right = VisitExpression(node.Right);

            return node.Update(left, node.OperatorToken, right);
        }

        private ExpressionSyntax VisitExpression(ExpressionSyntax expression)
        {
            bool isFixable = AddParenthesesWhenNecessaryAnalyzer.IsFixable(expression);

            expression = (ExpressionSyntax)base.Visit(expression);

            if (isFixable)
                expression = expression.Parenthesize(simplifiable: false);

            return expression;
        }
    }
}
