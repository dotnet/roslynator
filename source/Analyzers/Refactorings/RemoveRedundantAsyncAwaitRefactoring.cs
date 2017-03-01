// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantAsyncAwaitRefactoring
    {
        public static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            Analyze(context, methodDeclaration, methodDeclaration.Modifiers, methodDeclaration.Body);
        }

        public static void AnalyzeLocalFunctionStatement(SyntaxNodeAnalysisContext context)
        {
            var localFunction = (LocalFunctionStatementSyntax)context.Node;

            Analyze(context, localFunction, localFunction.Modifiers, localFunction.Body);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, SyntaxNode node, SyntaxTokenList modifiers, BlockSyntax body)
        {
            int index = modifiers.IndexOf(SyntaxKind.AsyncKeyword);

            if (index != -1)
                Analyze(context, node, modifiers[index], body);
        }

        public static void AnalyzeLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var lambda = (LambdaExpressionSyntax)context.Node;

            Analyze(context, lambda, lambda.AsyncKeyword, lambda.Body as BlockSyntax);
        }

        public static void AnalyzeAnonymousMethodExpression(SyntaxNodeAnalysisContext context)
        {
            var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;

            Analyze(context, anonymousMethod, anonymousMethod.AsyncKeyword, anonymousMethod.Block);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, SyntaxNode node, SyntaxToken asyncKeyword, BlockSyntax body)
        {
            if (asyncKeyword.IsKind(SyntaxKind.AsyncKeyword)
                && body?.IsMissing == false)
            {
                SyntaxList<StatementSyntax> statements = body.Statements;

                if (statements.Any())
                {
                    StatementSyntax statement = statements.LastOrDefault(f => !f.IsKind(SyntaxKind.LocalFunctionStatement));

                    if (statement?.IsKind(SyntaxKind.ReturnStatement) == true)
                    {
                        var returnStatement = (ReturnStatementSyntax)statement;

                        ExpressionSyntax expression = returnStatement.Expression;

                        if (expression?.IsKind(SyntaxKind.AwaitExpression) == true
                            && !ContainsOtherAwait(body, statements, statement)
                            && !node.SpanContainsDirectives())
                        {
                            var awaitExpression = (AwaitExpressionSyntax)expression;

                            context.ReportDiagnostic(DiagnosticDescriptors.RemoveRedundantAsyncAwait, asyncKeyword);

                            context.ReportToken(DiagnosticDescriptors.RemoveRedundantAsyncAwaitFadeOut, asyncKeyword);
                            context.ReportToken(DiagnosticDescriptors.RemoveRedundantAsyncAwaitFadeOut, awaitExpression.AwaitKeyword);

                            SemanticModel semanticModel = context.SemanticModel;
                            CancellationToken cancellationToken = context.CancellationToken;

                            FadeOutConfigureAwait(context, awaitExpression, semanticModel, cancellationToken);
                        }
                    }
                }
            }
        }

        private static void FadeOutConfigureAwait(SyntaxNodeAnalysisContext context, AwaitExpressionSyntax awaitExpression, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            ExpressionSyntax expression = awaitExpression.Expression;

            var typeSymbol = semanticModel.GetTypeSymbol(expression, cancellationToken) as INamedTypeSymbol;

            if (typeSymbol?.ConstructedFrom.Equals(semanticModel.GetTypeByMetadataName(MetadataNames.System_Runtime_CompilerServices_ConfiguredTaskAwaitable_T)) == true)
            {
                var invocation = expression as InvocationExpressionSyntax;

                if (invocation != null)
                {
                    var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;

                    if (string.Equals(memberAccess?.Name?.Identifier.ValueText, "ConfigureAwait", StringComparison.Ordinal))
                    {
                        context.ReportNode(DiagnosticDescriptors.RemoveRedundantAsyncAwaitFadeOut, memberAccess.Name);
                        context.ReportToken(DiagnosticDescriptors.RemoveRedundantAsyncAwaitFadeOut, memberAccess.OperatorToken);
                        context.ReportNode(DiagnosticDescriptors.RemoveRedundantAsyncAwaitFadeOut, invocation.ArgumentList);
                    }
                }
            }
        }

        private static bool ContainsOtherAwait(BlockSyntax body, SyntaxList<StatementSyntax> statements, StatementSyntax statement)
        {
            int index = statements.IndexOf(statement);

            if (index > 0)
            {
                TextSpan span = TextSpan.FromBounds(body.SpanStart, statements[index - 1].Span.End);

                return body
                    .DescendantNodes(span, f => !f.IsNestedMethod())
                    .Any(f => f.IsKind(SyntaxKind.AwaitExpression));
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            SyntaxNode node,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            switch (node.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    return await RefactorAsync(document, (MethodDeclarationSyntax)node, semanticModel, cancellationToken).ConfigureAwait(false);
                case SyntaxKind.LocalFunctionStatement:
                    return await RefactorAsync(document, (LocalFunctionStatementSyntax)node, semanticModel, cancellationToken).ConfigureAwait(false);
                case SyntaxKind.SimpleLambdaExpression:
                    return await RefactorAsync(document, (SimpleLambdaExpressionSyntax)node, semanticModel, cancellationToken).ConfigureAwait(false);
                case SyntaxKind.ParenthesizedLambdaExpression:
                    return await RefactorAsync(document, (ParenthesizedLambdaExpressionSyntax)node, semanticModel, cancellationToken).ConfigureAwait(false);
                case SyntaxKind.AnonymousMethodExpression:
                    return await RefactorAsync(document, (AnonymousMethodExpressionSyntax)node, semanticModel, cancellationToken).ConfigureAwait(false);
            }

            Debug.Assert(false, node.Kind().ToString());

            return document;
        }

        private static Task<Document> RefactorAsync(Document document, MethodDeclarationSyntax methodDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            BlockSyntax body = methodDeclaration.Body;

            SyntaxList<StatementSyntax> statements = body.Statements;

            var returnStatement = (ReturnStatementSyntax)statements.Last(f => !f.IsKind(SyntaxKind.LocalFunctionStatement));

            ReturnStatementSyntax newReturnStatement = RemoveAwaitAndConfigureAwait(returnStatement, semanticModel, cancellationToken);

            SyntaxList<StatementSyntax> newStatements = statements.Replace(returnStatement, newReturnStatement);

            MethodDeclarationSyntax newNode = methodDeclaration.WithBody(body.WithStatements(newStatements));

            newNode = Remover.RemoveModifier(newNode, SyntaxKind.AsyncKeyword);

            return document.ReplaceNodeAsync(methodDeclaration, newNode, cancellationToken);
        }

        private static Task<Document> RefactorAsync(Document document, LocalFunctionStatementSyntax localFunction, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            BlockSyntax body = localFunction.Body;

            SyntaxList<StatementSyntax> statements = body.Statements;

            var returnStatement = (ReturnStatementSyntax)statements.Last(f => !f.IsKind(SyntaxKind.LocalFunctionStatement));

            ReturnStatementSyntax newReturnStatement = RemoveAwaitAndConfigureAwait(returnStatement, semanticModel, cancellationToken);

            SyntaxList<StatementSyntax> newStatements = statements.Replace(returnStatement, newReturnStatement);

            LocalFunctionStatementSyntax newNode = localFunction.WithBody(body.WithStatements(newStatements));

            newNode = Remover.RemoveModifier(newNode, SyntaxKind.AsyncKeyword);

            return document.ReplaceNodeAsync(localFunction, newNode, cancellationToken);
        }

        private static Task<Document> RefactorAsync(Document document, SimpleLambdaExpressionSyntax lambda, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var body = (BlockSyntax)lambda.Body;

            SyntaxList<StatementSyntax> statements = body.Statements;

            var returnStatement = (ReturnStatementSyntax)statements.Last(f => !f.IsKind(SyntaxKind.LocalFunctionStatement));

            ReturnStatementSyntax newReturnStatement = RemoveAwaitAndConfigureAwait(returnStatement, semanticModel, cancellationToken);

            SyntaxList<StatementSyntax> newStatements = statements.Replace(returnStatement, newReturnStatement);

            SimpleLambdaExpressionSyntax newNode = lambda.WithBody(body.WithStatements(newStatements));

            newNode = newNode.WithAsyncKeyword(GetMissingAsyncKeyword(lambda.AsyncKeyword));

            return document.ReplaceNodeAsync(lambda, newNode, cancellationToken);
        }

        private static Task<Document> RefactorAsync(Document document, ParenthesizedLambdaExpressionSyntax lambda, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var body = (BlockSyntax)lambda.Body;

            SyntaxList<StatementSyntax> statements = body.Statements;

            var returnStatement = (ReturnStatementSyntax)statements.Last(f => !f.IsKind(SyntaxKind.LocalFunctionStatement));

            ReturnStatementSyntax newReturnStatement = RemoveAwaitAndConfigureAwait(returnStatement, semanticModel, cancellationToken);

            SyntaxList<StatementSyntax> newStatements = statements.Replace(returnStatement, newReturnStatement);

            ParenthesizedLambdaExpressionSyntax newNode = lambda.WithBody(body.WithStatements(newStatements));

            newNode = newNode.WithAsyncKeyword(GetMissingAsyncKeyword(lambda.AsyncKeyword));

            return document.ReplaceNodeAsync(lambda, newNode, cancellationToken);
        }

        private static Task<Document> RefactorAsync(Document document, AnonymousMethodExpressionSyntax anonymousMethod, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            BlockSyntax body = anonymousMethod.Block;

            SyntaxList<StatementSyntax> statements = body.Statements;

            var returnStatement = (ReturnStatementSyntax)statements.Last(f => !f.IsKind(SyntaxKind.LocalFunctionStatement));

            ReturnStatementSyntax newReturnStatement = RemoveAwaitAndConfigureAwait(returnStatement, semanticModel, cancellationToken);

            SyntaxList<StatementSyntax> newStatements = statements.Replace(returnStatement, newReturnStatement);

            AnonymousMethodExpressionSyntax newNode = anonymousMethod.WithBody(body.WithStatements(newStatements));

            newNode = newNode.WithAsyncKeyword(GetMissingAsyncKeyword(anonymousMethod.AsyncKeyword));

            return document.ReplaceNodeAsync(anonymousMethod, newNode, cancellationToken);
        }

        private static SyntaxToken GetMissingAsyncKeyword(SyntaxToken asyncKeyword)
        {
            if (asyncKeyword.TrailingTrivia.All(f => f.IsWhitespaceTrivia()))
            {
                return MissingToken(SyntaxKind.AsyncKeyword).WithLeadingTrivia(asyncKeyword.LeadingTrivia);
            }
            else
            {
                return MissingToken(SyntaxKind.AsyncKeyword).WithTriviaFrom(asyncKeyword);
            }
        }

        private static ReturnStatementSyntax RemoveAwaitAndConfigureAwait(
            ReturnStatementSyntax returnStatement,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            var awaitExpression = (AwaitExpressionSyntax)returnStatement.Expression;

            ExpressionSyntax expression = awaitExpression.Expression;

            var typeSymbol = semanticModel.GetTypeSymbol(expression, cancellationToken) as INamedTypeSymbol;

            if (typeSymbol?.ConstructedFrom.Equals(semanticModel.GetTypeByMetadataName(MetadataNames.System_Runtime_CompilerServices_ConfiguredTaskAwaitable_T)) == true)
            {
                var invocation = expression as InvocationExpressionSyntax;

                if (invocation != null)
                {
                    var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;

                    if (string.Equals(memberAccess?.Name?.Identifier.ValueText, "ConfigureAwait", StringComparison.Ordinal))
                        expression = memberAccess.Expression;
                }
            }

            expression = expression.WithTriviaFrom(awaitExpression);

            return returnStatement.WithExpression(expression);
        }
    }
}
