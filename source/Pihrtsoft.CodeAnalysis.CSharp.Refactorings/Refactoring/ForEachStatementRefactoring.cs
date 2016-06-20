// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Analysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ForEachStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ForEachStatementSyntax forEachStatement)
        {
            if (context.SupportsSemanticModel)
            {
                await ChangeTypeAccordingToExpressionAsync(context, forEachStatement);

                await ChangeTypeAsync(context, forEachStatement);

                await RenameIdentifierAccordingToTypeNameAsync(context, forEachStatement);

                if (ForEachToForRefactoring.CanRefactor(forEachStatement, await context.GetSemanticModelAsync(), context.CancellationToken))
                {
                    context.RegisterRefactoring(
                        "Convert foreach to for",
                        cancellationToken => ForEachToForRefactoring.RefactorAsync(context.Document, forEachStatement, cancellationToken));
                }
            }
        }

        internal static async Task ChangeTypeAsync(
            RefactoringContext context,
            ForEachStatementSyntax forEachStatement)
        {
            TypeSyntax type = forEachStatement.Type;

            if (type?.Span.Contains(context.Span) != true)
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync();

            TypeAnalysisResult result = ForEachStatementAnalysis.AnalyzeType(
                forEachStatement,
                semanticModel,
                context.CancellationToken);

            switch (result)
            {
                case TypeAnalysisResult.Explicit:
                    {
                        context.RegisterRefactoring(
                            "Change type to 'var'",
                            cancellationToken => TypeSyntaxRefactoring.ChangeTypeToImplicitAsync(context.Document, type, cancellationToken));

                        break;
                    }
                case TypeAnalysisResult.ImplicitButShouldBeExplicit:
                    {
                        ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(type, context.CancellationToken).Type;

                        context.RegisterRefactoring(
                            $"Change type to '{typeSymbol.ToDisplayString(TypeSyntaxRefactoring.SymbolDisplayFormat)}'",
                            cancellationToken => TypeSyntaxRefactoring.ChangeTypeToExplicitAsync(context.Document, type, typeSymbol, cancellationToken));

                        break;
                    }
            }
        }

        internal static async Task RenameIdentifierAccordingToTypeNameAsync(
            RefactoringContext context,
            ForEachStatementSyntax forEachStatement)
        {
            if (forEachStatement.Type != null
                && forEachStatement.Identifier.Span.Contains(context.Span))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync();

                string newName = NamingHelper.CreateIdentifierName(
                    forEachStatement.Type,
                    semanticModel,
                    firstCharToLower: true);

                if (!string.IsNullOrEmpty(newName)
                    && !string.Equals(newName, forEachStatement.Identifier.ValueText, StringComparison.Ordinal))
                {
                    ISymbol symbol = semanticModel.GetDeclaredSymbol(forEachStatement, context.CancellationToken);

                    context.RegisterRefactoring(
                        $"Rename foreach variable to '{newName}'",
                        cancellationToken => symbol.RenameAsync(newName, context.Document, cancellationToken));
                }
            }
        }

        internal static async Task ChangeTypeAccordingToExpressionAsync(
            RefactoringContext context,
            ForEachStatementSyntax forEachStatement)
        {
            if (forEachStatement.Type?.IsVar == false
                && forEachStatement.Type.Span.Contains(context.Span))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync();

                ForEachStatementInfo info = semanticModel.GetForEachStatementInfo(forEachStatement);

                if (info.ElementType != null)
                {
                    ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(forEachStatement.Type).ConvertedType;

                    if (!info.ElementType.Equals(typeSymbol))
                    {
                        context.RegisterRefactoring(
                            $"Change type to '{info.ElementType.ToDisplayString(TypeSyntaxRefactoring.SymbolDisplayFormat)}'",
                            cancellationToken =>
                            {
                                return TypeSyntaxRefactoring.ChangeTypeToExplicitAsync(
                                    context.Document,
                                    forEachStatement.Type,
                                    info.ElementType,
                                    cancellationToken);
                            });
                    }
                }
            }
        }
    }
}