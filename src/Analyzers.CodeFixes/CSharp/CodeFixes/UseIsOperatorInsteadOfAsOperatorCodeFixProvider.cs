// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
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

        SyntaxTriviaList isTrailingTrivia = asExpressionInfo.OperatorToken.TrailingTrivia;
        SyntaxTriviaList typeLeadingTrivia = asExpressionInfo.Type.GetLeadingTrivia();

        if (!isTrailingTrivia.Any(static t => t.IsWhitespaceOrEndOfLineTrivia())
            && !typeLeadingTrivia.Any(static t => t.IsWhitespaceOrEndOfLineTrivia()))
        {
            isTrailingTrivia = isTrailingTrivia.Add(Space);
        }

        SyntaxToken isKeyword = Token(
            asExpressionInfo.OperatorToken.LeadingTrivia,
            SyntaxKind.IsKeyword,
            isTrailingTrivia);

        ExpressionSyntax newNode = IsExpression(
            asExpressionInfo.Expression,
            isKeyword,
            asExpressionInfo.Type.WithLeadingTrivia(typeLeadingTrivia));

        if (nullCheck.IsCheckingNull)
        {
            newNode = LogicalNotExpression(
                ParenthesizedExpression(
                    Token(SyntaxTriviaList.Empty, SyntaxKind.OpenParenToken, SyntaxTriviaList.Empty),
                    newNode,
                    Token(SyntaxTriviaList.Empty, SyntaxKind.CloseParenToken, SyntaxTriviaList.Empty)));
        }

        newNode = newNode.WithFormatterAnnotation();

        return document.ReplaceNodeAsync(node, newNode, cancellationToken);
    }
}
