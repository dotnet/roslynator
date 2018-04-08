// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis.ReturnTaskInsteadOfNull
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ReturnTaskInsteadOfReturningNullAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.ReturnTaskInsteadOfNull); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(startContext =>
            {
                INamedTypeSymbol taskOfTSymbol = startContext.Compilation.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task_T);

                if (taskOfTSymbol == null)
                    return;

                startContext.RegisterSyntaxNodeAction(nodeContext => AnalyzeMethodDeclaration(nodeContext, taskOfTSymbol), SyntaxKind.MethodDeclaration);
                startContext.RegisterSyntaxNodeAction(nodeContext => AnalyzePropertyDeclaration(nodeContext, taskOfTSymbol), SyntaxKind.PropertyDeclaration);
                startContext.RegisterSyntaxNodeAction(nodeContext => AnalyzeIndexerDeclaration(nodeContext, taskOfTSymbol), SyntaxKind.IndexerDeclaration);
                startContext.RegisterSyntaxNodeAction(nodeContext => AnalyzeLocalFunction(nodeContext, taskOfTSymbol), SyntaxKind.LocalFunctionStatement);
                startContext.RegisterSyntaxNodeAction(nodeContext => AnalyzeLambdaExpression(nodeContext, taskOfTSymbol), SyntaxKind.SimpleLambdaExpression);
                startContext.RegisterSyntaxNodeAction(nodeContext => AnalyzeLambdaExpression(nodeContext, taskOfTSymbol), SyntaxKind.ParenthesizedLambdaExpression);
                startContext.RegisterSyntaxNodeAction(nodeContext => AnalyzeAnonymousMethod(nodeContext, taskOfTSymbol), SyntaxKind.AnonymousMethodExpression);
            });
        }

        public static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context, INamedTypeSymbol taskOfTSymbol)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            if (methodDeclaration.Modifiers.Contains(SyntaxKind.AsyncKeyword))
                return;

            if ((methodDeclaration.ReturnType as GenericNameSyntax)?.TypeArgumentList?.Arguments.Count != 1)
                return;

            ArrowExpressionClauseSyntax expressionBody = methodDeclaration.ExpressionBody;

            if (expressionBody != null)
            {
                ExpressionSyntax expression = expressionBody.Expression?.WalkDownParentheses();

                if (expression?.IsKind(
                    SyntaxKind.NullLiteralExpression,
                    SyntaxKind.DefaultExpression,
                    SyntaxKind.DefaultLiteralExpression,
                    SyntaxKind.ConditionalAccessExpression) != true)
                {
                    return;
                }

                if (!IsReturnTypeConstructedFromTaskOfT())
                    return;

                ReportDiagnostic(context, expression);
            }
            else
            {
                BlockSyntax body = methodDeclaration.Body;

                if (body == null)
                    return;

                if (!IsReturnTypeConstructedFromTaskOfT())
                    return;

                AnalyzeBlock(context, body);
            }

            bool IsReturnTypeConstructedFromTaskOfT()
            {
                return context.SemanticModel
                    .GetDeclaredSymbol(methodDeclaration, context.CancellationToken)?
                    .ReturnType
                    .OriginalDefinition
                    .Equals(taskOfTSymbol) == true;
            }
        }

        public static void AnalyzeLocalFunction(SyntaxNodeAnalysisContext context, INamedTypeSymbol taskOfTSymbol)
        {
            var localFunction = (LocalFunctionStatementSyntax)context.Node;

            if (localFunction.Modifiers.Contains(SyntaxKind.AsyncKeyword))
                return;

            if ((localFunction.ReturnType as GenericNameSyntax)?.TypeArgumentList?.Arguments.Count != 1)
                return;

            ArrowExpressionClauseSyntax expressionBody = localFunction.ExpressionBody;

            if (expressionBody != null)
            {
                ExpressionSyntax expression = expressionBody.Expression?.WalkDownParentheses();

                if (expression?.IsKind(
                    SyntaxKind.NullLiteralExpression,
                    SyntaxKind.DefaultExpression,
                    SyntaxKind.DefaultLiteralExpression,
                    SyntaxKind.ConditionalAccessExpression) != true)
                {
                    return;
                }

                if (!IsReturnTypeConstructedFromTaskOfT())
                    return;

                ReportDiagnostic(context, expression);
            }
            else
            {
                BlockSyntax body = localFunction.Body;

                if (body == null)
                    return;

                if (!IsReturnTypeConstructedFromTaskOfT())
                    return;

                AnalyzeBlock(context, body);
            }

            bool IsReturnTypeConstructedFromTaskOfT()
            {
                return context.SemanticModel
                    .GetDeclaredSymbol(localFunction, context.CancellationToken)?
                    .ReturnType
                    .OriginalDefinition
                    .Equals(taskOfTSymbol) == true;
            }
        }

        public static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context, INamedTypeSymbol taskOfTSymbol)
        {
            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

            if ((propertyDeclaration.Type as GenericNameSyntax)?.TypeArgumentList?.Arguments.Count != 1)
                return;

            ArrowExpressionClauseSyntax expressionBody = propertyDeclaration.ExpressionBody;

            if (expressionBody != null)
            {
                ExpressionSyntax expression = expressionBody.Expression?.WalkDownParentheses();

                if (expression?.IsKind(
                    SyntaxKind.NullLiteralExpression,
                    SyntaxKind.DefaultExpression,
                    SyntaxKind.DefaultLiteralExpression,
                    SyntaxKind.ConditionalAccessExpression) != true)
                {
                    return;
                }

                if (!IsReturnTypeConstructedFromTaskOfT())
                    return;

                ReportDiagnostic(context, expression);
            }
            else
            {
                AccessorDeclarationSyntax getter = propertyDeclaration.Getter();

                if (getter == null)
                    return;

                if (!IsReturnTypeConstructedFromTaskOfT())
                    return;

                AnalyzeGetAccessor(context, getter);
            }

            bool IsReturnTypeConstructedFromTaskOfT()
            {
                return context.SemanticModel
                    .GetDeclaredSymbol(propertyDeclaration, context.CancellationToken)?
                    .Type
                    .OriginalDefinition
                    .Equals(taskOfTSymbol) == true;
            }
        }

        public static void AnalyzeIndexerDeclaration(SyntaxNodeAnalysisContext context, INamedTypeSymbol taskOfTSymbol)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;

            if ((indexerDeclaration.Type as GenericNameSyntax)?.TypeArgumentList?.Arguments.Count != 1)
                return;

            ArrowExpressionClauseSyntax expressionBody = indexerDeclaration.ExpressionBody;

            if (expressionBody != null)
            {
                ExpressionSyntax expression = expressionBody.Expression?.WalkDownParentheses();

                if (expression?.IsKind(
                    SyntaxKind.NullLiteralExpression,
                    SyntaxKind.DefaultExpression,
                    SyntaxKind.DefaultLiteralExpression,
                    SyntaxKind.ConditionalAccessExpression) != true)
                {
                    return;
                }

                if (!IsReturnTypeConstructedFromTaskOfT())
                    return;

                ReportDiagnostic(context, expression);
            }
            else
            {
                AccessorDeclarationSyntax getter = indexerDeclaration.Getter();

                if (getter == null)
                    return;

                if (!IsReturnTypeConstructedFromTaskOfT())
                    return;

                AnalyzeGetAccessor(context, getter);
            }

            bool IsReturnTypeConstructedFromTaskOfT()
            {
                return context.SemanticModel
                    .GetDeclaredSymbol(indexerDeclaration, context.CancellationToken)?
                    .Type
                    .OriginalDefinition
                    .Equals(taskOfTSymbol) == true;
            }
        }

        public static void AnalyzeLambdaExpression(SyntaxNodeAnalysisContext context, INamedTypeSymbol taskOfTSymbol)
        {
            var lambda = (LambdaExpressionSyntax)context.Node;

            if (lambda.AsyncKeyword.Kind() == SyntaxKind.AsyncKeyword)
                return;

            CSharpSyntaxNode body = lambda.Body;

            if (body is ExpressionSyntax expression)
            {
                expression = expression.WalkDownParentheses();

                if (expression?.IsKind(
                    SyntaxKind.NullLiteralExpression,
                    SyntaxKind.DefaultExpression,
                    SyntaxKind.DefaultLiteralExpression,
                    SyntaxKind.ConditionalAccessExpression) != true)
                {
                    return;
                }

                if (!IsReturnTypeConstructedFromTaskOfT())
                    return;

                ReportDiagnostic(context, expression);
            }
            else if (body is BlockSyntax block)
            {
                if (!IsReturnTypeConstructedFromTaskOfT())
                    return;

                AnalyzeBlock(context, block);
            }

            bool IsReturnTypeConstructedFromTaskOfT()
            {
                return context.SemanticModel
                    .GetMethodSymbol(lambda, context.CancellationToken)?
                    .ReturnType
                    .OriginalDefinition
                    .Equals(taskOfTSymbol) == true;
            }
        }

        public static void AnalyzeAnonymousMethod(SyntaxNodeAnalysisContext context, INamedTypeSymbol taskOfTSymbol)
        {
            var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;

            if (anonymousMethod.AsyncKeyword.Kind() == SyntaxKind.AsyncKeyword)
                return;

            if (!IsReturnTypeConstructedFromTaskOfT())
                return;

            AnalyzeBlock(context, anonymousMethod.Block);

            bool IsReturnTypeConstructedFromTaskOfT()
            {
                return context.SemanticModel
                    .GetMethodSymbol(anonymousMethod, context.CancellationToken)?
                    .ReturnType
                    .OriginalDefinition
                    .Equals(taskOfTSymbol) == true;
            }
        }

        private static void AnalyzeGetAccessor(SyntaxNodeAnalysisContext context, AccessorDeclarationSyntax getter)
        {
            ArrowExpressionClauseSyntax expressionBody = getter.ExpressionBody;

            if (expressionBody != null)
            {
                ExpressionSyntax expression = expressionBody.Expression?.WalkDownParentheses();

                if (expression?.IsKind(
                    SyntaxKind.NullLiteralExpression,
                    SyntaxKind.DefaultExpression,
                    SyntaxKind.DefaultLiteralExpression,
                    SyntaxKind.ConditionalAccessExpression) == true)
                {
                    ReportDiagnostic(context, expression);
                }
            }
            else
            {
                AnalyzeBlock(context, getter.Body);
            }
        }

        private static void AnalyzeBlock(SyntaxNodeAnalysisContext context, BlockSyntax body)
        {
            if (body == null)
                return;

            ReturnTaskInsteadOfNullWalker walker = ReturnTaskInsteadOfNullWalkerCache.GetInstance();

            walker.VisitBlock(body);

            foreach (ExpressionSyntax expression in ReturnTaskInsteadOfNullWalkerCache.GetExpressionsAndFree(walker))
                ReportDiagnostic(context, expression);
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
        {
            context.ReportDiagnostic(DiagnosticDescriptors.ReturnTaskInsteadOfNull, expression);
        }
    }
}
