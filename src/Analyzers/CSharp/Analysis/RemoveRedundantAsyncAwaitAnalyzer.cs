// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.Analysis.RemoveAsyncAwait;

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

            context.RegisterCompilationStartAction(startContext =>
            {
                if (startContext.IsAnalyzerSuppressed(DiagnosticDescriptors.RemoveRedundantAsyncAwait))
                    return;

                startContext.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
                startContext.RegisterSyntaxNodeAction(AnalyzeLocalFunctionStatement, SyntaxKind.LocalFunctionStatement);
                startContext.RegisterSyntaxNodeAction(AnalyzeAnonymousMethodExpression, SyntaxKind.AnonymousMethodExpression);
                startContext.RegisterSyntaxNodeAction(AnalyzeLambdaExpression, SyntaxKind.SimpleLambdaExpression);
                startContext.RegisterSyntaxNodeAction(AnalyzeLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression);
            });
        }

        public static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            if (methodDeclaration.SpanContainsDirectives())
                return;

            SyntaxToken asyncKeyword = methodDeclaration.Modifiers.Find(SyntaxKind.AsyncKeyword);

            if (!asyncKeyword.IsKind(SyntaxKind.AsyncKeyword))
                return;

            RemoveAsyncAwaitResult result = RemoveAsyncAwaitAnalysis.Analyze(methodDeclaration, context.SemanticModel, context.CancellationToken);

            if (!result.Success)
                return;

            ReportDiagnostic(context, asyncKeyword, result);
        }

        public static void AnalyzeLocalFunctionStatement(SyntaxNodeAnalysisContext context)
        {
            var localFunction = (LocalFunctionStatementSyntax)context.Node;

            if (localFunction.SpanContainsDirectives())
                return;

            SyntaxToken asyncKeyword = localFunction.Modifiers.Find(SyntaxKind.AsyncKeyword);

            if (!asyncKeyword.IsKind(SyntaxKind.AsyncKeyword))
                return;

            RemoveAsyncAwaitResult result = RemoveAsyncAwaitAnalysis.Analyze(localFunction, context.SemanticModel, context.CancellationToken);

            if (!result.Success)
                return;

            ReportDiagnostic(context, asyncKeyword, result);
        }

        public static void AnalyzeAnonymousMethodExpression(SyntaxNodeAnalysisContext context)
        {
            var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;

            if (anonymousMethod.SpanContainsDirectives())
                return;

            SyntaxToken asyncKeyword = anonymousMethod.AsyncKeyword;

            if (!asyncKeyword.IsKind(SyntaxKind.AsyncKeyword))
                return;

            RemoveAsyncAwaitResult result = RemoveAsyncAwaitAnalysis.Analyze(anonymousMethod, context.SemanticModel, context.CancellationToken);

            if (!result.Success)
                return;

            ReportDiagnostic(context, asyncKeyword, result);
        }

        public static void AnalyzeLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var lambda = (LambdaExpressionSyntax)context.Node;

            if (lambda.SpanContainsDirectives())
                return;

            SyntaxToken asyncKeyword = lambda.AsyncKeyword;

            if (!asyncKeyword.IsKind(SyntaxKind.AsyncKeyword))
                return;

            RemoveAsyncAwaitResult result = RemoveAsyncAwaitAnalysis.Analyze(lambda, context.SemanticModel, context.CancellationToken);

            if (!result.Success)
                return;

            ReportDiagnostic(context, asyncKeyword, result);
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxToken asyncKeyword, in RemoveAsyncAwaitResult result)
        {
            context.ReportDiagnostic(DiagnosticDescriptors.RemoveRedundantAsyncAwait, asyncKeyword);
            context.ReportToken(DiagnosticDescriptors.RemoveRedundantAsyncAwaitFadeOut, asyncKeyword);

            if (result.AwaitExpression != null)
            {
                ReportAwaitAndConfigureAwait(result.AwaitExpression);
            }
            else
            {
                foreach (AwaitExpressionSyntax awaitExpression in result.Walker.AwaitExpressions)
                    ReportAwaitAndConfigureAwait(awaitExpression);

                RemoveAsyncAwaitWalker.Free(result.Walker);
            }

            void ReportAwaitAndConfigureAwait(AwaitExpressionSyntax awaitExpression)
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
}
