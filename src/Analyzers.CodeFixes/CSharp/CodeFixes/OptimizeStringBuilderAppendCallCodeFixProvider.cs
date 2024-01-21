// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
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
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(OptimizeStringBuilderAppendCallCodeFixProvider))]
[Shared]
public sealed class OptimizeStringBuilderAppendCallCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray.Create(DiagnosticIdentifiers.OptimizeStringBuilderAppendCall); }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(
            root,
            context.Span,
            out SyntaxNode node,
            getInnermostNodeForTie: false,
            predicate: f => f.IsKind(SyntaxKind.Argument, SyntaxKind.InvocationExpression)))
        {
            return;
        }

        Diagnostic diagnostic = context.Diagnostics[0];
        Document document = context.Document;

        if (node is ArgumentSyntax argument)
        {
            SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo((InvocationExpressionSyntax)argument.Parent.Parent);

            CodeAction codeAction = CodeAction.Create(
                $"Optimize '{invocationInfo.NameText}' call",
                ct => RefactorAsync(document, argument, invocationInfo, ct),
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }
        else if (node is InvocationExpressionSyntax invocationExpression)
        {
            SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocationExpression);

            CodeAction codeAction = CodeAction.Create(
                $"Optimize '{invocationInfo.NameText}' call",
                ct => RefactorAsync(document, invocationInfo, ct),
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }
    }

    public static Task<Document> RefactorAsync(
        Document document,
        in SimpleMemberInvocationExpressionInfo invocationInfo,
        CancellationToken cancellationToken)
    {
        SimpleMemberInvocationExpressionInfo invocationInfo2 = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocationInfo.Expression);

        SyntaxTriviaList trivia = invocationInfo2.InvocationExpression
            .GetTrailingTrivia()
            .EmptyIfWhitespace()
            .AddRange(invocationInfo.InvocationExpression.GetTrailingTrivia());

        InvocationExpressionSyntax newNode = invocationInfo2
            .WithName("AppendLine")
            .InvocationExpression
            .WithTrailingTrivia(trivia);

        return document.ReplaceNodeAsync(invocationInfo.InvocationExpression, newNode, cancellationToken);
    }

    public static async Task<Document> RefactorAsync(
        Document document,
        ArgumentSyntax argument,
        SimpleMemberInvocationExpressionInfo invocationInfo,
        CancellationToken cancellationToken)
    {
        InvocationExpressionSyntax invocation = invocationInfo.InvocationExpression;
        InvocationExpressionSyntax newInvocation;

        bool isAppendLine = string.Equals(invocationInfo.NameText, "AppendLine", StringComparison.Ordinal);

        ExpressionSyntax expression = argument.Expression;

        newInvocation = CreateInvocationExpression(
            (InvocationExpressionSyntax)expression,
            invocation);

        if (isAppendLine)
            newInvocation = SimpleMemberInvocationExpression(newInvocation, IdentifierName("AppendLine"), ArgumentList());

        newInvocation = newInvocation
            .WithTriviaFrom(invocation)
            .WithFormatterAnnotation();

        return await document.ReplaceNodeAsync(invocation, newInvocation, cancellationToken).ConfigureAwait(false);
    }

    private static InvocationExpressionSyntax CreateInvocationExpression(
        InvocationExpressionSyntax innerInvocationExpression,
        InvocationExpressionSyntax outerInvocationExpression)
    {
        SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(innerInvocationExpression);

        switch (invocationInfo.NameText)
        {
            case "Substring":
                {
                    SeparatedSyntaxList<ArgumentSyntax> arguments = invocationInfo.Arguments;

                    switch (arguments.Count)
                    {
                        case 1:
                            {
                                ArgumentListSyntax argumentList = ArgumentList(
                                    Argument(invocationInfo.Expression),
                                    arguments[0],
                                    Argument(
                                        SubtractExpression(
                                            SimpleMemberAccessExpression(invocationInfo.Expression, IdentifierName("Length")),
                                            arguments[0].Expression.Parenthesize())));

                                return CreateNewInvocationExpression(outerInvocationExpression, "Append", argumentList);
                            }
                        case 2:
                            {
                                ArgumentListSyntax argumentList = ArgumentList(
                                    Argument(invocationInfo.Expression),
                                    arguments[0],
                                    arguments[1]
                                );

                                return CreateNewInvocationExpression(outerInvocationExpression, "Append", argumentList);
                            }
                        default:
                            {
                                throw new InvalidOperationException();
                            }
                    }
                }
            case "Remove":
                {
                    SeparatedSyntaxList<ArgumentSyntax> arguments = invocationInfo.Arguments;

                    ArgumentListSyntax argumentList = ArgumentList(
                        Argument(invocationInfo.Expression),
                        Argument(NumericLiteralExpression(0)),
                        arguments[0]
                    );

                    return CreateNewInvocationExpression(outerInvocationExpression, "Append", argumentList);
                }
            case "Format":
                {
                    return CreateNewInvocationExpression(outerInvocationExpression, "AppendFormat", invocationInfo.ArgumentList);
                }
            case "Join":
                {
                    return CreateNewInvocationExpression(outerInvocationExpression, "AppendJoin", invocationInfo.ArgumentList);
                }
        }

        SyntaxDebug.Fail(innerInvocationExpression);
        return outerInvocationExpression;
    }

    private static InvocationExpressionSyntax CreateNewInvocationExpression(InvocationExpressionSyntax invocationExpression, string methodName, ArgumentListSyntax argumentList)
    {
        var memberAccess = (MemberAccessExpressionSyntax)invocationExpression.Expression;

        return invocationExpression
            .WithExpression(memberAccess.WithName(IdentifierName(methodName).WithTriviaFrom(memberAccess.Name)))
            .WithArgumentList(argumentList);
    }
}
