// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;
using Roslynator.Utilities;

namespace Roslynator.CSharp.Analyzers.UnusedParameter
{
    internal static class UnusedParameterRefactoring
    {
        public static void AnalyzeConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = (ConstructorDeclarationSyntax)context.Node;

            if (constructorDeclaration.ContainsDiagnostics)
                return;

            ParametersInfo parametersInfo = SyntaxInfo.ParametersInfo(constructorDeclaration);

            if (!parametersInfo.Success)
                return;

            if (ContainsOnlyThrowNewExpression(parametersInfo.Body))
                return;

            Analyze(context, parametersInfo);
        }

        public static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context, INamedTypeSymbol eventArgsSymbol)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            if (methodDeclaration.ContainsDiagnostics)
                return;

            if (methodDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                return;

            if (methodDeclaration.Modifiers.ContainsAny(SyntaxKind.AbstractKeyword, SyntaxKind.VirtualKeyword, SyntaxKind.OverrideKeyword))
                return;

            ParametersInfo parametersInfo = SyntaxInfo.ParametersInfo(methodDeclaration);

            if (!parametersInfo.Success)
                return;

            if (ContainsOnlyThrowNewExpression(parametersInfo.Body))
                return;

            IMethodSymbol methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken);

            if (methodSymbol == null)
                return;

            if (SymbolUtility.IsEventHandlerMethod(methodSymbol, eventArgsSymbol))
                return;

            if (!methodSymbol.ExplicitInterfaceImplementations.IsDefaultOrEmpty)
                return;

            if (methodSymbol.ImplementsInterfaceMember())
                return;

            Dictionary<string, NodeSymbolInfo> unusedNodes = FindUnusedNodes(context, parametersInfo);

            if (unusedNodes.Count == 0)
                return;

            if (IsReferencedAsMethodGroup(context, methodDeclaration))
                return;

            foreach (KeyValuePair<string, NodeSymbolInfo> kvp in unusedNodes)
                ReportDiagnostic(context, kvp.Value.Node);
        }

        public static void AnalyzeOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var operatorDeclaration = (OperatorDeclarationSyntax)context.Node;

            if (operatorDeclaration.ContainsDiagnostics)
                return;

            ParametersInfo parametersInfo = SyntaxInfo.ParametersInfo(operatorDeclaration);

            if (!parametersInfo.Success)
                return;

            if (ContainsOnlyThrowNewExpression(parametersInfo.Body))
                return;

            Analyze(context, parametersInfo);
        }

        public static void AnalyzeConversionOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var conversionOperatorDeclaration = (ConversionOperatorDeclarationSyntax)context.Node;

            if (conversionOperatorDeclaration.ContainsDiagnostics)
                return;

            ParametersInfo parametersInfo = SyntaxInfo.ParametersInfo(conversionOperatorDeclaration);

            if (!parametersInfo.Success)
                return;

            if (ContainsOnlyThrowNewExpression(parametersInfo.Body))
                return;

            Analyze(context, parametersInfo);
        }

        public static void AnalyzeIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;

            if (indexerDeclaration.ContainsDiagnostics)
                return;

            if (indexerDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                return;

            if (indexerDeclaration.Modifiers.ContainsAny(SyntaxKind.AbstractKeyword, SyntaxKind.VirtualKeyword, SyntaxKind.OverrideKeyword))
                return;

            ParametersInfo parametersInfo = SyntaxInfo.ParametersInfo(indexerDeclaration);

            if (!parametersInfo.Success)
                return;

            if (ContainsOnlyThrowNewExpression(parametersInfo.Body))
                return;

            IPropertySymbol propertySymbol = context.SemanticModel.GetDeclaredSymbol(indexerDeclaration, context.CancellationToken);

            if (propertySymbol?.ExplicitInterfaceImplementations.IsDefaultOrEmpty != true)
                return;

            if (propertySymbol.ImplementsInterfaceMember())
                return;

            Analyze(context, parametersInfo, isIndexer: true);
        }

        public static void AnalyzeLocalFunctionStatement(SyntaxNodeAnalysisContext context, INamedTypeSymbol eventArgsSymbol)
        {
            var localFunctionStatement = (LocalFunctionStatementSyntax)context.Node;

            if (localFunctionStatement.ContainsDiagnostics)
                return;

            ParametersInfo parametersInfo = SyntaxInfo.ParametersInfo(localFunctionStatement);

            if (!parametersInfo.Success)
                return;

            var methodSymbol = (IMethodSymbol)context.SemanticModel.GetDeclaredSymbol(localFunctionStatement, context.CancellationToken);

            if (methodSymbol == null)
                return;

            if (SymbolUtility.IsEventHandlerMethod(methodSymbol, eventArgsSymbol))
                return;

            Analyze(context, parametersInfo);
        }

        public static void AnalyzeSimpleLambdaExpression(SyntaxNodeAnalysisContext context, INamedTypeSymbol eventArgsSymbol)
        {
            var lambda = (SimpleLambdaExpressionSyntax)context.Node;

            if (lambda.ContainsDiagnostics)
                return;

            ParametersInfo parametersInfo = SyntaxInfo.ParametersInfo(lambda);

            if (!parametersInfo.Success)
                return;

            var methodSymbol = (IMethodSymbol)context.SemanticModel.GetSymbol(lambda, context.CancellationToken);

            if (methodSymbol == null)
                return;

            if (SymbolUtility.IsEventHandlerMethod(methodSymbol, eventArgsSymbol))
                return;

            Analyze(context, parametersInfo);
        }

        public static void AnalyzeParenthesizedLambdaExpression(SyntaxNodeAnalysisContext context, INamedTypeSymbol eventArgsSymbol)
        {
            var lambda = (ParenthesizedLambdaExpressionSyntax)context.Node;

            if (lambda.ContainsDiagnostics)
                return;

            ParametersInfo parametersInfo = SyntaxInfo.ParametersInfo(lambda);

            if (!parametersInfo.Success)
                return;

            var methodSymbol = (IMethodSymbol)context.SemanticModel.GetSymbol(lambda, context.CancellationToken);

            if (methodSymbol == null)
                return;

            if (SymbolUtility.IsEventHandlerMethod(methodSymbol, eventArgsSymbol))
                return;

            Analyze(context, parametersInfo);
        }

        public static void AnalyzeAnonymousMethodExpression(SyntaxNodeAnalysisContext context, INamedTypeSymbol eventArgsSymbol)
        {
            var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;

            if (anonymousMethod.ContainsDiagnostics)
                return;

            ParametersInfo parametersInfo = SyntaxInfo.ParametersInfo(anonymousMethod);

            if (!parametersInfo.Success)
                return;

            var methodSymbol = (IMethodSymbol)context.SemanticModel.GetSymbol(anonymousMethod, context.CancellationToken);

            if (methodSymbol == null)
                return;

            if (SymbolUtility.IsEventHandlerMethod(methodSymbol, eventArgsSymbol))
                return;

            Analyze(context, parametersInfo);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, ParametersInfo parametersInfo, bool isIndexer = false)
        {
            foreach (KeyValuePair<string, NodeSymbolInfo> kvp in FindUnusedNodes(context, parametersInfo, isIndexer))
                ReportDiagnostic(context, kvp.Value.Node);
        }

        private static Dictionary<string, NodeSymbolInfo> FindUnusedNodes(SyntaxNodeAnalysisContext context, ParametersInfo parametersInfo, bool isIndexer = false)
        {
            UnusedParameterWalker walker = UnusedParameterWalkerCache.Acquire(context.SemanticModel, context.CancellationToken, isIndexer);

            if (parametersInfo.Parameter != null
                && !StringUtility.IsOneOrManyUnderscores(parametersInfo.Parameter.Identifier.ValueText))
            {
                walker.AddParameter(parametersInfo.Parameter);
            }
            else
            {
                foreach (ParameterSyntax parameter in parametersInfo.Parameters)
                {
                    if (!StringUtility.IsOneOrManyUnderscores(parameter.Identifier.ValueText))
                        walker.AddParameter(parameter);
                }
            }

            foreach (TypeParameterSyntax typeParameter in parametersInfo.TypeParameters)
            {
                walker.AddTypeParameter(typeParameter);
                walker.IsAnyTypeParameter = true;
            }

            if (walker.Nodes.Count == 0)
                return walker.Nodes;

            walker.Visit(parametersInfo.Node);

            return UnusedParameterWalkerCache.GetNodesAndRelease(walker);
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxNode node)
        {
            if (node is ParameterSyntax parameter)
            {
                if (parameter.IsThis())
                {
                    context.ReportDiagnostic(DiagnosticDescriptors.UnusedThisParameter, parameter, parameter.Identifier.ValueText);
                }
                else
                {
                    context.ReportDiagnostic(DiagnosticDescriptors.UnusedParameter, parameter, parameter.Identifier.ValueText);
                }
            }
            else if (node is TypeParameterSyntax typeParameter)
            {
                context.ReportDiagnostic(DiagnosticDescriptors.UnusedTypeParameter, typeParameter, typeParameter.Identifier.ValueText);
            }
            else
            {
                Debug.Fail(node.ToString());
            }
        }

        //TODO: UnusedMemberWalker
        private static bool IsReferencedAsMethodGroup(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax methodDeclaration)
        {
            ISymbol methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken);

            string methodName = methodSymbol.Name;

            foreach (SyntaxReference syntaxReference in methodSymbol.ContainingType.DeclaringSyntaxReferences)
            {
                SyntaxNode typeDeclaration = syntaxReference.GetSyntax(context.CancellationToken);

                SemanticModel semanticModel = null;

                foreach (SyntaxNode node in typeDeclaration.DescendantNodes())
                {
                    if (node.IsKind(SyntaxKind.IdentifierName))
                    {
                        var identifierName = (IdentifierNameSyntax)node;

                        if (string.Equals(methodName, identifierName.Identifier.ValueText, StringComparison.Ordinal)
                            && !IsInvoked(identifierName))
                        {
                            if (semanticModel == null)
                            {
                                semanticModel = (context.SemanticModel.SyntaxTree == typeDeclaration.SyntaxTree)
                                    ? context.SemanticModel
                                    : context.Compilation.GetSemanticModel(typeDeclaration.SyntaxTree);
                            }

                            if (methodSymbol.Equals(semanticModel.GetSymbol(identifierName, context.CancellationToken)))
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        private static bool IsInvoked(IdentifierNameSyntax identifierName)
        {
            SyntaxNode parent = identifierName.Parent;

            switch (parent.Kind())
            {
                case SyntaxKind.InvocationExpression:
                    {
                        return true;
                    }
                case SyntaxKind.SimpleMemberAccessExpression:
                case SyntaxKind.MemberBindingExpression:
                    {
                        if (parent.IsParentKind(SyntaxKind.InvocationExpression))
                            return true;

                        break;
                    }
            }

            return false;
        }

        private static bool ContainsOnlyThrowNewExpression(CSharpSyntaxNode node)
        {
            switch (node?.Kind())
            {
                case SyntaxKind.Block:
                    return ContainsOnlyThrowNewExpression((BlockSyntax)node);
                case SyntaxKind.ArrowExpressionClause:
                    return ContainsOnlyThrowNewExpression((ArrowExpressionClauseSyntax)node);
                case SyntaxKind.AccessorList:
                    {
                        return ((AccessorListSyntax)node)
                            .Accessors
                            .All(f => ContainsOnlyThrowNewExpression(f.BodyOrExpressionBody()));
                    }
            }

            return false;
        }

        private static bool ContainsOnlyThrowNewExpression(BlockSyntax body)
        {
            StatementSyntax statement = body?.Statements.SingleOrDefault(shouldThrow: false);

            if (statement?.IsKind(SyntaxKind.ThrowStatement) == true)
            {
                var throwStatement = (ThrowStatementSyntax)statement;

                return throwStatement.Expression?.IsKind(SyntaxKind.ObjectCreationExpression) == true;
            }

            return false;
        }

        private static bool ContainsOnlyThrowNewExpression(ArrowExpressionClauseSyntax expressionBody)
        {
            ExpressionSyntax expression = expressionBody?.Expression;

            if (expression?.IsKind(SyntaxKind.ThrowExpression) == true)
            {
                var throwExpression = (ThrowExpressionSyntax)expression;

                return throwExpression.Expression?.IsKind(SyntaxKind.ObjectCreationExpression) == true;
            }

            return false;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            TypeParameterSyntax typeParameter,
            CancellationToken cancellationToken)
        {
            SyntaxNode node = typeParameter;

            var typeParameterList = (TypeParameterListSyntax)typeParameter.Parent;

            if (typeParameterList.Parameters.Count == 1)
                node = typeParameterList;

            SyntaxRemoveOptions options = RemoveHelper.DefaultRemoveOptions;

            if (node.GetLeadingTrivia().All(f => f.IsWhitespaceTrivia()))
                options &= ~SyntaxRemoveOptions.KeepLeadingTrivia;

            if (node.GetTrailingTrivia().All(f => f.IsWhitespaceTrivia()))
                options &= ~SyntaxRemoveOptions.KeepTrailingTrivia;

            return document.RemoveNodeAsync(node, options, cancellationToken);
        }
    }
}
