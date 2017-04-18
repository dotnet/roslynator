// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static partial class RemoveRedundantAsyncAwaitRefactoring
    {
        public static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            BlockSyntax body = methodDeclaration.Body;

            if (body != null)
            {
                Analyze(context, methodDeclaration, methodDeclaration.Modifiers, body);
            }
            else
            {
                ArrowExpressionClauseSyntax expressionBody = methodDeclaration.ExpressionBody;

                if (expressionBody != null)
                    Analyze(context, methodDeclaration, methodDeclaration.Modifiers, expressionBody);
            }
        }

        public static void AnalyzeLocalFunctionStatement(SyntaxNodeAnalysisContext context)
        {
            var localFunction = (LocalFunctionStatementSyntax)context.Node;

            BlockSyntax body = localFunction.Body;

            if (body != null)
            {
                Analyze(context, localFunction, localFunction.Modifiers, body);
            }
            else
            {
                ArrowExpressionClauseSyntax expressionBody = localFunction.ExpressionBody;

                if (expressionBody != null)
                    Analyze(context, localFunction, localFunction.Modifiers, expressionBody);
            }
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, SyntaxNode node, SyntaxTokenList modifiers, BlockSyntax body)
        {
            int index = modifiers.IndexOf(SyntaxKind.AsyncKeyword);

            if (index != -1)
                Analyze(context, node, modifiers[index], body);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, SyntaxNode node, SyntaxTokenList modifiers, ArrowExpressionClauseSyntax expressionBody)
        {
            int index = modifiers.IndexOf(SyntaxKind.AsyncKeyword);

            if (index != -1)
                Analyze(context, node, modifiers[index], expressionBody);
        }

        public static void AnalyzeLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var lambda = (LambdaExpressionSyntax)context.Node;

            CSharpSyntaxNode body = lambda.Body;

            if (body != null)
            {
                SyntaxKind kind = body.Kind();

                if (kind == SyntaxKind.Block)
                {
                    Analyze(context, lambda, lambda.AsyncKeyword, (BlockSyntax)body);
                }
                else if (kind == SyntaxKind.AwaitExpression)
                {
                    var awaitExpression = (AwaitExpressionSyntax)body;

                    SyntaxToken asyncKeyword = lambda.AsyncKeyword;

                    if (asyncKeyword.IsKind(SyntaxKind.AsyncKeyword)
                        && !lambda.SpanContainsDirectives())
                    {
                        ReportDiagnostic(context, asyncKeyword, awaitExpression);
                    }
                }
            }
        }

        public static void AnalyzeAnonymousMethodExpression(SyntaxNodeAnalysisContext context)
        {
            var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;

            Analyze(context, anonymousMethod, anonymousMethod.AsyncKeyword, anonymousMethod.Block);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, SyntaxNode node, SyntaxToken asyncKeyword, BlockSyntax body)
        {
            if (asyncKeyword.IsKind(SyntaxKind.AsyncKeyword)
                && body?.IsMissing == false
                && !node.SpanContainsDirectives())
            {
                SyntaxList<StatementSyntax> statements = body.Statements;

                if (statements.Any())
                {
                    StatementSyntax statement = statements.LastOrDefault(f => !f.IsKind(SyntaxKind.LocalFunctionStatement));

                    if (statement != null)
                    {
                        SyntaxKind kind = statement.Kind();

                        if (kind == SyntaxKind.ReturnStatement)
                        {
                            var returnStatement = (ReturnStatementSyntax)statement;

                            ExpressionSyntax expression = returnStatement.Expression;

                            if (expression?.IsKind(SyntaxKind.AwaitExpression) == true)
                            {
                                var awaitExpression = (AwaitExpressionSyntax)expression;

                                HashSet<AwaitExpressionSyntax> awaitExpressions = CollectAwaitExpressions(body, TextSpan.FromBounds(body.SpanStart, returnStatement.SpanStart));

                                if (awaitExpressions == null)
                                {
                                    ReportDiagnostic(context, asyncKeyword, awaitExpression);
                                }
                                else
                                {
                                    int index = statements.IndexOf(returnStatement);

                                    if (index > 0)
                                    {
                                        StatementSyntax previousStatement = statements[index - 1];

                                        SyntaxKind previousStatementKind = previousStatement.Kind();

                                        if (previousStatementKind == SyntaxKind.IfStatement)
                                        {
                                            Analyze(context, asyncKeyword, awaitExpression, awaitExpressions, GetAwaitExpressionsFromIfStatement((IfStatementSyntax)previousStatement, endsWithElse: false));
                                        }
                                        else if (previousStatementKind == SyntaxKind.SwitchStatement)
                                        {
                                            Analyze(context, asyncKeyword, awaitExpression, awaitExpressions, GetAwaitExpressionsFromSwitchStatement((SwitchStatementSyntax)previousStatement, containsDefaultSection: false));
                                        }
                                    }
                                }
                            }
                        }
                        else if (kind == SyntaxKind.IfStatement)
                        {
                            Analyze(context, body, asyncKeyword, GetAwaitExpressionsFromIfStatement((IfStatementSyntax)statement, endsWithElse: true));
                        }
                        else if (kind == SyntaxKind.SwitchStatement)
                        {
                            Analyze(context, body, asyncKeyword, GetAwaitExpressionsFromSwitchStatement((SwitchStatementSyntax)statement, containsDefaultSection: true));
                        }
                    }
                }
            }
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            SyntaxToken asyncKeyword,
            AwaitExpressionSyntax awaitExpression,
            HashSet<AwaitExpressionSyntax> awaitExpressions,
            HashSet<AwaitExpressionSyntax> awaitExpressions2)
        {
            if (awaitExpressions2 != null)
            {
                awaitExpressions.ExceptWith(awaitExpressions2);

                if (awaitExpressions.Count == 0)
                {
                    awaitExpressions2.Add(awaitExpression);

                    ReportDiagnostic(context, asyncKeyword, awaitExpressions2);
                }
            }
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            BlockSyntax body,
            SyntaxToken asyncKeyword,
            HashSet<AwaitExpressionSyntax> awaitExpressions)
        {
            if (awaitExpressions != null
                && !body
                    .DescendantNodes(body.Span, f => !f.IsNestedMethod())
                    .Any(f => f.IsKind(SyntaxKind.AwaitExpression) && !awaitExpressions.Contains((AwaitExpressionSyntax)f)))
            {
                ReportDiagnostic(context, asyncKeyword, awaitExpressions);
            }
        }

        private static HashSet<AwaitExpressionSyntax> GetAwaitExpressionsFromIfStatement(
            IfStatementSyntax ifStatement,
            bool endsWithElse)
        {
            HashSet<AwaitExpressionSyntax> awaitExpressions = null;

            foreach (IfStatementOrElseClause ifOrElse in ifStatement.GetChain())
            {
                if (ifOrElse.IsElse
                    && !endsWithElse)
                {
                    return null;
                }

                AwaitExpressionSyntax awaitExpression = GetLastReturnAwaitExpressionOfDefault(ifOrElse.Statement);

                if (awaitExpression != null)
                {
                    (awaitExpressions ?? (awaitExpressions = new HashSet<AwaitExpressionSyntax>())).Add(awaitExpression);
                }
                else
                {
                    return null;
                }
            }

            return awaitExpressions;
        }

        private static HashSet<AwaitExpressionSyntax> GetAwaitExpressionsFromSwitchStatement(
            SwitchStatementSyntax switchStatement,
            bool containsDefaultSection)
        {
            HashSet<AwaitExpressionSyntax> awaitExpressions = null;

            foreach (SwitchSectionSyntax section in switchStatement.Sections)
            {
                if (section.ContainsDefaultLabel())
                {
                    if (containsDefaultSection)
                    {
                        AwaitExpressionSyntax awaitExpression = GetLastReturnAwaitExpressionOfDefault(section.Statements.LastOrDefault());

                        if (awaitExpression != null)
                        {
                            (awaitExpressions ?? (awaitExpressions = new HashSet<AwaitExpressionSyntax>())).Add(awaitExpression);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    AwaitExpressionSyntax awaitExpression = GetLastReturnAwaitExpressionOfDefault(section.Statements.LastOrDefault());

                    if (awaitExpression != null)
                    {
                        (awaitExpressions ?? (awaitExpressions = new HashSet<AwaitExpressionSyntax>())).Add(awaitExpression);
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return awaitExpressions;
        }

        private static AwaitExpressionSyntax GetLastReturnAwaitExpressionOfDefault(StatementSyntax statement)
        {
            if (statement != null)
            {
                SyntaxKind kind = statement.Kind();

                if (kind == SyntaxKind.Block)
                {
                    var block = (BlockSyntax)statement;

                    SyntaxList<StatementSyntax> statements = block.Statements;

                    if (statements.Any())
                    {
                        StatementSyntax last = statements.Last();

                        if (last.IsKind(SyntaxKind.ReturnStatement))
                            return ((ReturnStatementSyntax)last).Expression as AwaitExpressionSyntax;
                    }
                }
                else if (kind == SyntaxKind.ReturnStatement)
                {
                    return ((ReturnStatementSyntax)statement).Expression as AwaitExpressionSyntax;
                }
            }

            return null;
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            SyntaxNode node,
            SyntaxToken asyncKeyword,
            ArrowExpressionClauseSyntax expressionBody)
        {
            if (asyncKeyword.IsKind(SyntaxKind.AsyncKeyword)
                && expressionBody?.IsMissing == false)
            {
                ExpressionSyntax expression = expressionBody.Expression;

                if (expression?.IsKind(SyntaxKind.AwaitExpression) == true
                    && !node.SpanContainsDirectives())
                {
                    ReportDiagnostic(context, asyncKeyword, (AwaitExpressionSyntax)expression);
                }
            }
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxToken asyncKeyword, AwaitExpressionSyntax awaitExpression)
        {
            context.ReportDiagnostic(DiagnosticDescriptors.RemoveRedundantAsyncAwait, asyncKeyword);

            context.ReportToken(DiagnosticDescriptors.RemoveRedundantAsyncAwaitFadeOut, asyncKeyword);

            ReportAwaitAndConfigureAwait(context, awaitExpression);
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxToken asyncKeyword, IEnumerable<AwaitExpressionSyntax> awaitExpressions)
        {
            context.ReportDiagnostic(DiagnosticDescriptors.RemoveRedundantAsyncAwait, asyncKeyword);

            context.ReportToken(DiagnosticDescriptors.RemoveRedundantAsyncAwaitFadeOut, asyncKeyword);

            foreach (AwaitExpressionSyntax awaitExpression in awaitExpressions)
                ReportAwaitAndConfigureAwait(context, awaitExpression);
        }

        private static void ReportAwaitAndConfigureAwait(SyntaxNodeAnalysisContext context, AwaitExpressionSyntax awaitExpression)
        {
            context.ReportToken(DiagnosticDescriptors.RemoveRedundantAsyncAwaitFadeOut, awaitExpression.AwaitKeyword);

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

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

        private static HashSet<AwaitExpressionSyntax> CollectAwaitExpressions(BlockSyntax body, TextSpan span)
        {
            HashSet<AwaitExpressionSyntax> awaitExpressions = null;

            foreach (SyntaxNode node in body.DescendantNodes(span, f => !f.IsNestedMethod()))
            {
                if (node.IsKind(SyntaxKind.AwaitExpression))
                    (awaitExpressions ?? (awaitExpressions = new HashSet<AwaitExpressionSyntax>())).Add((AwaitExpressionSyntax)node);
            }

            return awaitExpressions;
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
            MethodDeclarationSyntax newNode = AwaitRemover.Visit(methodDeclaration, semanticModel, cancellationToken);

            newNode = newNode.RemoveModifier(SyntaxKind.AsyncKeyword);

            return document.ReplaceNodeAsync(methodDeclaration, newNode, cancellationToken);
        }

        private static Task<Document> RefactorAsync(Document document, LocalFunctionStatementSyntax localFunction, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            LocalFunctionStatementSyntax newNode = AwaitRemover.Visit(localFunction, semanticModel, cancellationToken);

            newNode = newNode.RemoveModifier(SyntaxKind.AsyncKeyword);

            return document.ReplaceNodeAsync(localFunction, newNode, cancellationToken);
        }

        private static Task<Document> RefactorAsync(Document document, SimpleLambdaExpressionSyntax lambda, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            SimpleLambdaExpressionSyntax newNode = AwaitRemover.Visit(lambda, semanticModel, cancellationToken);

            newNode = newNode.WithAsyncKeyword(GetMissingAsyncKeyword(lambda.AsyncKeyword));

            return document.ReplaceNodeAsync(lambda, newNode, cancellationToken);
        }

        private static Task<Document> RefactorAsync(Document document, ParenthesizedLambdaExpressionSyntax lambda, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            ParenthesizedLambdaExpressionSyntax newNode = AwaitRemover.Visit(lambda, semanticModel, cancellationToken);

            newNode = newNode.WithAsyncKeyword(GetMissingAsyncKeyword(lambda.AsyncKeyword));

            return document.ReplaceNodeAsync(lambda, newNode, cancellationToken);
        }

        private static Task<Document> RefactorAsync(Document document, AnonymousMethodExpressionSyntax anonymousMethod, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            AnonymousMethodExpressionSyntax newNode = AwaitRemover.Visit(anonymousMethod, semanticModel, cancellationToken);

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
    }
}
