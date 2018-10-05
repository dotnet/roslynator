// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseAsyncAwaitCodeFixProvider))]
    [Shared]
    public class UseAsyncAwaitCodeFixProvider : BaseCodeFixProvider
    {
        private static readonly SyntaxAnnotation[] _asyncAwaitAnnotation = new SyntaxAnnotation[] { new SyntaxAnnotation() };

        private static readonly SyntaxAnnotation[] _asyncAwaitAnnotationAndFormatterAnnotation = new SyntaxAnnotation[] { _asyncAwaitAnnotation[0], Formatter.Annotation };

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.UseAsyncAwait); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out SyntaxNode node, predicate: f => f.IsKind(
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

            switch (node.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)node;

                        IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, cancellationToken);

                        UseAsyncAwaitRewriter rewriter = UseAsyncAwaitRewriter.Create(methodSymbol);

                        var newNode = (MethodDeclarationSyntax)rewriter.VisitMethodDeclaration(methodDeclaration);

                        newNode = ModifierList<MethodDeclarationSyntax>.Instance.Insert(newNode, SyntaxKind.AsyncKeyword);

                        return await document.ReplaceNodeAsync(methodDeclaration, newNode, cancellationToken).ConfigureAwait(false);
                    }
                case SyntaxKind.LocalFunctionStatement:
                    {
                        var localFunction = (LocalFunctionStatementSyntax)node;

                        IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(localFunction, cancellationToken);

                        UseAsyncAwaitRewriter rewriter = UseAsyncAwaitRewriter.Create(methodSymbol);

                        var newBody = (BlockSyntax)rewriter.VisitBlock(localFunction.Body);

                        LocalFunctionStatementSyntax newNode = localFunction.WithBody(newBody);

                        newNode = ModifierList<LocalFunctionStatementSyntax>.Instance.Insert(newNode, SyntaxKind.AsyncKeyword);

                        return await document.ReplaceNodeAsync(localFunction, newNode, cancellationToken).ConfigureAwait(false);
                    }
                case SyntaxKind.SimpleLambdaExpression:
                    {
                        var lambda = (SimpleLambdaExpressionSyntax)node;

                        var methodSymbol = (IMethodSymbol)semanticModel.GetSymbol(lambda, cancellationToken);

                        UseAsyncAwaitRewriter rewriter = UseAsyncAwaitRewriter.Create(methodSymbol);

                        var newBody = (BlockSyntax)rewriter.VisitBlock((BlockSyntax)lambda.Body);

                        SimpleLambdaExpressionSyntax newNode = lambda
                            .WithBody(newBody)
                            .WithAsyncKeyword(Token(SyntaxKind.AsyncKeyword));

                        return await document.ReplaceNodeAsync(lambda, newNode, cancellationToken).ConfigureAwait(false);
                    }
                case SyntaxKind.ParenthesizedLambdaExpression:
                    {
                        var lambda = (ParenthesizedLambdaExpressionSyntax)node;

                        var methodSymbol = (IMethodSymbol)semanticModel.GetSymbol(lambda, cancellationToken);

                        UseAsyncAwaitRewriter rewriter = UseAsyncAwaitRewriter.Create(methodSymbol);

                        var newBody = (BlockSyntax)rewriter.VisitBlock((BlockSyntax)lambda.Body);

                        ParenthesizedLambdaExpressionSyntax newNode = lambda
                            .WithBody(newBody)
                            .WithAsyncKeyword(Token(SyntaxKind.AsyncKeyword));

                        return await document.ReplaceNodeAsync(lambda, newNode, cancellationToken).ConfigureAwait(false);
                    }
                case SyntaxKind.AnonymousMethodExpression:
                    {
                        var anonymousMethod = (AnonymousMethodExpressionSyntax)node;

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

        private class UseAsyncAwaitRewriter : SkipFunctionRewriter
        {
            private UseAsyncAwaitRewriter(bool keepReturnStatement)
            {
                KeepReturnStatement = keepReturnStatement;
            }

            public bool KeepReturnStatement { get; }

            public static UseAsyncAwaitRewriter Create(IMethodSymbol methodSymbol)
            {
                ITypeSymbol returnType = methodSymbol.ReturnType.OriginalDefinition;

                bool keepReturnStatement = false;

                if (returnType.EqualsOrInheritsFrom(MetadataNames.System_Threading_Tasks_ValueTask_T)
                    || returnType.EqualsOrInheritsFrom(MetadataNames.System_Threading_Tasks_Task_T))
                {
                    keepReturnStatement = true;
                }

                return new UseAsyncAwaitRewriter(keepReturnStatement: keepReturnStatement);
            }

            public override SyntaxNode VisitReturnStatement(ReturnStatementSyntax node)
            {
                ExpressionSyntax expression = node.Expression;

                if (expression?.IsKind(SyntaxKind.AwaitExpression) == false)
                {
                    if (KeepReturnStatement)
                    {
                        return node.WithExpression(AwaitExpression(expression.WithoutTrivia().Parenthesize()).WithTriviaFrom(expression));
                    }
                    else
                    {
                        return ExpressionStatement(AwaitExpression(expression.WithoutTrivia().Parenthesize()).WithTriviaFrom(expression))
                                .WithLeadingTrivia(node.GetLeadingTrivia())
                                .WithAdditionalAnnotations(_asyncAwaitAnnotationAndFormatterAnnotation);
                    }
                }

                return base.VisitReturnStatement(node);
            }

            public override SyntaxNode VisitBlock(BlockSyntax node)
            {
                node = (BlockSyntax)base.VisitBlock(node);

                SyntaxList<StatementSyntax> statements = node.Statements;

                statements = RewriteStatements(statements);

                return node.WithStatements(statements);
            }

            public override SyntaxNode VisitSwitchSection(SwitchSectionSyntax node)
            {
                node = (SwitchSectionSyntax)base.VisitSwitchSection(node);

                SyntaxList<StatementSyntax> statements = node.Statements;

                statements = RewriteStatements(statements);

                return node.WithStatements(statements);
            }

            private static SyntaxList<StatementSyntax> RewriteStatements(SyntaxList<StatementSyntax> statements)
            {
                for (int i = statements.Count - 1; i >= 0; i--)
                {
                    StatementSyntax statement = statements[i];

                    if (statement.HasAnnotation(_asyncAwaitAnnotation[0]))
                    {
                        statements = statements.Replace(
                            statement,
                            statement.WithoutAnnotations(_asyncAwaitAnnotation).WithTrailingTrivia(NewLine()));

                        statements = statements.Insert(
                            i + 1,
                            ReturnStatement().WithTrailingTrivia(statement.GetTrailingTrivia()).WithFormatterAnnotation());
                    }
                }

                return statements;
            }
        }
    }
}
