// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UnnecessaryNullCoalescingCodeFixProvider))]
[Shared]
public sealed class UnnecessaryNullCoalescingCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray.Create(DiagnosticIdentifiers.UnnecessaryNullCoalescing); }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(
            root,
            context.Span,
            out SyntaxNode node,
            predicate: f => f.IsKind(SyntaxKind.CoalesceExpression, SyntaxKind.CoalesceAssignmentExpression)))
        {
            return;
        }

        Diagnostic diagnostic = context.Diagnostics[0];
        Document document = context.Document;

        CodeAction codeAction = CodeAction.Create(
            "Remove unnecessary null coalescing",
            ct => RefactorAsync(document, node, ct),
            GetEquivalenceKey(diagnostic));

        context.RegisterCodeFix(codeAction, diagnostic);
    }

    private static Task<Document> RefactorAsync(Document document, SyntaxNode node, CancellationToken cancellationToken)
    {
        if (node is BinaryExpressionSyntax coalesceExpression)
        {
            return SimplifyCoalesceExpressionRefactoring.RefactorAsync(
                document,
                coalesceExpression,
                coalesceExpression.Right,
                cancellationToken);
        }

        var assignment = (AssignmentExpressionSyntax)node;

        if (assignment.Parent is ExpressionStatementSyntax expressionStatement)
            return document.RemoveStatementAsync(expressionStatement, cancellationToken);

        return ReplaceAssignmentWithLeftAsync(document, assignment, cancellationToken);
    }

    private static Task<Document> ReplaceAssignmentWithLeftAsync(
        Document document,
        AssignmentExpressionSyntax assignment,
        CancellationToken cancellationToken)
    {
        IEnumerable<SyntaxTrivia> trivia = assignment.DescendantTrivia(
            TextSpan.FromBounds(assignment.OperatorToken.FullSpan.Start, assignment.Right.FullSpan.End));

        ExpressionSyntax newNode = assignment.Left
            .WithTrailingTrivia(trivia)
            .Parenthesize()
            .WithFormatterAnnotation();

        return document.ReplaceNodeAsync(assignment, newNode, cancellationToken);
    }
}
