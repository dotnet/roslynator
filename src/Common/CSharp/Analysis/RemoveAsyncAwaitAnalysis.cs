// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator;
using Roslynator.CSharp;
using Roslynator.CSharp.Syntax;
using Roslynator.CSharp.SyntaxWalkers;

namespace Roslynator.CSharp.Analysis
{
    internal struct RemoveAsyncAwaitAnalysis : IDisposable
    {
        private RemoveAsyncAwaitAnalysis(AwaitExpressionWalker walker)
        {
            Walker = walker;
            AwaitExpression = null;
        }

        private RemoveAsyncAwaitAnalysis(AwaitExpressionSyntax awaitExpression)
        {
            AwaitExpression = awaitExpression;
            Walker = null;
        }

        public bool Success => AwaitExpression != null || Walker?.AwaitExpressions.Count > 0;

        public AwaitExpressionSyntax AwaitExpression { get; }

        public AwaitExpressionWalker Walker { get; }

        public static RemoveAsyncAwaitAnalysis Create(
            MethodDeclarationSyntax methodDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default)
        {
            BlockSyntax body = methodDeclaration.Body;

            if (body != null)
            {
                return AnalyzeMethodBody(methodDeclaration, body, semanticModel, cancellationToken);
            }
            else
            {
                ArrowExpressionClauseSyntax expressionBody = methodDeclaration.ExpressionBody;

                if (expressionBody != null)
                    return AnalyzeExpressionBody(methodDeclaration, expressionBody, semanticModel, cancellationToken);
            }

            return default;
        }

        public static RemoveAsyncAwaitAnalysis Create(
            LocalFunctionStatementSyntax localFunction,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default)
        {
            BlockSyntax body = localFunction.Body;

            if (body != null)
            {
                return AnalyzeMethodBody(localFunction, body, semanticModel, cancellationToken);
            }
            else
            {
                ArrowExpressionClauseSyntax expressionBody = localFunction.ExpressionBody;

                if (expressionBody != null)
                    return AnalyzeExpressionBody(localFunction, expressionBody, semanticModel, cancellationToken);
            }

            return default;
        }

        public static RemoveAsyncAwaitAnalysis Create(
            AnonymousMethodExpressionSyntax anonymousMethod,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default)
        {
            BlockSyntax block = anonymousMethod.Block;

            if (block == null)
                return default;

            return AnalyzeMethodBody(anonymousMethod, block, semanticModel, cancellationToken);
        }

        private static RemoveAsyncAwaitAnalysis AnalyzeExpressionBody(
            SyntaxNode node,
            ArrowExpressionClauseSyntax expressionBody,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax expression = expressionBody.Expression;

            if (expression?.Kind() != SyntaxKind.AwaitExpression)
                return default;

            var awaitExpression = (AwaitExpressionSyntax)expression;

            if (!VerifyTypes(node, awaitExpression, semanticModel, cancellationToken))
                return default;

            if (AwaitExpressionWalker.ContainsAwaitExpression(awaitExpression.Expression))
                return default;

            return new RemoveAsyncAwaitAnalysis(awaitExpression);
        }

        public static RemoveAsyncAwaitAnalysis Create(
            LambdaExpressionSyntax lambda,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default)
        {
            CSharpSyntaxNode body = lambda.Body;

            if (body == null)
                return default;

            switch (body.Kind())
            {
                case SyntaxKind.Block:
                    {
                        return AnalyzeMethodBody(lambda, (BlockSyntax)body, semanticModel, cancellationToken);
                    }
                case SyntaxKind.AwaitExpression:
                    {
                        var awaitExpression = (AwaitExpressionSyntax)body;

                        if (!AwaitExpressionWalker.ContainsAwaitExpression(awaitExpression.Expression)
                            && VerifyTypes(lambda, awaitExpression, semanticModel, cancellationToken))
                        {
                            return new RemoveAsyncAwaitAnalysis(awaitExpression);
                        }

                        break;
                    }
            }

            return default;
        }

