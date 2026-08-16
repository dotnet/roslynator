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
using Roslynator.CSharp;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CodeAnalysis.CSharp;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseIsKindInsteadOfKindComparisonCodeFixProvider))]
[Shared]
public sealed class UseIsKindInsteadOfKindComparisonCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray.Create(CodeAnalysisDiagnosticIdentifiers.UseIsKindInsteadOfKindComparison); }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(root, context.Span, out ExpressionSyntax expression))
            return;

        Document document = context.Document;
        Diagnostic diagnostic = context.Diagnostics[0];

        CodeAction codeAction = CodeAction.Create(
            "Call 'IsKind' instead of comparing 'Kind'",
            ct => RefactorAsync(document, expression, ct),
            GetEquivalenceKey(diagnostic));

        context.RegisterCodeFix(codeAction, diagnostic);
    }

    private static async Task<Document> RefactorAsync(
        Document document,
        ExpressionSyntax expression,
        CancellationToken cancellationToken)
    {
        SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

        expression = expression.WalkUpParentheses();

        ExpressionSyntax newExpression;

        if (expression.IsKind(SyntaxKind.LogicalOrExpression))
        {
            var logicalOr = (BinaryExpressionSyntax)expression;

            ImmutableArray<IsKindExpressionInfo>.Builder builder = ImmutableArray.CreateBuilder<IsKindExpressionInfo>();

            foreach (ExpressionSyntax operand in SyntaxInfo.BinaryExpressionInfo(logicalOr).AsChain())
            {
                IsKindExpressionInfo info = IsKindExpressionInfo.Create(
                    operand,
                    semanticModel,
                    cancellationToken: cancellationToken);

                if (!info.Success)
                    return document;

                builder.Add(info);
            }

            if (builder.Count == 0)
                return document;

            newExpression = CreateIsKindExpression(builder[0]);

            for (int i = 1; i < builder.Count; i++)
                newExpression = LogicalOrExpression(newExpression, CreateIsKindExpression(builder[i]));
        }
        else
        {
            IsKindExpressionInfo info = IsKindExpressionInfo.Create(
                expression,
                semanticModel,
                cancellationToken: cancellationToken);

            if (!info.Success)
                return document;

            newExpression = CreateIsKindExpression(info);
        }

        newExpression = newExpression
            .WithTriviaFrom(expression)
            .WithFormatterAnnotation();

        return await document.ReplaceNodeAsync(expression, newExpression, cancellationToken).ConfigureAwait(false);
    }

    private static ExpressionSyntax CreateIsKindExpression(IsKindExpressionInfo info)
    {
        InvocationExpressionSyntax invocation = SimpleMemberInvocationExpression(
            info.Expression.WithoutTrivia(),
            IdentifierName("IsKind"),
            Argument(info.KindExpression.WithoutTrivia()));

        return info.Style switch
        {
            IsKindExpressionStyle.NotKind or IsKindExpressionStyle.NotKindConditional => LogicalNotExpression(invocation),
            _ => invocation,
        };
    }
}
