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
using Roslynator.CSharp.SyntaxRewriters;

namespace Roslynator.CSharp.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DisposeResourceAsynchronouslyCodeFixProvider))]
[Shared]
public sealed class DisposeResourceAsynchronouslyCodeFixProvider : BaseCodeFixProvider
{
    private const string Title = "Dispose resource asynchronously";

    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray.Create(DiagnosticIdentifiers.DisposeResourceAsynchronously); }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(
            root,
            context.Span,
            out SyntaxNode node,
            predicate: f => f.IsKind(SyntaxKind.LocalDeclarationStatement, SyntaxKind.UsingStatement)))
        {
            return;
        }

        Diagnostic diagnostic = context.Diagnostics[0];
        Document document = context.Document;

        switch (diagnostic.Id)
        {
            case DiagnosticIdentifiers.DisposeResourceAsynchronously:
                {
                    if (node is LocalDeclarationStatementSyntax localDeclaration)
                    {
                        CodeAction codeAction = CodeAction.Create(
                            Title,
                            ct => RefactorAsync(document, localDeclaration, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                    }
                    else if (node is UsingStatementSyntax usingStatement)
                    {
                        CodeAction codeAction = CodeAction.Create(
                            Title,
                            ct => RefactorAsync(document, usingStatement, ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }

                    break;
                }
        }
    }

    private static async Task<Document> RefactorAsync(
        Document document,
        UsingStatementSyntax usingStatement,
        CancellationToken cancellationToken)
    {
        UsingStatementSyntax newUsingStatement = usingStatement
            .WithoutLeadingTrivia()
            .WithAwaitKeyword(SyntaxFactory.Token(SyntaxKind.AwaitKeyword).WithLeadingTrivia(usingStatement.GetLeadingTrivia()));

        SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

        return await RefactorAsync(document, usingStatement, newUsingStatement, semanticModel, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<Document> RefactorAsync(
        Document document,
        LocalDeclarationStatementSyntax localDeclaration,
        CancellationToken cancellationToken)
    {
        LocalDeclarationStatementSyntax newLocalDeclaration = localDeclaration
            .WithoutLeadingTrivia()
            .WithAwaitKeyword(SyntaxFactory.Token(SyntaxKind.AwaitKeyword).WithLeadingTrivia(localDeclaration.GetLeadingTrivia()));

        SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

        return await RefactorAsync(document, localDeclaration, newLocalDeclaration, semanticModel, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<Document> RefactorAsync(
        Document document,
        StatementSyntax statement,
        StatementSyntax newStatement,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        for (SyntaxNode node = statement.Parent; node is not null; node = node.Parent)
        {
            if (node is MethodDeclarationSyntax methodDeclaration)
            {
                if (methodDeclaration.Modifiers.Contains(SyntaxKind.AsyncKeyword))
                    return await document.ReplaceNodeAsync(statement, newStatement, cancellationToken).ConfigureAwait(false);

                MethodDeclarationSyntax newNode = methodDeclaration.ReplaceNode(statement, newStatement);

                IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, cancellationToken);

                UseAsyncAwaitRewriter rewriter = UseAsyncAwaitRewriter.Create(methodSymbol, semanticModel, node.SpanStart);
                var newBody = (BlockSyntax)rewriter.VisitBlock(newNode.Body);

                newNode = newNode
                    .WithBody(newBody)
                    .InsertModifier(SyntaxKind.AsyncKeyword);

                return await document.ReplaceNodeAsync(methodDeclaration, newNode, cancellationToken).ConfigureAwait(false);
            }
            else if (node is LocalFunctionStatementSyntax localFunction)
            {
                if (localFunction.Modifiers.Contains(SyntaxKind.AsyncKeyword))
                    return await document.ReplaceNodeAsync(statement, newStatement, cancellationToken).ConfigureAwait(false);

                LocalFunctionStatementSyntax newNode = localFunction.ReplaceNode(statement, newStatement);

                IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(localFunction, cancellationToken);

                UseAsyncAwaitRewriter rewriter = UseAsyncAwaitRewriter.Create(methodSymbol, semanticModel, node.SpanStart);
                var newBody = (BlockSyntax)rewriter.VisitBlock(newNode.Body);

                newNode = newNode
                    .WithBody(newBody)
                    .InsertModifier(SyntaxKind.AsyncKeyword);

                return await document.ReplaceNodeAsync(localFunction, newNode, cancellationToken).ConfigureAwait(false);
            }
            else if (node is LambdaExpressionSyntax lambdaExpression)
            {
                if (lambdaExpression.Modifiers.Contains(SyntaxKind.AsyncKeyword))
                    return await document.ReplaceNodeAsync(statement, newStatement, cancellationToken).ConfigureAwait(false);

                LambdaExpressionSyntax newNode = lambdaExpression.ReplaceNode(statement, newStatement);

                var methodSymbol = (IMethodSymbol)semanticModel.GetSymbol(lambdaExpression, cancellationToken);

                UseAsyncAwaitRewriter rewriter = UseAsyncAwaitRewriter.Create(methodSymbol, semanticModel, node.SpanStart);
                var newBody = (BlockSyntax)rewriter.VisitBlock((BlockSyntax)newNode.Body);

                newNode = newNode
                    .WithBody(newBody)
                    .InsertModifier(SyntaxKind.AsyncKeyword);

                return await document.ReplaceNodeAsync(lambdaExpression, newNode, cancellationToken).ConfigureAwait(false);
            }
            else if (node is AnonymousMethodExpressionSyntax anonymousMethod)
            {
                if (anonymousMethod.Modifiers.Contains(SyntaxKind.AsyncKeyword))
                    return await document.ReplaceNodeAsync(statement, newStatement, cancellationToken).ConfigureAwait(false);

                AnonymousMethodExpressionSyntax newNode = anonymousMethod.ReplaceNode(statement, newStatement);

                var methodSymbol = (IMethodSymbol)semanticModel.GetSymbol(anonymousMethod, cancellationToken);

                UseAsyncAwaitRewriter rewriter = UseAsyncAwaitRewriter.Create(methodSymbol, semanticModel, node.SpanStart);
                var newBody = (BlockSyntax)rewriter.VisitBlock((BlockSyntax)newNode.Body);

                newNode = newNode
                    .WithBody(newBody)
                    .InsertModifier(SyntaxKind.AsyncKeyword);

                return await document.ReplaceNodeAsync(anonymousMethod, newNode, cancellationToken).ConfigureAwait(false);
            }
        }

        throw new InvalidOperationException();
    }
}
