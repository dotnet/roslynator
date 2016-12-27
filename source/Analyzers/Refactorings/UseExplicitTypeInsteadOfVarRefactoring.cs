// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseExplicitTypeInsteadOfVarRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, VariableDeclarationSyntax variableDeclaration)
        {
            if (variableDeclaration.Variables.Count != 1)
                return;

            TypeAnalysisResult result = CSharpUtility.AnalyzeType(
                variableDeclaration,
                context.SemanticModel,
                context.CancellationToken);

            switch (result)
            {
                case TypeAnalysisResult.Explicit:
                    {
                        break;
                    }
                case TypeAnalysisResult.ExplicitButShouldBeImplicit:
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.UseVarInsteadOfExplicitType,
                            variableDeclaration.Type.GetLocation());

                        break;
                    }
                case TypeAnalysisResult.Implicit:
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.UseExplicitTypeInsteadOfVarEvenIfObvious,
                            variableDeclaration.Type.GetLocation());

                        break;
                    }
                case TypeAnalysisResult.ImplicitButShouldBeExplicit:
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.UseExplicitTypeInsteadOfVar,
                            variableDeclaration.Type.GetLocation());

                        break;
                    }
            }
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, ForEachStatementSyntax forEachStatement)
        {
            TypeAnalysisResult result = CSharpUtility.AnalyzeType(forEachStatement, context.SemanticModel, context.CancellationToken);

            if (result == TypeAnalysisResult.ImplicitButShouldBeExplicit)
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.UseExplicitTypeInsteadOfVarInForEach,
                    forEachStatement.Type.GetLocation());
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            TypeSyntax type,
            ITypeSymbol typeSymbol,
            CancellationToken cancellationToken)
        {
            return await ChangeTypeRefactoring.ChangeTypeAsync(document, type, typeSymbol, cancellationToken).ConfigureAwait(false);
        }
    }
}