        private static RemoveAsyncAwaitAnalysis AnalyzeMethodBody(
            SyntaxNode node,
            BlockSyntax body,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            SyntaxList<StatementSyntax> statements = body.Statements;

            StatementSyntax statement = statements.LastOrDefault(f => !f.IsKind(SyntaxKind.LocalFunctionStatement));

            if (statement == null)
                return default;

            switch (statement.Kind())
            {
                case SyntaxKind.ReturnStatement:
                    {
                        var returnStatement = (ReturnStatementSyntax)statement;

                        AwaitExpressionSyntax awaitExpression = GetAwaitExpression(returnStatement);

                        if (awaitExpression == null)
                            return default;

                        AwaitExpressionWalker walker = VisitStatements();

                        HashSet<AwaitExpressionSyntax> awaitExpressions = walker.AwaitExpressions;

                        if (awaitExpressions.Count == 1)
                        {
                            if (VerifyTypes(node, awaitExpression, semanticModel, cancellationToken))
                                return new RemoveAsyncAwaitAnalysis(walker);
                        }
                        else if (awaitExpressions.Count > 1)
                        {
                            StatementSyntax prevStatement = statements[statements.IndexOf(returnStatement) - 1];

                            switch (prevStatement.Kind())
                            {
                                case SyntaxKind.IfStatement:
                                    {
                                        if (VerifyIfStatement((IfStatementSyntax)prevStatement, awaitExpressions.Count - 1, endsWithElse: false)
                                            && VerifyTypes(node, awaitExpressions, semanticModel, cancellationToken))
                                        {
                                            return new RemoveAsyncAwaitAnalysis(walker);
                                        }

                                        break;
                                    }
                                case SyntaxKind.SwitchStatement:
                                    {
                                        if (VerifySwitchStatement((SwitchStatementSyntax)prevStatement, awaitExpressions.Count - 1, containsDefaultSection: false)
                                            && VerifyTypes(node, awaitExpressions, semanticModel, cancellationToken))
                                        {
                                            return new RemoveAsyncAwaitAnalysis(walker);
                                        }

                                        break;
                                    }
                            }
                        }

                        return default;
                    }
                case SyntaxKind.IfStatement:
                    {
                        AwaitExpressionWalker walker = VisitStatements();

                        HashSet<AwaitExpressionSyntax> awaitExpressions = walker.AwaitExpressions;

                        if (awaitExpressions.Count > 0
                            && VerifyIfStatement((IfStatementSyntax)statement, awaitExpressions.Count, endsWithElse: true)
                            && VerifyTypes(node, awaitExpressions, semanticModel, cancellationToken))
                        {
                            return new RemoveAsyncAwaitAnalysis(walker);
                        }

                        return default;
                    }
                case SyntaxKind.SwitchStatement:
                    {
                        AwaitExpressionWalker walker = VisitStatements();

                        HashSet<AwaitExpressionSyntax> awaitExpressions = walker.AwaitExpressions;

                        if (awaitExpressions.Count > 0
                            && VerifySwitchStatement((SwitchStatementSyntax)statement, awaitExpressions.Count, containsDefaultSection: true)
                            && VerifyTypes(node, awaitExpressions, semanticModel, cancellationToken))
                        {
                            return new RemoveAsyncAwaitAnalysis(walker);
                        }

                        return default;
                    }
            }

            return default;

            AwaitExpressionWalker VisitStatements()
            {
                AwaitExpressionWalker walker = AwaitExpressionWalker.GetInstance();

                walker.VisitStatements(statements, statement);

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
            if (!SymbolEqualityComparer.Default.Equals(typeArgument, semanticModel.GetTypeSymbol(awaitExpression, cancellationToken)))
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

        private static IMethodSymbol GetMethodSymbol(
            SyntaxNode node,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
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

        public void Dispose()
        {
            if (Walker != null)
            {
                AwaitExpressionWalker.Free(Walker);
            }
        }
    }
}
