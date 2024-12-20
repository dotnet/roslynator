// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
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
using Roslynator.CSharp.Refactorings;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InvocationExpressionCodeFixProvider))]
[Shared]
public sealed class InvocationExpressionCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get
        {
            return ImmutableArray.Create(
                DiagnosticIds.UseCountOrLengthPropertyInsteadOfAnyMethod,
                DiagnosticIds.RemoveRedundantToStringCall,
                DiagnosticIds.RemoveRedundantStringToCharArrayCall,
                DiagnosticIds.CombineEnumerableWhereMethodChain,
                DiagnosticIds.CallExtensionMethodAsInstanceMethod,
                DiagnosticIds.CallThenByInsteadOfOrderBy,
                DiagnosticIds.UseStringInterpolationInsteadOfStringConcat);
        }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(root, context.Span, out InvocationExpressionSyntax invocation))
            return;

        foreach (Diagnostic diagnostic in context.Diagnostics)
        {
            switch (diagnostic.Id)
            {
                case DiagnosticIds.CombineEnumerableWhereMethodChain:
                {
                    CodeAction codeAction = CodeAction.Create(
                        "Combine 'Where' method chain",
                        ct => CombineEnumerableWhereMethodChainRefactoring.RefactorAsync(context.Document, invocation, ct),
                        GetEquivalenceKey(diagnostic));

                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
                case DiagnosticIds.UseCountOrLengthPropertyInsteadOfAnyMethod:
                {
                    string propertyName = diagnostic.Properties["PropertyName"];

                    CodeAction codeAction = CodeAction.Create(
                        $"Use '{propertyName}' property instead of calling 'Any'",
                        ct => UseCountOrLengthPropertyInsteadOfAnyMethodRefactoring.RefactorAsync(context.Document, invocation, propertyName, ct),
                        GetEquivalenceKey(diagnostic));

                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
                case DiagnosticIds.RemoveRedundantToStringCall:
                {
                    CodeAction codeAction = CodeAction.Create(
                        "Remove redundant 'ToString' call",
                        ct => context.Document.ReplaceNodeAsync(invocation, RemoveInvocation(invocation).WithFormatterAnnotation(), ct),
                        GetEquivalenceKey(diagnostic));

                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
                case DiagnosticIds.RemoveRedundantStringToCharArrayCall:
                {
                    CodeAction codeAction = CodeAction.Create(
                        "Remove redundant 'ToCharArray' call",
                        ct => context.Document.ReplaceNodeAsync(invocation, RemoveInvocation(invocation).WithFormatterAnnotation(), ct),
                        GetEquivalenceKey(diagnostic));

                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
                case DiagnosticIds.CallExtensionMethodAsInstanceMethod:
                {
                    CodeAction codeAction = CodeAction.Create(
                        CallExtensionMethodAsInstanceMethodRefactoring.Title,
                        ct => CallExtensionMethodAsInstanceMethodRefactoring.RefactorAsync(context.Document, invocation, ct),
                        GetEquivalenceKey(diagnostic));

                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
                case DiagnosticIds.CallThenByInsteadOfOrderBy:
                {
                    SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocation);

                    string oldName = invocationInfo.NameText;

                    string newName = (string.Equals(oldName, "OrderBy", StringComparison.Ordinal))
                        ? "ThenBy"
                        : "ThenByDescending";

                    CodeAction codeAction = CodeAction.Create(
                        $"Call '{newName}' instead of '{oldName}'",
                        ct => CallThenByInsteadOfOrderByAsync(context.Document, invocation, newName, ct),
                        GetEquivalenceKey(diagnostic));

                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
                case DiagnosticIds.UseStringInterpolationInsteadOfStringConcat:
                {
                    CodeAction codeAction = CodeAction.Create(
                        "Use string interpolation",
                        ct => UseStringInterpolationInsteadOfStringConcatAsync(context.Document, invocation, ct),
                        GetEquivalenceKey(diagnostic));

                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
            }
        }
    }

    private static ExpressionSyntax RemoveInvocation(InvocationExpressionSyntax invocation)
    {
        var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

        ArgumentListSyntax argumentList = invocation.ArgumentList;

        SyntaxToken closeParen = argumentList.CloseParenToken;

        return memberAccess.Expression
            .AppendToTrailingTrivia(
                memberAccess.OperatorToken.GetAllTrivia()
                    .Concat(memberAccess.Name.GetLeadingAndTrailingTrivia())
                    .Concat(argumentList.OpenParenToken.GetAllTrivia())
                    .Concat(closeParen.LeadingTrivia)
                    .ToSyntaxTriviaList()
                    .EmptyIfWhitespace()
                    .AddRange(closeParen.TrailingTrivia));
    }

    private static Task<Document> CallThenByInsteadOfOrderByAsync(
        Document document,
        InvocationExpressionSyntax invocationExpression,
        string newName,
        CancellationToken cancellationToken)
    {
        InvocationExpressionSyntax newInvocationExpression = SyntaxRefactorings.ChangeInvokedMethodName(invocationExpression, newName);

        return document.ReplaceNodeAsync(invocationExpression, newInvocationExpression, cancellationToken);
    }

    private static Task<Document> UseStringInterpolationInsteadOfStringConcatAsync(
        Document document,
        InvocationExpressionSyntax invocationExpression,
        CancellationToken cancellationToken)
    {
        var contents = new List<InterpolatedStringContentSyntax>();
        var isVerbatim = false;

        foreach (ArgumentSyntax argument in invocationExpression.ArgumentList.Arguments)
        {
            ExpressionSyntax expression = argument.Expression;

            if (expression.IsKind(SyntaxKind.StringLiteralExpression))
            {
                var literal = (LiteralExpressionSyntax)expression;

                SyntaxToken token = literal.Token;
                string text = token.Text;

                if (text.StartsWith("@"))
                    isVerbatim = true;

                text = (isVerbatim)
                    ? text.Substring(2, text.Length - 3)
                    : text.Substring(1, text.Length - 2);

                contents.Add(
                    InterpolatedStringText(
                        Token(
                            token.LeadingTrivia,
                            SyntaxKind.InterpolatedStringTextToken,
                            text,
                            token.ValueText,
                            token.TrailingTrivia)));
            }
            else
            {
                contents.Add(Interpolation(expression.Parenthesize()));
            }
        }

        string startTokenText = (isVerbatim) ? "@$\"" : "$\"";

        SyntaxToken startToken = Token(
            SyntaxTriviaList.Empty,
            SyntaxKind.InterpolatedStringStartToken,
            startTokenText,
            startTokenText,
            SyntaxTriviaList.Empty);

        SyntaxToken endToken = Token(
            SyntaxTriviaList.Empty,
            SyntaxKind.InterpolatedStringEndToken,
            "\"",
            "\"",
            SyntaxTriviaList.Empty);

        InterpolatedStringExpressionSyntax interpolatedString = InterpolatedStringExpression(
            startToken,
            contents.ToSyntaxList(),
            endToken)
            .WithTriviaFrom(invocationExpression);

        return document.ReplaceNodeAsync(invocationExpression, interpolatedString, cancellationToken);
    }
}
