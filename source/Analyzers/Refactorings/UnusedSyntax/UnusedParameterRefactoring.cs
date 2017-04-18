// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
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
                && !UnusedMethodParameterRefactoring.IsReferencedAsMethodGroup(methodDeclaration, context.SemanticModel, context.CancellationToken))
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

        public static void LocalFunctionStatement(SyntaxNodeAnalysisContext context)
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
