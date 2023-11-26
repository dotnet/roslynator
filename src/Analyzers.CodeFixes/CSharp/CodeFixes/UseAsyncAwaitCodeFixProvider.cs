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
using Microsoft.CodeAnalysis.Formatting;
using Roslynator.CodeFixes;
using Roslynator.CSharp.SyntaxRewriters;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseAsyncAwaitCodeFixProvider))]
[Shared]
public sealed class UseAsyncAwaitCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray.Create(DiagnosticIdentifiers.UseAsyncAwait); }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(
            root,
            context.Span,
            out SyntaxNode node,
            predicate: f => f.IsKind(
                SyntaxKind.MethodDeclaration,
                SyntaxKind.LocalFunctionStatement,
                SyntaxKind.SimpleLambdaExpression,
                SyntaxKind.ParenthesizedLambdaExpression,
                SyntaxKind.AnonymousMethodExpression)))
        {
            return;
        }

        Diagnostic diagnostic = context.Diagnostics[0];

        CodeAction codeAction = CodeAction.Create(
            "Use async/await",
            ct => RefactorAsync(context.Document, node, ct),
            GetEquivalenceKey(diagnostic.Id));

        context.RegisterCodeFix(codeAction, diagnostic);
    }

    private static async Task<Document> RefactorAsync(
        Document document,
        SyntaxNode node,
        CancellationToken cancellationToken)
    {
        SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

        switch (node)
        {
            case MethodDeclarationSyntax methodDeclaration:
                {
                    IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, cancellationToken);

                    UseAsyncAwaitRewriter rewriter = UseAsyncAwaitRewriter.Create(methodSymbol);

                    var newNode = (MethodDeclarationSyntax)rewriter.VisitMethodDeclaration(methodDeclaration);

                    newNode = ModifierList<MethodDeclarationSyntax>.Instance.Insert(newNode, SyntaxKind.AsyncKeyword);

                    return await document.ReplaceNodeAsync(methodDeclaration, newNode, cancellationToken).ConfigureAwait(false);
                }
            case LocalFunctionStatementSyntax localFunction:
                {
                    IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(localFunction, cancellationToken);

                    UseAsyncAwaitRewriter rewriter = UseAsyncAwaitRewriter.Create(methodSymbol);

                    var newBody = (BlockSyntax)rewriter.VisitBlock(localFunction.Body);

                    LocalFunctionStatementSyntax newNode = localFunction.WithBody(newBody);

                    newNode = ModifierList<LocalFunctionStatementSyntax>.Instance.Insert(newNode, SyntaxKind.AsyncKeyword);

                    return await document.ReplaceNodeAsync(localFunction, newNode, cancellationToken).ConfigureAwait(false);
                }
            case SimpleLambdaExpressionSyntax lambda:
                {
                    var methodSymbol = (IMethodSymbol)semanticModel.GetSymbol(lambda, cancellationToken);

                    UseAsyncAwaitRewriter rewriter = UseAsyncAwaitRewriter.Create(methodSymbol);

                    var newBody = (BlockSyntax)rewriter.VisitBlock((BlockSyntax)lambda.Body);

                    SimpleLambdaExpressionSyntax newNode = lambda
                        .WithBody(newBody)
                        .WithAsyncKeyword(Token(SyntaxKind.AsyncKeyword));

                    return await document.ReplaceNodeAsync(lambda, newNode, cancellationToken).ConfigureAwait(false);
                }
            case ParenthesizedLambdaExpressionSyntax lambda:
                {
                    var methodSymbol = (IMethodSymbol)semanticModel.GetSymbol(lambda, cancellationToken);

                    UseAsyncAwaitRewriter rewriter = UseAsyncAwaitRewriter.Create(methodSymbol);

                    var newBody = (BlockSyntax)rewriter.VisitBlock((BlockSyntax)lambda.Body);

                    ParenthesizedLambdaExpressionSyntax newNode = lambda
                        .WithBody(newBody)
                        .WithAsyncKeyword(Token(SyntaxKind.AsyncKeyword));

                    return await document.ReplaceNodeAsync(lambda, newNode, cancellationToken).ConfigureAwait(false);
                }
            case AnonymousMethodExpressionSyntax anonymousMethod:
                {
                    var methodSymbol = (IMethodSymbol)semanticModel.GetSymbol(anonymousMethod, cancellationToken);

                    UseAsyncAwaitRewriter rewriter = UseAsyncAwaitRewriter.Create(methodSymbol);

                    var newBody = (BlockSyntax)rewriter.VisitBlock((BlockSyntax)anonymousMethod.Body);

                    AnonymousMethodExpressionSyntax newNode = anonymousMethod
                        .WithBody(newBody)
                        .WithAsyncKeyword(Token(SyntaxKind.AsyncKeyword));

                    return await document.ReplaceNodeAsync(anonymousMethod, newNode, cancellationToken).ConfigureAwait(false);
                }
        }

        throw new InvalidOperationException();
    }
}
