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
using Microsoft.CodeAnalysis.Text;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Syntax;
using Roslynator.CSharp.SyntaxRewriters;

namespace Roslynator.CSharp.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LocalDeclarationStatementCodeFixProvider))]
[Shared]
public sealed class LocalDeclarationStatementCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get
        {
            return ImmutableArray.Create(
                DiagnosticIdentifiers.InlineLocalVariable,
                DiagnosticIdentifiers.DisposeResourceAsynchronously);
        }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(root, context.Span, out LocalDeclarationStatementSyntax localDeclaration))
            return;

        Diagnostic diagnostic = context.Diagnostics[0];
        Document document = context.Document;

        switch (diagnostic.Id)
        {
            case DiagnosticIdentifiers.InlineLocalVariable:
                {
                    CodeAction codeAction = CodeAction.Create(
                        "Inline local variable",
                        ct => RefactorAsync(document, localDeclaration, ct),
                        GetEquivalenceKey(diagnostic));

                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
            case DiagnosticIdentifiers.DisposeResourceAsynchronously:
                {
                    CodeAction codeAction = CodeAction.Create(
                        "Dispose resource asynchronously",
                        ct => DisposeResourceAsynchronouslyAsync(document, localDeclaration, ct),
                        GetEquivalenceKey(diagnostic));

                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
        }
    }

    private static async Task<Document> RefactorAsync(
        Document document,
        LocalDeclarationStatementSyntax localDeclaration,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        StatementListInfo statementsInfo = SyntaxInfo.StatementListInfo(localDeclaration);

        int index = statementsInfo.Statements.IndexOf(localDeclaration);

        StatementSyntax nextStatement = statementsInfo.Statements[index + 1];

        SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

        ExpressionSyntax value = GetExpressionToInline(localDeclaration, semanticModel, cancellationToken);

        StatementSyntax newStatement = GetStatementWithInlinedExpression(nextStatement, value);

        SyntaxTriviaList leadingTrivia = localDeclaration.GetLeadingTrivia();

        IEnumerable<SyntaxTrivia> trivia = statementsInfo
            .Parent
            .DescendantTrivia(TextSpan.FromBounds(localDeclaration.Span.End, nextStatement.SpanStart));

        if (!trivia.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
        {
            newStatement = newStatement.WithLeadingTrivia(leadingTrivia.Concat(trivia));
        }
        else
        {
            newStatement = newStatement.WithLeadingTrivia(leadingTrivia);
        }

        SyntaxList<StatementSyntax> newStatements = statementsInfo.Statements
            .Replace(nextStatement, newStatement)
            .RemoveAt(index);

        return await document.ReplaceStatementsAsync(statementsInfo, newStatements, cancellationToken).ConfigureAwait(false);
    }

    private static ExpressionSyntax GetExpressionToInline(LocalDeclarationStatementSyntax localDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        VariableDeclarationSyntax variableDeclaration = localDeclaration.Declaration;

        ExpressionSyntax expression = variableDeclaration
            .Variables[0]
            .Initializer
            .Value;

        if (expression.IsKind(SyntaxKind.ArrayInitializerExpression))
        {
            expression = SyntaxFactory.ArrayCreationExpression(
                (ArrayTypeSyntax)variableDeclaration.Type.WithoutTrivia(),
                (InitializerExpressionSyntax)expression);

            return expression.WithFormatterAnnotation();
        }
        else
        {
            expression = expression.Parenthesize();

            ExpressionSyntax typeExpression = (variableDeclaration.Type.IsVar)
                ? variableDeclaration.Variables[0].Initializer.Value
                : variableDeclaration.Type;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(typeExpression, cancellationToken);

            if (typeSymbol.SupportsExplicitDeclaration())
            {
                TypeSyntax type = typeSymbol.ToMinimalTypeSyntax(semanticModel, localDeclaration.SpanStart);

                expression = SyntaxFactory.CastExpression(type, expression).WithSimplifierAnnotation();
            }

            return expression;
        }
    }

    private static StatementSyntax GetStatementWithInlinedExpression(StatementSyntax statement, ExpressionSyntax expression)
    {
        switch (statement.Kind())
        {
            case SyntaxKind.ExpressionStatement:
                {
                    var expressionStatement = (ExpressionStatementSyntax)statement;

                    var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

                    AssignmentExpressionSyntax newAssignment = assignment.WithRight(expression.WithTriviaFrom(assignment.Right));

                    return expressionStatement.WithExpression(newAssignment);
                }
            case SyntaxKind.LocalDeclarationStatement:
                {
                    var localDeclaration = (LocalDeclarationStatementSyntax)statement;

                    ExpressionSyntax value = localDeclaration
                        .Declaration
                        .Variables[0]
                        .Initializer
                        .Value;

                    return statement.ReplaceNode(value, expression.WithTriviaFrom(value));
                }
            case SyntaxKind.ReturnStatement:
                {
                    var returnStatement = (ReturnStatementSyntax)statement;

                    return returnStatement.WithExpression(expression.WithTriviaFrom(returnStatement.Expression));
                }
            case SyntaxKind.YieldReturnStatement:
                {
                    var yieldStatement = (YieldStatementSyntax)statement;

                    return yieldStatement.WithExpression(expression.WithTriviaFrom(yieldStatement.Expression));
                }
            case SyntaxKind.ForEachStatement:
                {
                    var forEachStatement = (ForEachStatementSyntax)statement;

                    return forEachStatement.WithExpression(expression.WithTriviaFrom(forEachStatement.Expression));
                }
            case SyntaxKind.SwitchStatement:
                {
                    var switchStatement = (SwitchStatementSyntax)statement;

                    return switchStatement.WithExpression(expression.WithTriviaFrom(switchStatement.Expression));
                }
            default:
                {
                    throw new InvalidOperationException();
                }
        }
    }

    private static async Task<Document> DisposeResourceAsynchronouslyAsync(
        Document document,
        LocalDeclarationStatementSyntax localDeclaration,
        CancellationToken cancellationToken)
    {
        SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

        LocalDeclarationStatementSyntax newLocalDeclaration = localDeclaration.WithAwaitKeyword(SyntaxFactory.Token(SyntaxKind.AwaitKeyword));

        for (SyntaxNode node = localDeclaration.Parent; node is not null; node = node.Parent)
        {
            if (node is MethodDeclarationSyntax methodDeclaration)
            {
                if (methodDeclaration.Modifiers.Contains(SyntaxKind.AsyncKeyword))
                {
                    return await document.ReplaceNodeAsync(localDeclaration, newLocalDeclaration, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    MethodDeclarationSyntax newMethodDeclaration = methodDeclaration.ReplaceNode(localDeclaration, newLocalDeclaration);

                    IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, cancellationToken);

                    UseAsyncAwaitRewriter rewriter = UseAsyncAwaitRewriter.Create(methodSymbol);
                    var newBody = (BlockSyntax)rewriter.VisitBlock(newMethodDeclaration.Body);

                    newMethodDeclaration = newMethodDeclaration
                        .WithBody(newBody)
                        .InsertModifier(SyntaxKind.AsyncKeyword);

                    return await document.ReplaceNodeAsync(methodDeclaration, newMethodDeclaration, cancellationToken).ConfigureAwait(false);
                }
            }
            else if (node is LocalFunctionStatementSyntax localFunction)
            {
                if (localFunction.Modifiers.Contains(SyntaxKind.AsyncKeyword))
                {
                    return await document.ReplaceNodeAsync(localDeclaration, newLocalDeclaration, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    LocalFunctionStatementSyntax newLocalFunction = localFunction.ReplaceNode(localDeclaration, newLocalDeclaration);

                    IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(localFunction, cancellationToken);

                    UseAsyncAwaitRewriter rewriter = UseAsyncAwaitRewriter.Create(methodSymbol);
                    var newBody = (BlockSyntax)rewriter.VisitBlock(newLocalFunction.Body);

                    newLocalFunction = newLocalFunction
                        .WithBody(newBody)
                        .InsertModifier(SyntaxKind.AsyncKeyword);

                    return await document.ReplaceNodeAsync(localFunction, newLocalFunction, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        throw new InvalidOperationException();
    }
}
