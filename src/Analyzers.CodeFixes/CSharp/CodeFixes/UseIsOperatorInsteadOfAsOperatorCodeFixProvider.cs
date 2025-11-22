// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
using Roslynator.CSharp.Refactorings;
using Roslynator.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseIsOperatorInsteadOfAsOperatorCodeFixProvider))]
[Shared]
public sealed class UseIsOperatorInsteadOfAsOperatorCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray.Create(DiagnosticIdentifiers.UseIsOperatorInsteadOfAsOperator); }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(
            root,
            context.Span,
            out SyntaxNode node,
            predicate: f => f.IsKind(SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression, SyntaxKind.IsPatternExpression)))
        {
            return;
        }

        Diagnostic diagnostic = context.Diagnostics[0];

        CodeAction codeAction = CodeAction.Create(
            "Use 'is' operator",
            ct => RefactorAsync(context.Document, node, ct),
            GetEquivalenceKey(diagnostic));

        context.RegisterCodeFix(codeAction, diagnostic);
    }

    private static Task<Document> RefactorAsync(
        Document document,
        SyntaxNode node,
        CancellationToken cancellationToken)
    {
        NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(node);

        AsExpressionInfo asExpressionInfo = SyntaxInfo.AsExpressionInfo(nullCheck.Expression);

        ExpressionSyntax newNode = IsExpression(
            asExpressionInfo.Expression,
            SyntaxFactory.Token(asExpressionInfo.OperatorToken.LeadingTrivia, SyntaxKind.IsKeyword, asExpressionInfo.OperatorToken.TrailingTrivia),
            asExpressionInfo.Type);

        if (nullCheck.IsCheckingNull)
            newNode = LogicalNotExpression(newNode.WithoutTrivia().Parenthesize()).WithTriviaFrom(newNode);

        newNode = newNode
            .Parenthesize()
            .WithFormatterAnnotation();

        return document.ReplaceNodeAsync(node, newNode, cancellationToken);
    }
}
