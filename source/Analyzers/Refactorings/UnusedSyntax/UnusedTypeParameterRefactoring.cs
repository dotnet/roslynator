// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings.UnusedSyntax
{
    internal static class UnusedTypeParameterRefactoring
    {
        public static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            foreach (TypeParameterSyntax typeParameter in UnusedMethodTypeParameterRefactoring.Instance.FindUnusedSyntax(methodDeclaration, context.SemanticModel, context.CancellationToken))
                ReportDiagnostic(context, typeParameter);
        }

        public static void AnalyzeLocalFunctionStatement(SyntaxNodeAnalysisContext context)
        {
            var localFunctionStatement = (LocalFunctionStatementSyntax)context.Node;

            foreach (TypeParameterSyntax typeParameter in UnusedLocalFunctionTypeParameterRefactoring.Instance.FindUnusedSyntax(localFunctionStatement, context.SemanticModel, context.CancellationToken))
                ReportDiagnostic(context, typeParameter);
        }

        private static void ReportDiagnostic(
            SyntaxNodeAnalysisContext context,
            TypeParameterSyntax typeParameter)
        {
            context.ReportDiagnostic(DiagnosticDescriptors.UnusedTypeParameter, typeParameter, typeParameter.Identifier.ValueText);
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
