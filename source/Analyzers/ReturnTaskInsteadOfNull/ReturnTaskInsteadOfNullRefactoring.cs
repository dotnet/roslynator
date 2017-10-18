// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Analyzers.ReturnTaskInsteadOfNull
{
    internal static class ReturnTaskInsteadOfNullRefactoring
    {
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
                ExpressionSyntax expression = expressionBody.Expression;

                if (expression?.WalkDownParentheses().IsKind(SyntaxKind.NullLiteralExpression, SyntaxKind.DefaultExpression) != true)
                    return;

                if (!IsReturnTypeConstructedFromTaskOfT(methodDeclaration, taskOfTSymbol, context.SemanticModel, context.CancellationToken))
                    return;

                ReportDiagnostic(context, expression);
            }
            else
            {
                BlockSyntax body = methodDeclaration.Body;

                if (body == null)
                    return;

                if (!IsReturnTypeConstructedFromTaskOfT(methodDeclaration, taskOfTSymbol, context.SemanticModel, context.CancellationToken))
                    return;

                AnalyzeBlock(context, body);
            }
        }

        private static bool IsReturnTypeConstructedFromTaskOfT(MethodDeclarationSyntax methodDeclaration, INamedTypeSymbol taskOfTSymbol, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, cancellationToken);

            return methodSymbol?.IsErrorType() == false
                && methodSymbol.ReturnType.IsConstructedFrom(taskOfTSymbol);
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
                ExpressionSyntax expression = expressionBody.Expression;

                if (expression?.WalkDownParentheses().IsKind(SyntaxKind.NullLiteralExpression, SyntaxKind.DefaultExpression) != true)
                    return;

                if (!IsReturnTypeConstructedFromTaskOfT(localFunction, taskOfTSymbol, context.SemanticModel, context.CancellationToken))
                    return;

                ReportDiagnostic(context, expression);
            }
            else
            {
                BlockSyntax body = localFunction.Body;

                if (body == null)
                    return;

                if (!IsReturnTypeConstructedFromTaskOfT(localFunction, taskOfTSymbol, context.SemanticModel, context.CancellationToken))
                    return;

                AnalyzeBlock(context, body);
            }
        }

        private static bool IsReturnTypeConstructedFromTaskOfT(LocalFunctionStatementSyntax localFunction, INamedTypeSymbol taskOfTSymbol, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var methodSymbol = (IMethodSymbol)semanticModel.GetDeclaredSymbol(localFunction, cancellationToken);

            return methodSymbol?.IsErrorType() == false
                && methodSymbol.ReturnType.IsConstructedFrom(taskOfTSymbol);
        }

        public static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context, INamedTypeSymbol taskOfTSymbol)
        {
            var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;

            if ((propertyDeclaration.Type as GenericNameSyntax)?.TypeArgumentList?.Arguments.Count != 1)
                return;

            ArrowExpressionClauseSyntax expressionBody = propertyDeclaration.ExpressionBody;

            if (expressionBody != null)
            {
                ExpressionSyntax expression = expressionBody.Expression;

                if (expression?.WalkDownParentheses().IsKind(SyntaxKind.NullLiteralExpression, SyntaxKind.DefaultExpression) != true)
                    return;

                if (!IsReturnTypeConstructedFromTaskOfT(propertyDeclaration, taskOfTSymbol, context.SemanticModel, context.CancellationToken))
                    return;

                ReportDiagnostic(context, expression);
            }
            else
            {
                AccessorDeclarationSyntax getter = propertyDeclaration.Getter();

                if (getter == null)
                    return;

                if (!IsReturnTypeConstructedFromTaskOfT(propertyDeclaration, taskOfTSymbol, context.SemanticModel, context.CancellationToken))
                    return;

                AnalyzeGetAccessor(context, getter);
            }
        }

        private static bool IsReturnTypeConstructedFromTaskOfT(PropertyDeclarationSyntax propertyDeclaration, INamedTypeSymbol taskOfTSymbol, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, cancellationToken);

            return propertySymbol?.IsErrorType() == false
                && propertySymbol.Type.IsConstructedFrom(taskOfTSymbol);
        }

        public static void AnalyzeIndexerDeclaration(SyntaxNodeAnalysisContext context, INamedTypeSymbol taskOfTSymbol)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;

            if ((indexerDeclaration.Type as GenericNameSyntax)?.TypeArgumentList?.Arguments.Count != 1)
                return;

            ArrowExpressionClauseSyntax expressionBody = indexerDeclaration.ExpressionBody;

            if (expressionBody != null)
            {
                ExpressionSyntax expression = expressionBody.Expression;

                if (expression?.WalkDownParentheses().IsKind(SyntaxKind.NullLiteralExpression, SyntaxKind.DefaultExpression) != true)
                    return;

                if (!IsReturnTypeConstructedFromTaskOfT(indexerDeclaration, taskOfTSymbol, context.SemanticModel, context.CancellationToken))
                    return;

                ReportDiagnostic(context, expression);
            }
            else
            {
                AccessorDeclarationSyntax getter = indexerDeclaration.Getter();

                if (getter == null)
                    return;

                if (!IsReturnTypeConstructedFromTaskOfT(indexerDeclaration, taskOfTSymbol, context.SemanticModel, context.CancellationToken))
                    return;

                AnalyzeGetAccessor(context, getter);
            }
        }

        private static bool IsReturnTypeConstructedFromTaskOfT(IndexerDeclarationSyntax indexerDeclaration, INamedTypeSymbol taskOfTSymbol, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            IPropertySymbol propertySymbol = semanticModel.GetDeclaredSymbol(indexerDeclaration, cancellationToken);

            return propertySymbol?.IsErrorType() == false
                && propertySymbol.Type.IsConstructedFrom(taskOfTSymbol);
        }

        public static void AnalyzeLambdaExpression(SyntaxNodeAnalysisContext context, INamedTypeSymbol taskOfTSymbol)
        {
            var lambda = (LambdaExpressionSyntax)context.Node;

            if (lambda.AsyncKeyword.Kind() == SyntaxKind.AsyncKeyword)
                return;

            CSharpSyntaxNode body = lambda.Body;

            if (body is ExpressionSyntax expression)
            {
                if (expression?.WalkDownParentheses().IsKind(SyntaxKind.NullLiteralExpression, SyntaxKind.DefaultExpression) != true)
                    return;

                if (!IsReturnTypeConstructedFromTaskOfT(lambda, taskOfTSymbol, context.SemanticModel, context.CancellationToken))
                    return;

                ReportDiagnostic(context, expression);
            }
            else if (body is BlockSyntax block)
            {
                if (!IsReturnTypeConstructedFromTaskOfT(lambda, taskOfTSymbol, context.SemanticModel, context.CancellationToken))
                    return;

                AnalyzeBlock(context, block);
            }
        }

        public static void AnalyzeAnonymousMethod(SyntaxNodeAnalysisContext context, INamedTypeSymbol taskOfTSymbol)
        {
            var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;

            if (anonymousMethod.AsyncKeyword.Kind() == SyntaxKind.AsyncKeyword)
                return;

            if (!IsReturnTypeConstructedFromTaskOfT(anonymousMethod, taskOfTSymbol, context.SemanticModel, context.CancellationToken))
                return;

            AnalyzeBlock(context, anonymousMethod.Block);
        }

        private static bool IsReturnTypeConstructedFromTaskOfT(SyntaxNode node, INamedTypeSymbol taskOfTSymbol, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var methodSymbol = semanticModel.GetSymbol(node, cancellationToken) as IMethodSymbol;

            return methodSymbol?.IsErrorType() == false
                && methodSymbol.ReturnType.IsConstructedFrom(taskOfTSymbol);
        }

        private static void AnalyzeGetAccessor(SyntaxNodeAnalysisContext context, AccessorDeclarationSyntax getter)
        {
            ArrowExpressionClauseSyntax expressionBody = getter.ExpressionBody;

            if (expressionBody != null)
            {
                ExpressionSyntax expression = expressionBody.Expression;

                if (expression?
                    .WalkDownParentheses()
                    .IsKind(SyntaxKind.NullLiteralExpression, SyntaxKind.DefaultExpression) == true)
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

            ReturnTaskInsteadOfNullWalker walker = ReturnTaskInsteadOfNullWalkerCache.Acquire();

            walker.VisitBlock(body);

            foreach (ExpressionSyntax expression in ReturnTaskInsteadOfNullWalkerCache.GetExpressionsAndRelease(walker))
                ReportDiagnostic(context, expression);
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
        {
            context.ReportDiagnostic(DiagnosticDescriptors.ReturnTaskInsteadOfNull, expression);
        }

        public static InvocationExpressionSyntax CreateNewExpression(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            var typeSymbol = semanticModel.GetTypeInfo(expression, cancellationToken).ConvertedType as INamedTypeSymbol;

            int position = expression.SpanStart;

            TypeSyntax type = typeSymbol.ToMinimalTypeSyntax(semanticModel, position);

            InvocationExpressionSyntax newNode = SimpleMemberInvocationExpression(
                semanticModel.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task).ToMinimalTypeSyntax(semanticModel, position),
                GenericName("FromResult", typeSymbol.TypeArguments[0].ToMinimalTypeSyntax(semanticModel, position)),
                Argument(typeSymbol.ToDefaultValueSyntax(type)));

            return newNode.WithTriviaFrom(expression);
        }
    }
}
