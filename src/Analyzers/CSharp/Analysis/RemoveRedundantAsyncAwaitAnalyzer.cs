// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
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
            context.RegisterSyntaxNodeAction(AnalyzeLambdaExpression, SyntaxKind.SimpleLambdaExpression);
            context.RegisterSyntaxNodeAction(AnalyzeLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression);
            context.RegisterSyntaxNodeAction(AnalyzeAnonymousMethodExpression, SyntaxKind.AnonymousMethodExpression);
        }

        //XPERF:
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
                        && !ContainsAwaitExpression(awaitExpression)
                        && ReturnTypeAndAwaitTypeEquals(lambda, awaitExpression, context.SemanticModel, context.CancellationToken)
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

                            AwaitExpressionSyntax awaitExpression = GetAwaitExpressionOrDefault(returnStatement);

                            if (awaitExpression != null)
                            {
                                HashSet<AwaitExpressionSyntax> awaitExpressions = CollectAwaitExpressions(body, TextSpan.FromBounds(body.SpanStart, returnStatement.Span.End));

                                if (awaitExpressions != null)
                                {
                                    if (awaitExpressions.Count == 1)
                                    {
                                        if (ReturnTypeAndAwaitTypeEquals(node, awaitExpression, context.SemanticModel, context.CancellationToken))
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
                                                Analyze(context, node, asyncKeyword, awaitExpression, awaitExpressions, GetAwaitExpressionsFromIfStatement((IfStatementSyntax)previousStatement, endsWithElse: false));
                                            }
                                            else if (previousStatementKind == SyntaxKind.SwitchStatement)
                                            {
                                                Analyze(context, node, asyncKeyword, awaitExpression, awaitExpressions, GetAwaitExpressionsFromSwitchStatement((SwitchStatementSyntax)previousStatement, containsDefaultSection: false));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (kind == SyntaxKind.IfStatement)
                        {
                            Analyze(context, node, body, asyncKeyword, GetAwaitExpressionsFromIfStatement((IfStatementSyntax)statement, endsWithElse: true));
                        }
                        else if (kind == SyntaxKind.SwitchStatement)
                        {
                            Analyze(context, node, body, asyncKeyword, GetAwaitExpressionsFromSwitchStatement((SwitchStatementSyntax)statement, containsDefaultSection: true));
                        }
                    }
                }
            }
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            SyntaxNode node,
            SyntaxToken asyncKeyword,
            AwaitExpressionSyntax awaitExpression,
            HashSet<AwaitExpressionSyntax> awaitExpressions,
            HashSet<AwaitExpressionSyntax> awaitExpressions2)
        {
            if (awaitExpressions2 != null)
            {
                awaitExpressions.ExceptWith(awaitExpressions2);

                if (awaitExpressions.Count == 1)
                {
                    awaitExpressions2.Add(awaitExpression);

                    if (ReturnTypeAndAwaitTypeEquals(node, awaitExpressions2, context.SemanticModel, context.CancellationToken))
                        ReportDiagnostic(context, asyncKeyword, awaitExpressions2);
                }
            }
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            SyntaxNode node,
            BlockSyntax body,
            SyntaxToken asyncKeyword,
            HashSet<AwaitExpressionSyntax> awaitExpressions)
        {
            if (awaitExpressions != null
                && !ContainsOtherAwaitOrReturnWithoutAwait(body, awaitExpressions)
                && ReturnTypeAndAwaitTypeEquals(node, awaitExpressions, context.SemanticModel, context.CancellationToken))
            {
                ReportDiagnostic(context, asyncKeyword, awaitExpressions);
            }
        }

        private static bool ContainsOtherAwaitOrReturnWithoutAwait(BlockSyntax body, HashSet<AwaitExpressionSyntax> awaitExpressions)
        {
            foreach (SyntaxNode descendant in body.DescendantNodes(body.Span, f => !CSharpFacts.IsFunction(f.Kind()) && !f.IsKind(SyntaxKind.ReturnStatement)))
            {
                switch (descendant.Kind())
                {
                    case SyntaxKind.ReturnStatement:
                        {
                            ExpressionSyntax expression = ((ReturnStatementSyntax)descendant).Expression;

                            if (expression?.Kind() == SyntaxKind.AwaitExpression)
                            {
                                if (!awaitExpressions.Contains((AwaitExpressionSyntax)expression))
                                    return true;
                            }
                            else
                            {
                                return true;
                            }

                            break;
                        }
                    case SyntaxKind.AwaitExpression:
                        {
                            return true;
                        }
                }
            }

            return false;
        }

        private static HashSet<AwaitExpressionSyntax> GetAwaitExpressionsFromIfStatement(
            IfStatementSyntax ifStatement,
            bool endsWithElse)
        {
            HashSet<AwaitExpressionSyntax> awaitExpressions = null;

            foreach (IfStatementOrElseClause ifOrElse in ifStatement.AsCascade())
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
                        for (int i = 0; i < statements.Count - 1; i++)
                        {
                            if (!statements[i].IsKind(SyntaxKind.LocalFunctionStatement)
                                && ContainsAwaitExpression(statements[i]))
                            {
                                return null;
                            }
                        }

                        return GetAwaitExpressionOrDefault(statements.Last());
                    }
                }
                else if (kind == SyntaxKind.ReturnStatement)
                {
                    return GetAwaitExpressionOrDefault((ReturnStatementSyntax)statement);
                }
            }

            return null;
        }

        private static AwaitExpressionSyntax GetAwaitExpressionOrDefault(StatementSyntax statement)
        {
            if (statement.IsKind(SyntaxKind.ReturnStatement))
            {
                return GetAwaitExpressionOrDefault((ReturnStatementSyntax)statement);
            }
            else
            {
                return null;
            }
        }

        private static AwaitExpressionSyntax GetAwaitExpressionOrDefault(ReturnStatementSyntax returnStatement)
        {
            ExpressionSyntax expression = returnStatement.Expression;

            if (expression?.Kind() == SyntaxKind.AwaitExpression)
            {
                var awaitExpression = (AwaitExpressionSyntax)expression;

                if (!ContainsAwaitExpression(awaitExpression))
                    return awaitExpression;
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

                if (expression?.Kind() == SyntaxKind.AwaitExpression)
                {
                    var awaitExpression = (AwaitExpressionSyntax)expression;

                    if (ReturnTypeAndAwaitTypeEquals(node, awaitExpression, context.SemanticModel, context.CancellationToken)
                        && !ContainsAwaitExpression(awaitExpression)
                        && !node.SpanContainsDirectives())
                    {
                        ReportDiagnostic(context, asyncKeyword, awaitExpression);
                    }
                }
            }
        }

        private static bool ReturnTypeAndAwaitTypeEquals(
            SyntaxNode node,
            HashSet<AwaitExpressionSyntax> awaitExpressions,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            INamedTypeSymbol taskOfT = semanticModel.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task_T);

            if (taskOfT == null)
                return false;

            IMethodSymbol methodSymbol = GetMethodSymbol(node, semanticModel, cancellationToken);

            if (methodSymbol == null)
                return false;

            var returnType = methodSymbol.ReturnType as INamedTypeSymbol;

            if (returnType?.ConstructedFrom.EqualsOrInheritsFrom(taskOfT) != true)
                return false;

            ITypeSymbol typeArgument = returnType.TypeArguments.SingleOrDefault(shouldThrow: false);

            if (typeArgument == null)
                return false;

            foreach (AwaitExpressionSyntax awaitExpression in awaitExpressions)
            {
                if (!typeArgument.Equals(semanticModel.GetTypeSymbol(awaitExpression, cancellationToken)))
                    return false;

                ExpressionSyntax expression = awaitExpression.Expression;

                var expressionTypeSymbol = semanticModel.GetTypeSymbol(expression, cancellationToken) as INamedTypeSymbol;

                if (expressionTypeSymbol == null)
                    return false;

                if (!expressionTypeSymbol.ConstructedFrom.EqualsOrInheritsFrom(taskOfT)
                    && !IsConfiguredTaskAwaitableOfT(expression, expressionTypeSymbol, semanticModel))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ReturnTypeAndAwaitTypeEquals(
            SyntaxNode node,
            AwaitExpressionSyntax awaitExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            INamedTypeSymbol taskOfT = semanticModel.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task_T);

            if (taskOfT == null)
                return false;

            IMethodSymbol methodSymbol = GetMethodSymbol(node, semanticModel, cancellationToken);

            if (methodSymbol == null)
                return false;

            var returnType = methodSymbol.ReturnType as INamedTypeSymbol;

            if (returnType?.ConstructedFrom.EqualsOrInheritsFrom(taskOfT) != true)
                return false;

            ITypeSymbol typeArgument = returnType.TypeArguments.SingleOrDefault(shouldThrow: false);

            if (typeArgument == null)
                return false;

            if (!typeArgument.Equals(semanticModel.GetTypeSymbol(awaitExpression, cancellationToken)))
                return false;

            ExpressionSyntax expression = awaitExpression.Expression;

            var expressionTypeSymbol = semanticModel.GetTypeSymbol(expression, cancellationToken) as INamedTypeSymbol;

            if (expressionTypeSymbol == null)
                return false;

            return expressionTypeSymbol.ConstructedFrom.EqualsOrInheritsFrom(taskOfT)
                || IsConfiguredTaskAwaitableOfT(expression, expressionTypeSymbol, semanticModel);
        }

        private static bool IsConfiguredTaskAwaitableOfT(
            ExpressionSyntax expression,
            INamedTypeSymbol expressionTypeSymbol,
            SemanticModel semanticModel)
        {
            SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(expression);

            if (!invocationInfo.Success)
                return false;

            if (invocationInfo.Arguments.Count != 1)
                return false;

            if (invocationInfo.NameText != "ConfigureAwait")
                return false;

            return expressionTypeSymbol.ConstructedFrom.Equals(semanticModel.GetTypeByMetadataName(MetadataNames.System_Runtime_CompilerServices_ConfiguredTaskAwaitable_T));
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
                default:
                    {
                        Debug.Fail(node.Kind().ToString());
                        return null;
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

            if (typeSymbol?.ConstructedFrom.Equals(semanticModel.GetTypeByMetadataName(MetadataNames.System_Runtime_CompilerServices_ConfiguredTaskAwaitable_T)) == true
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

        private static bool ContainsAwaitExpression(SyntaxNode node)
        {
            return node
                .DescendantNodes(f => !CSharpFacts.IsFunction(f.Kind()))
                .Any(f => f.IsKind(SyntaxKind.AwaitExpression));
        }

        private static HashSet<AwaitExpressionSyntax> CollectAwaitExpressions(BlockSyntax body, TextSpan span)
        {
            HashSet<AwaitExpressionSyntax> awaitExpressions = null;

            foreach (SyntaxNode node in body.DescendantNodes(span, f => !CSharpFacts.IsFunction(f.Kind()) && !f.IsKind(SyntaxKind.ReturnStatement)))
            {
                SyntaxKind kind = node.Kind();

                if (kind == SyntaxKind.ReturnStatement)
                {
                    ExpressionSyntax expression = ((ReturnStatementSyntax)node).Expression;

                    if (expression?.Kind() == SyntaxKind.AwaitExpression)
                    {
                        var awaitExpression = (AwaitExpressionSyntax)expression;

                        if (ContainsAwaitExpression(awaitExpression))
                            return null;

                        (awaitExpressions ?? (awaitExpressions = new HashSet<AwaitExpressionSyntax>())).Add(awaitExpression);
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (kind == SyntaxKind.AwaitExpression)
                {
                    return null;
                }
            }

            return awaitExpressions;
        }
    }
}
