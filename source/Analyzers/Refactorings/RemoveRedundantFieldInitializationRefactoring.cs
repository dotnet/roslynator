// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantFieldInitializationRefactoring
    {
        internal static void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var fieldDeclaration = (FieldDeclarationSyntax)context.Node;

            if (!fieldDeclaration.ContainsDiagnostics
                && !fieldDeclaration.Modifiers.Contains(SyntaxKind.ConstKeyword))
            {
                VariableDeclarationSyntax declaration = fieldDeclaration.Declaration;
                if (declaration != null)
                {
                    foreach (VariableDeclaratorSyntax declarator in declaration.Variables)
                    {
                        EqualsValueClauseSyntax initializer = declarator.Initializer;
                        if (initializer?.ContainsDirectives == false)
                        {
                            ExpressionSyntax value = initializer.Value;
                            if (value != null)
                            {
                                SemanticModel semanticModel = context.SemanticModel;
                                CancellationToken cancellationToken = context.CancellationToken;

                                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(declaration.Type, cancellationToken);

                                if (typeSymbol?.IsErrorType() == false
                                    && semanticModel.IsDefaultValue(typeSymbol, value, cancellationToken))
                                {
                                    context.ReportDiagnostic(
                                        DiagnosticDescriptors.RemoveRedundantFieldInitialization,
                                        initializer);
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static Task<Document> RefactorAsync(
            Document document,
            VariableDeclaratorSyntax variableDeclarator,
            CancellationToken cancellationToken)
        {
            EqualsValueClauseSyntax initializer = variableDeclarator.Initializer;

            var removeOptions = SyntaxRemoveOptions.KeepExteriorTrivia;

            if (initializer.GetLeadingTrivia().IsEmptyOrWhitespace())
                removeOptions &= ~SyntaxRemoveOptions.KeepLeadingTrivia;

            if (initializer.GetTrailingTrivia().IsEmptyOrWhitespace())
                removeOptions &= ~SyntaxRemoveOptions.KeepTrailingTrivia;

            VariableDeclaratorSyntax newNode = variableDeclarator
                .RemoveNode(initializer, removeOptions)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(variableDeclarator, newNode, cancellationToken);
        }
    }
}