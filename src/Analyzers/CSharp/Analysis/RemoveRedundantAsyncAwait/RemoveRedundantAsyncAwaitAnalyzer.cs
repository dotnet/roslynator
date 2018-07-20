// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.RemoveRedundantAsyncAwait
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RemoveRedundantAsyncAwaitAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.RemoveRedundantAsyncAwait,
                    DiagnosticDescriptors.RemoveRedundantAsyncAwaitFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeLocalFunctionStatement, SyntaxKind.LocalFunctionStatement);
            context.RegisterSyntaxNodeAction(AnalyzeAnonymousMethodExpression, SyntaxKind.AnonymousMethodExpression);
            context.RegisterSyntaxNodeAction(AnalyzeLambdaExpression, SyntaxKind.SimpleLambdaExpression);
            context.RegisterSyntaxNodeAction(AnalyzeLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression);
        }

        public static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            if (methodDeclaration.SpanContainsDirectives())
                return;

            SyntaxToken asyncKeyword = methodDeclaration.Modifiers.Find(SyntaxKind.AsyncKeyword);

            if (!asyncKeyword.IsKind(SyntaxKind.AsyncKeyword))
                return;

            BlockSyntax body = methodDeclaration.Body;

            if (body != null)
            {
                Analyze(context, methodDeclaration, asyncKeyword, body);
            }
            else
            {
                ArrowExpressionClauseSyntax expressionBody = methodDeclaration.ExpressionBody;

                if (expressionBody != null)
                    AnalyzeExpressionBody(context, methodDeclaration, asyncKeyword, expressionBody);
            }
        }

        public static void AnalyzeLocalFunctionStatement(SyntaxNodeAnalysisContext context)
        {
            var localFunction = (LocalFunctionStatementSyntax)context.Node;

            if (localFunction.SpanContainsDirectives())
                return;

            SyntaxToken asyncKeyword = localFunction.Modifiers.Find(SyntaxKind.AsyncKeyword);

            if (!asyncKeyword.IsKind(SyntaxKind.AsyncKeyword))
                return;

            BlockSyntax body = localFunction.Body;

            if (body != null)
            {
                Analyze(context, localFunction, asyncKeyword, body);
            }
            else
            {
                ArrowExpressionClauseSyntax expressionBody = localFunction.ExpressionBody;

                if (expressionBody != null)
                    AnalyzeExpressionBody(context, localFunction, asyncKeyword, expressionBody);
            }
        }

        public static void AnalyzeAnonymousMethodExpression(SyntaxNodeAnalysisContext context)
        {
            var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;

            if (anonymousMethod.SpanContainsDirectives())
                return;

            SyntaxToken asyncKeyword = anonymousMethod.AsyncKeyword;

            if (!asyncKeyword.IsKind(SyntaxKind.AsyncKeyword))
                return;

            BlockSyntax block = anonymousMethod.Block;

            if (block == null)
                return;

            Analyze(context, anonymousMethod, asyncKeyword, block);
        }

        private static void AnalyzeExpressionBody(
            SyntaxNodeAnalysisContext context,
            SyntaxNode node,
            SyntaxToken asyncKeyword,
            ArrowExpressionClauseSyntax expressionBody)
        {
            ExpressionSyntax expression = expressionBody.Expression;

            if (expression?.Kind() != SyntaxKind.AwaitExpression)
                return;

            var awaitExpression = (AwaitExpressionSyntax)expression;

            if (!VerifyTypes(node, awaitExpression, context.SemanticModel, context.CancellationToken))
                return;

            if (ContainsAwaitExpression(awaitExpression.Expression))
                return;

            ReportDiagnostic(context, asyncKeyword, awaitExpression);
        }

        public static void AnalyzeLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var lambda = (LambdaExpressionSyntax)context.Node;

            if (lambda.SpanContainsDirectives())
                return;

            SyntaxToken asyncKeyword = lambda.AsyncKeyword;

            if (!asyncKeyword.IsKind(SyntaxKind.AsyncKeyword))
                return;

            CSharpSyntaxNode body = lambda.Body;

            if (body == null)
                return;

            SyntaxKind kind = body.Kind();

            if (kind == SyntaxKind.Block)
            {
                Analyze(context, lambda, asyncKeyword, (BlockSyntax)body);
            }
            else if (kind == SyntaxKind.AwaitExpression)
            {
                var awaitExpression = (AwaitExpressionSyntax)body;

                if (!ContainsAwaitExpression(awaitExpression.Expression)
                    && VerifyTypes(lambda, awaitExpression, context.SemanticModel, context.CancellationToken))
                {
                    ReportDiagnostic(context, asyncKeyword, awaitExpression);
                }
            }
        }

        private static bool ContainsAwaitExpression(SyntaxNode node)
        {
            RemoveRedundantAsyncAwaitWalker walker = RemoveRedundantAsyncAwaitWalkerCache.GetInstance();

            walker.StopOnFirstAwaitExpression = true;
            walker.Visit(node);

            bool result = walker.AwaitExpressions.Count == 1;

            RemoveRedundantAsyncAwaitWalkerCache.Free(walker);

            return result;
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, SyntaxNode node, SyntaxToken asyncKeyword, BlockSyntax body)
        {
            SyntaxList<StatementSyntax> statements = body.Statements;

            StatementSyntax statement = statements.LastOrDefault(f => !f.IsKind(SyntaxKind.LocalFunctionStatement));

            if (statement == null)
                return;

            switch (statement.Kind())
            {
                case SyntaxKind.ReturnStatement:
                    {
                        var returnStatement = (ReturnStatementSyntax)statement;

                        AwaitExpressionSyntax awaitExpression = GetAwaitExpression(returnStatement);

                        if (awaitExpression == null)
                            return;

                        RemoveRedundantAsyncAwaitWalker walker = VisitStatements();

                        HashSet<AwaitExpressionSyntax> awaitExpressions = walker.AwaitExpressions;

                        if (awaitExpressions.Count == 1)
                        {
                            if (VerifyTypes(node, awaitExpression, context.SemanticModel, context.CancellationToken))
                                ReportDiagnostic(context, asyncKeyword, awaitExpression);
                        }
                        else if (awaitExpressions.Count > 1)
                        {
                            StatementSyntax prevStatement = statements[statements.IndexOf(returnStatement) - 1];

                            switch (prevStatement.Kind())
                            {
                                case SyntaxKind.IfStatement:
                                    {
                                        if (VerifyIfStatement((IfStatementSyntax)prevStatement, awaitExpressions.Count - 1, endsWithElse: false)
                                            && VerifyTypes(node, awaitExpressions, context.SemanticModel, context.CancellationToken))
                                        {
                                            ReportDiagnostic(context, asyncKeyword, awaitExpressions);
                                        }

                                        break;
                                    }
                                case SyntaxKind.SwitchStatement:
                                    {
                                        if (VerifySwitchStatement((SwitchStatementSyntax)prevStatement, awaitExpressions.Count - 1, containsDefaultSection: false)
                                            && VerifyTypes(node, awaitExpressions, context.SemanticModel, context.CancellationToken))
                                        {
                                            ReportDiagnostic(context, asyncKeyword, awaitExpressions);
                                        }

                                        break;
                                    }
                            }
                        }

                        RemoveRedundantAsyncAwaitWalkerCache.Free(walker);
                        break;
                    }
                case SyntaxKind.IfStatement:
                    {
                        RemoveRedundantAsyncAwaitWalker walker = VisitStatements();

                        HashSet<AwaitExpressionSyntax> awaitExpressions = walker.AwaitExpressions;

                        if (awaitExpressions.Count > 0
                            && VerifyIfStatement((IfStatementSyntax)statement, awaitExpressions.Count, endsWithElse: true)
                            && VerifyTypes(node, awaitExpressions, context.SemanticModel, context.CancellationToken))
                        {
                            ReportDiagnostic(context, asyncKeyword, awaitExpressions);
                        }

                        RemoveRedundantAsyncAwaitWalkerCache.Free(walker);
                        break;
                    }

                case SyntaxKind.SwitchStatement:
                    {
                        RemoveRedundantAsyncAwaitWalker walker = VisitStatements();

                        HashSet<AwaitExpressionSyntax> awaitExpressions = walker.AwaitExpressions;

                        if (awaitExpressions.Count > 0
                            && VerifySwitchStatement((SwitchStatementSyntax)statement, awaitExpressions.Count, containsDefaultSection: true)
                            && VerifyTypes(node, awaitExpressions, context.SemanticModel, context.CancellationToken))
                        {
                            ReportDiagnostic(context, asyncKeyword, awaitExpressions);
                        }

                        RemoveRedundantAsyncAwaitWalkerCache.Free(walker);
                        break;
                    }
            }

            RemoveRedundantAsyncAwaitWalker VisitStatements()
            {
                RemoveRedundantAsyncAwaitWalker walker = RemoveRedundantAsyncAwaitWalkerCache.GetInstance();

                foreach (StatementSyntax s in statements)
                {
                    walker.Visit(s);

                    if (walker.ShouldStop)
                        return walker;

                    if (s == statement)
                        return walker;
                }

                return walker;
            }
        }

        private static bool VerifyIfStatement(
            IfStatementSyntax ifStatement,
            int expectedCount,
            bool endsWithElse)
        {
            int count = 0;
            foreach (IfStatementOrElseClause ifOrElse in ifStatement.AsCascade())
            {
                if (ifOrElse.IsElse
                    && !endsWithElse)
                {
                    return false;
                }

                AwaitExpressionSyntax awaitExpression = GetAwaitExpression(ifOrElse.Statement);

                if (awaitExpression == null)
                    return false;

                count++;
            }

            return expectedCount == count;
        }

        private static bool VerifySwitchStatement(
            SwitchStatementSyntax switchStatement,
            int expectedCount,
            bool containsDefaultSection)
        {
            int count = 0;
            foreach (SwitchSectionSyntax section in switchStatement.Sections)
            {
                if (section.ContainsDefaultLabel()
                    && !containsDefaultSection)
                {
                    return false;
                }

                AwaitExpressionSyntax awaitExpression = GetAwaitExpression(section.Statements.LastOrDefault());

                if (awaitExpression == null)
                    return false;

                count++;
            }

            return expectedCount == count;
        }

        private static AwaitExpressionSyntax GetAwaitExpression(StatementSyntax statement)
        {
            if (statement == null)
                return null;

            SyntaxKind kind = statement.Kind();

            if (kind == SyntaxKind.Block)
            {
                var block = (BlockSyntax)statement;

                if (!(block.Statements.LastOrDefault() is ReturnStatementSyntax returnStatement))
                    return null;

                return GetAwaitExpression(returnStatement);
            }
            else if (kind == SyntaxKind.ReturnStatement)
            {
                return GetAwaitExpression((ReturnStatementSyntax)statement);
            }

            return null;
        }

        private static AwaitExpressionSyntax GetAwaitExpression(ReturnStatementSyntax returnStatement)
        {
            ExpressionSyntax expression = returnStatement.Expression;

            if (expression?.Kind() == SyntaxKind.AwaitExpression)
                return (AwaitExpressionSyntax)expression;

            return null;
        }

        private static bool VerifyTypes(
            SyntaxNode node,
            HashSet<AwaitExpressionSyntax> awaitExpressions,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            IMethodSymbol methodSymbol = GetMethodSymbol(node, semanticModel, cancellationToken);

            if (methodSymbol == null)
                return false;

            ITypeSymbol returnType = methodSymbol.ReturnType;

            if (returnType?.OriginalDefinition.EqualsOrInheritsFrom(MetadataNames.System_Threading_Tasks_Task_T) != true)
                return false;

            ITypeSymbol typeArgument = ((INamedTypeSymbol)returnType).TypeArguments.SingleOrDefault(shouldThrow: false);

            if (typeArgument == null)
                return false;

            foreach (AwaitExpressionSyntax awaitExpression in awaitExpressions)
            {
                if (!VerifyAwaitType(awaitExpression, typeArgument, semanticModel, cancellationToken))
                    return false;
            }

            return true;
        }

        private static bool VerifyTypes(
            SyntaxNode node,
            AwaitExpressionSyntax awaitExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            IMethodSymbol methodSymbol = GetMethodSymbol(node, semanticModel, cancellationToken);

            if (methodSymbol == null)
                return false;

            ITypeSymbol returnType = methodSymbol.ReturnType;

            if (returnType?.OriginalDefinition.EqualsOrInheritsFrom(MetadataNames.System_Threading_Tasks_Task_T) != true)
                return false;

            ITypeSymbol typeArgument = ((INamedTypeSymbol)returnType).TypeArguments.SingleOrDefault(shouldThrow: false);

            if (typeArgument == null)
                return false;

            return VerifyAwaitType(awaitExpression, typeArgument, semanticModel, cancellationToken);
        }

        private static bool VerifyAwaitType(AwaitExpressionSyntax awaitExpression, ITypeSymbol typeArgument, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (!typeArgument.Equals(semanticModel.GetTypeSymbol(awaitExpression, cancellationToken)))
                return false;

            ExpressionSyntax expression = awaitExpression.Expression;

            ITypeSymbol expressionTypeSymbol = semanticModel.GetTypeSymbol(expression, cancellationToken);

            if (expressionTypeSymbol == null)
                return false;

            if (expressionTypeSymbol.OriginalDefinition.EqualsOrInheritsFrom(MetadataNames.System_Threading_Tasks_Task_T))
                return true;

            SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(expression);

            return invocationInfo.Success
                && invocationInfo.Arguments.Count == 1
                && invocationInfo.NameText == "ConfigureAwait"
                && expressionTypeSymbol.OriginalDefinition.HasMetadataName(MetadataNames.System_Runtime_CompilerServices_ConfiguredTaskAwaitable_T);
        }

        private static IMethodSymbol GetMethodSymbol(SyntaxNode node, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            switch (node.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                case SyntaxKind.LocalFunctionStatement:
                    return (IMethodSymbol)semanticModel.GetDeclaredSymbol(node, cancellationToken);
                case SyntaxKind.SimpleLambdaExpression:
                case SyntaxKind.ParenthesizedLambdaExpression:
                case SyntaxKind.AnonymousMethodExpression:
                    return (IMethodSymbol)semanticModel.GetSymbol(node, cancellationToken);
            }

            throw new InvalidOperationException();
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

            ExpressionSyntax expression = awaitExpression.Expression;

            ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(expression, context.CancellationToken);

            if (typeSymbol?.OriginalDefinition.HasMetadataName(MetadataNames.System_Runtime_CompilerServices_ConfiguredTaskAwaitable_T) == true
                && (expression is InvocationExpressionSyntax invocation))
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
}
