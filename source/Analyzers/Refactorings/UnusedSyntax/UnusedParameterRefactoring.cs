// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings.UnusedSyntax
{
    internal static class UnusedParameterRefactoring
    {
        public static void AnalyzeConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var constructorDeclaration = (ConstructorDeclarationSyntax)context.Node;

            foreach (ParameterSyntax parameter in UnusedConstructorParameterRefactoring.Instance.FindUnusedSyntax(constructorDeclaration, context.SemanticModel, context.CancellationToken))
                ReportDiagnostic(context, parameter);
        }

        public static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            ImmutableArray<ParameterSyntax> unusedParameters = UnusedMethodParameterRefactoring.Instance.FindUnusedSyntax(methodDeclaration, context.SemanticModel, context.CancellationToken);

            if (unusedParameters.Any()
                && !UnusedMethodParameterRefactoring.IsReferencedAsMethodGroup(methodDeclaration, context.SemanticModel, context.CancellationToken))
            {
                foreach (ParameterSyntax parameter in unusedParameters)
                    ReportDiagnostic(context, parameter);
            }
        }

        public static void AnalyzeIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            var indexerDeclaration = (IndexerDeclarationSyntax)context.Node;

            foreach (ParameterSyntax parameter in UnusedIndexerParameterRefactoring.Instance.FindUnusedSyntax(indexerDeclaration, context.SemanticModel, context.CancellationToken))
                ReportDiagnostic(context, parameter);
        }

        private static void ReportDiagnostic(
            SyntaxNodeAnalysisContext context,
            ParameterSyntax parameter)
        {
            context.ReportDiagnostic(DiagnosticDescriptors.UnusedParameter, parameter, parameter.Identifier.ValueText);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ParameterSyntax parameter,
            CancellationToken cancellationToken)
        {
            SyntaxRemoveOptions options = Remover.DefaultRemoveOptions;

            if (parameter.GetLeadingTrivia().All(f => f.IsWhitespaceTrivia()))
                options &= ~SyntaxRemoveOptions.KeepLeadingTrivia;

            if (parameter.GetTrailingTrivia().All(f => f.IsWhitespaceTrivia()))
                options &= ~SyntaxRemoveOptions.KeepTrailingTrivia;

            return document.RemoveNodeAsync(parameter, options, cancellationToken);
        }
    }
}
