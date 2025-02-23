﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RemoveRedundantAsyncAwaitAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
            {
                Immutable.InterlockedInitialize(
                    ref _supportedDiagnostics,
                    DiagnosticRules.RemoveRedundantAsyncAwait,
                    DiagnosticRules.RemoveRedundantAsyncAwaitFadeOut);
            }

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(
            c =>
            {
                if (DiagnosticRules.RemoveRedundantAsyncAwait.IsEffective(c))
                    AnalyzeMethodDeclaration(c);
            },
            SyntaxKind.MethodDeclaration);

        context.RegisterSyntaxNodeAction(
            c =>
            {
                if (DiagnosticRules.RemoveRedundantAsyncAwait.IsEffective(c))
                    AnalyzeLocalFunctionStatement(c);
            },
            SyntaxKind.LocalFunctionStatement);

        context.RegisterSyntaxNodeAction(
            c =>
            {
                if (DiagnosticRules.RemoveRedundantAsyncAwait.IsEffective(c))
                    AnalyzeAnonymousMethodExpression(c);
            },
            SyntaxKind.AnonymousMethodExpression);

        context.RegisterSyntaxNodeAction(
            c =>
            {
                if (DiagnosticRules.RemoveRedundantAsyncAwait.IsEffective(c))
                    AnalyzeLambdaExpression(c);
            },
            SyntaxKind.SimpleLambdaExpression);

        context.RegisterSyntaxNodeAction(
            c =>
            {
                if (DiagnosticRules.RemoveRedundantAsyncAwait.IsEffective(c))
                    AnalyzeLambdaExpression(c);
            },
            SyntaxKind.ParenthesizedLambdaExpression);
    }

    private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;

        if (methodDeclaration.SpanContainsDirectives())
            return;

        SyntaxToken asyncKeyword = methodDeclaration.Modifiers.Find(SyntaxKind.AsyncKeyword);

        if (!asyncKeyword.IsKind(SyntaxKind.AsyncKeyword))
            return;

        using (RemoveAsyncAwaitAnalysis analysis = RemoveAsyncAwaitAnalysis.Create(methodDeclaration, context.SemanticModel, context.CancellationToken))
        {
            if (analysis.Success)
                ReportDiagnostic(context, asyncKeyword, analysis);
        }
    }

    private static void AnalyzeLocalFunctionStatement(SyntaxNodeAnalysisContext context)
    {
        var localFunction = (LocalFunctionStatementSyntax)context.Node;

        if (localFunction.SpanContainsDirectives())
            return;

        SyntaxToken asyncKeyword = localFunction.Modifiers.Find(SyntaxKind.AsyncKeyword);

        if (!asyncKeyword.IsKind(SyntaxKind.AsyncKeyword))
            return;

        using (RemoveAsyncAwaitAnalysis analysis = RemoveAsyncAwaitAnalysis.Create(localFunction, context.SemanticModel, context.CancellationToken))
        {
            if (analysis.Success)
                ReportDiagnostic(context, asyncKeyword, analysis);
        }
    }

    private static void AnalyzeAnonymousMethodExpression(SyntaxNodeAnalysisContext context)
    {
        var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;

        if (anonymousMethod.SpanContainsDirectives())
            return;

        SyntaxToken asyncKeyword = anonymousMethod.AsyncKeyword;

        if (!asyncKeyword.IsKind(SyntaxKind.AsyncKeyword))
            return;

        using (RemoveAsyncAwaitAnalysis analysis = RemoveAsyncAwaitAnalysis.Create(anonymousMethod, context.SemanticModel, context.CancellationToken))
        {
            if (analysis.Success)
                ReportDiagnostic(context, asyncKeyword, analysis);
        }
    }

    private static void AnalyzeLambdaExpression(SyntaxNodeAnalysisContext context)
    {
        var lambda = (LambdaExpressionSyntax)context.Node;

        if (lambda.SpanContainsDirectives())
            return;

        SyntaxToken asyncKeyword = lambda.AsyncKeyword;

        if (!asyncKeyword.IsKind(SyntaxKind.AsyncKeyword))
            return;

        using (RemoveAsyncAwaitAnalysis analysis = RemoveAsyncAwaitAnalysis.Create(lambda, context.SemanticModel, context.CancellationToken))
        {
            if (analysis.Success)
                ReportDiagnostic(context, asyncKeyword, analysis);
        }
    }

    private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxToken asyncKeyword, RemoveAsyncAwaitAnalysis analysis)
    {
        DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.RemoveRedundantAsyncAwait, asyncKeyword);
        DiagnosticHelpers.ReportToken(context, DiagnosticRules.RemoveRedundantAsyncAwaitFadeOut, asyncKeyword);

        if (analysis.AwaitExpression is not null)
        {
            ReportAwaitAndConfigureAwait(analysis.AwaitExpression);
        }
        else
        {
            foreach (AwaitExpressionSyntax awaitExpression in analysis.Walker.AwaitExpressions)
                ReportAwaitAndConfigureAwait(awaitExpression);
        }

        void ReportAwaitAndConfigureAwait(AwaitExpressionSyntax awaitExpression)
        {
            DiagnosticHelpers.ReportToken(context, DiagnosticRules.RemoveRedundantAsyncAwaitFadeOut, awaitExpression.AwaitKeyword);

            ExpressionSyntax expression = awaitExpression.Expression;

            ITypeSymbol typeSymbol = context.SemanticModel.GetTypeSymbol(expression, context.CancellationToken);

            if (typeSymbol?.OriginalDefinition.IsAwaitable(context.SemanticModel, expression.SpanStart) == true
                && (expression is InvocationExpressionSyntax invocation))
            {
                var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;

                if (string.Equals(memberAccess?.Name?.Identifier.ValueText, "ConfigureAwait", StringComparison.Ordinal))
                {
                    DiagnosticHelpers.ReportNode(context, DiagnosticRules.RemoveRedundantAsyncAwaitFadeOut, memberAccess.Name);
                    DiagnosticHelpers.ReportToken(context, DiagnosticRules.RemoveRedundantAsyncAwaitFadeOut, memberAccess.OperatorToken);
                    DiagnosticHelpers.ReportNode(context, DiagnosticRules.RemoveRedundantAsyncAwaitFadeOut, invocation.ArgumentList);
                }
            }
        }
    }
}
