// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.UnusedParameter
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UnusedParameterAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.UnusedParameter,
                    DiagnosticDescriptors.UnusedThisParameter,
                    DiagnosticDescriptors.UnusedTypeParameter);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeOperatorDeclaration, SyntaxKind.OperatorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeConversionOperatorDeclaration, SyntaxKind.ConversionOperatorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeIndexerDeclaration, SyntaxKind.IndexerDeclaration);

            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeLocalFunctionStatement, SyntaxKind.LocalFunctionStatement);
            context.RegisterSyntaxNodeAction(AnalyzeSimpleLambdaExpression, SyntaxKind.SimpleLambdaExpression);
            context.RegisterSyntaxNodeAction(AnalyzeParenthesizedLambdaExpression, SyntaxKind.ParenthesizedLambdaExpression);
            context.RegisterSyntaxNodeAction(AnalyzeAnonymousMethodExpression, SyntaxKind.AnonymousMethodExpression);
        }

        public static void AnalyzeConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = (ConstructorDeclarationSyntax)context.Node;

            if (constructorDeclaration.ContainsDiagnostics)
                return;

            ParameterInfo parameterInfo = SyntaxInfo.ParameterInfo(constructorDeclaration);

            if (!parameterInfo.Success)
                return;

            if (ContainsOnlyThrowNewExpression(parameterInfo.Body))
                return;

            // Skip a constructor that is required by ISerializable interface
            if (parameterInfo.Parameters.Count == 2)
            {
                IMethodSymbol symbol = context.SemanticModel.GetDeclaredSymbol(constructorDeclaration, context.CancellationToken);

                if (symbol != null)
                {
                    ImmutableArray<IParameterSymbol> parameters = symbol.Parameters;

                    if (parameters.Length == 2
                        && parameters[0].Type.HasMetadataName(MetadataNames.System_Runtime_Serialization_SerializationInfo)
                        && parameters[1].Type.HasMetadataName(MetadataNames.System_Runtime_Serialization_StreamingContext))
                    {
                        return;
                    }
                }
            }

            Analyze(context, parameterInfo);
        }

        public static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            if (methodDeclaration.ContainsDiagnostics)
                return;

            if (methodDeclaration.IsParentKind(SyntaxKind.InterfaceDeclaration))
                return;

            if (methodDeclaration.Modifiers.ContainsAny(SyntaxKind.AbstractKeyword, SyntaxKind.VirtualKeyword, SyntaxKind.OverrideKeyword))
                return;

            ParameterInfo parameterInfo = SyntaxInfo.ParameterInfo(methodDeclaration);

            if (!parameterInfo.Success)
                return;

            if (ContainsOnlyThrowNewExpression(parameterInfo.Body))
                return;

            IMethodSymbol methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration, context.CancellationToken);

            if (methodSymbol == null)
                return;

            if (SymbolUtility.IsEventHandlerMethod(methodSymbol))
                return;

            if (!methodSymbol.ExplicitInterfaceImplementations.IsDefaultOrEmpty)
                return;

            if (methodSymbol.ImplementsInterfaceMember(allInterfaces: true))
                return;

            Dictionary<string, NodeSymbolInfo> unusedNodes = FindUnusedNodes(context, parameterInfo);

            if (unusedNodes.Count == 0)
                return;

            if (IsReferencedAsMethodGroup(context, methodDeclaration))
                return;

            foreach (KeyValuePair<string, NodeSymbolInfo> kvp in unusedNodes)
                ReportDiagnostic(context, kvp.Value.Node);

            unusedNodes.Clear();
        }

        public static void AnalyzeOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var operatorDeclaration = (OperatorDeclarationSyntax)context.Node;

            if (operatorDeclaration.ContainsDiagnostics)
                return;

            ParameterInfo parameterInfo = SyntaxInfo.ParameterInfo(operatorDeclaration);

            if (!parameterInfo.Success)
                return;

            if (ContainsOnlyThrowNewExpression(parameterInfo.Body))
                return;

            Analyze(context, parameterInfo);
        }

        public static void AnalyzeConversionOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var conversionOperatorDeclaration = (ConversionOperatorDeclarationSyntax)context.Node;

            if (conversionOperatorDeclaration.ContainsDiagnostics)
                return;

            ParameterInfo parameterInfo = SyntaxInfo.ParameterInfo(conversionOperatorDeclaration);

            if (!parameterInfo.Success)
                return;

            if (ContainsOnlyThrowNewExpression(parameterInfo.Body))
                return;

            Analyze(context, parameterInfo);
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

            ParameterInfo parameterInfo = SyntaxInfo.ParameterInfo(indexerDeclaration);

            if (!parameterInfo.Success)
                return;

            if (ContainsOnlyThrowNewExpression(parameterInfo.Body))
                return;

            IPropertySymbol propertySymbol = context.SemanticModel.GetDeclaredSymbol(indexerDeclaration, context.CancellationToken);

            if (propertySymbol?.ExplicitInterfaceImplementations.IsDefaultOrEmpty != true)
                return;

            if (propertySymbol.ImplementsInterfaceMember(allInterfaces: true))
                return;

            Analyze(context, parameterInfo, isIndexer: true);
        }

        public static void AnalyzeLocalFunctionStatement(SyntaxNodeAnalysisContext context)
        {
            var localFunctionStatement = (LocalFunctionStatementSyntax)context.Node;

            if (localFunctionStatement.ContainsDiagnostics)
                return;

            ParameterInfo parameterInfo = SyntaxInfo.ParameterInfo(localFunctionStatement);

            if (!parameterInfo.Success)
                return;

            IMethodSymbol methodSymbol = context.SemanticModel.GetDeclaredSymbol(localFunctionStatement, context.CancellationToken);

            if (methodSymbol == null)
                return;

            if (SymbolUtility.IsEventHandlerMethod(methodSymbol))
                return;

            Analyze(context, parameterInfo);
        }

        public static void AnalyzeSimpleLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var lambda = (SimpleLambdaExpressionSyntax)context.Node;

            if (lambda.ContainsDiagnostics)
                return;

            ParameterInfo parameterInfo = SyntaxInfo.ParameterInfo(lambda);

            if (!parameterInfo.Success)
                return;

            var methodSymbol = (IMethodSymbol)context.SemanticModel.GetSymbol(lambda, context.CancellationToken);

            if (methodSymbol == null)
                return;

            if (SymbolUtility.IsEventHandlerMethod(methodSymbol))
                return;

            Analyze(context, parameterInfo);
        }

        public static void AnalyzeParenthesizedLambdaExpression(SyntaxNodeAnalysisContext context)
        {
            var lambda = (ParenthesizedLambdaExpressionSyntax)context.Node;

            if (lambda.ContainsDiagnostics)
                return;

            ParameterInfo parameterInfo = SyntaxInfo.ParameterInfo(lambda);

            if (!parameterInfo.Success)
                return;

            var methodSymbol = (IMethodSymbol)context.SemanticModel.GetSymbol(lambda, context.CancellationToken);

            if (methodSymbol == null)
                return;

            if (SymbolUtility.IsEventHandlerMethod(methodSymbol))
                return;

            Analyze(context, parameterInfo);
        }

        public static void AnalyzeAnonymousMethodExpression(SyntaxNodeAnalysisContext context)
        {
            var anonymousMethod = (AnonymousMethodExpressionSyntax)context.Node;

            if (anonymousMethod.ContainsDiagnostics)
                return;

            ParameterInfo parameterInfo = SyntaxInfo.ParameterInfo(anonymousMethod);

            if (!parameterInfo.Success)
                return;

            var methodSymbol = (IMethodSymbol)context.SemanticModel.GetSymbol(anonymousMethod, context.CancellationToken);

            if (methodSymbol == null)
                return;

            if (SymbolUtility.IsEventHandlerMethod(methodSymbol))
                return;

            Analyze(context, parameterInfo);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, in ParameterInfo parameterInfo, bool isIndexer = false)
        {
            Dictionary<string, NodeSymbolInfo> unusedNodes = FindUnusedNodes(context, parameterInfo, isIndexer);

            foreach (KeyValuePair<string, NodeSymbolInfo> kvp in unusedNodes)
                ReportDiagnostic(context, kvp.Value.Node);

            unusedNodes.Clear();
        }

        private static Dictionary<string, NodeSymbolInfo> FindUnusedNodes(SyntaxNodeAnalysisContext context, in ParameterInfo parameterInfo, bool isIndexer = false)
        {
            UnusedParameterWalker walker = UnusedParameterWalkerCache.GetInstance(context.SemanticModel, context.CancellationToken, isIndexer);

            if (parameterInfo.Parameter != null
                && !StringUtility.IsOneOrManyUnderscores(parameterInfo.Parameter.Identifier.ValueText))
            {
                walker.AddParameter(parameterInfo.Parameter);
            }
            else
            {
                foreach (ParameterSyntax parameter in parameterInfo.Parameters)
                {
                    if (!StringUtility.IsOneOrManyUnderscores(parameter.Identifier.ValueText))
                        walker.AddParameter(parameter);
                }
            }

            foreach (TypeParameterSyntax typeParameter in parameterInfo.TypeParameters)
            {
                walker.AddTypeParameter(typeParameter);
                walker.IsAnyTypeParameter = true;
            }

            if (walker.Nodes.Count == 0)
                return walker.Nodes;

            walker.Visit(parameterInfo.Node);

            return UnusedParameterWalkerCache.GetNodesAndFree(walker);
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxNode node)
        {
            if (node is ParameterSyntax parameter)
            {
                if (parameter.Modifiers.Contains(SyntaxKind.ThisKeyword))
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

        //XPERF:
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

            if (statement?.Kind() == SyntaxKind.ThrowStatement)
            {
                var throwStatement = (ThrowStatementSyntax)statement;

                return throwStatement.Expression?.Kind() == SyntaxKind.ObjectCreationExpression;
            }

            return false;
        }

        private static bool ContainsOnlyThrowNewExpression(ArrowExpressionClauseSyntax expressionBody)
        {
            ExpressionSyntax expression = expressionBody?.Expression;

            if (expression?.Kind() == SyntaxKind.ThrowExpression)
            {
                var throwExpression = (ThrowExpressionSyntax)expression;

                return throwExpression.Expression?.Kind() == SyntaxKind.ObjectCreationExpression;
            }

            return false;
        }
    }
}
