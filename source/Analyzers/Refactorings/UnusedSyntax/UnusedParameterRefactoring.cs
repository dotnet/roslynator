// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings.UnusedSyntax
{
    internal static class UnusedParameterRefactoring
    {
        public static void AnalyzeConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = (ConstructorDeclarationSyntax)context.Node;

            foreach (ParameterSyntax parameter in UnusedConstructorParameterRefactoring.Instance.FindUnusedSyntax(constructorDeclaration, context.SemanticModel, context.CancellationToken))
                ReportUnusedParameter(context, parameter);
        }

        public static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            ImmutableArray<ParameterSyntax> unusedParameters = UnusedMethodParameterRefactoring.Instance.FindUnusedSyntax(methodDeclaration, context.SemanticModel, context.CancellationToken);

            if (unusedParameters.Any()
                && !IsReferencedAsMethodGroup(context, methodDeclaration))
            {
                foreach (ParameterSyntax parameter in unusedParameters)
                {
                    if (parameter.IsThis())
                    {
                        ReportUnusedThisParameter(context, parameter);
                    }
                    else
                    {
                        ReportUnusedParameter(context, parameter);
                    }
                }
            }
        }

        public static void AnalyzeIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;

            foreach (ParameterSyntax parameter in UnusedIndexerParameterRefactoring.Instance.FindUnusedSyntax(indexerDeclaration, context.SemanticModel, context.CancellationToken))
                ReportUnusedParameter(context, parameter);
        }

        public static void AnalyzeLocalFunctionStatement(SyntaxNodeAnalysisContext context)
        {
            var localFunctionStatement = (LocalFunctionStatementSyntax)context.Node;

            foreach (ParameterSyntax parameter in UnusedLocalFunctionParameterRefactoring.Instance.FindUnusedSyntax(localFunctionStatement, context.SemanticModel, context.CancellationToken))
                ReportUnusedParameter(context, parameter);
        }

        private static void ReportUnusedParameter(SyntaxNodeAnalysisContext context, ParameterSyntax parameter)
        {
            ReportDiagnostic(context, DiagnosticDescriptors.UnusedParameter, parameter);
        }

        private static void ReportUnusedThisParameter(SyntaxNodeAnalysisContext context, ParameterSyntax parameter)
        {
            ReportDiagnostic(context, DiagnosticDescriptors.UnusedThisParameter, parameter);
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor, ParameterSyntax parameter)
        {
            context.ReportDiagnostic(descriptor, parameter, parameter.Identifier.ValueText);
        }

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

        public static Task<Document> RefactorAsync(
            Document document,
            ParameterSyntax parameter,
            CancellationToken cancellationToken)
        {
            SyntaxRemoveOptions options = RemoveHelper.DefaultRemoveOptions;

            if (parameter.GetLeadingTrivia().All(f => f.IsWhitespaceTrivia()))
                options &= ~SyntaxRemoveOptions.KeepLeadingTrivia;

            if (parameter.GetTrailingTrivia().All(f => f.IsWhitespaceTrivia()))
                options &= ~SyntaxRemoveOptions.KeepTrailingTrivia;

            return document.RemoveNodeAsync(parameter, options, cancellationToken);
        }
    }
}
