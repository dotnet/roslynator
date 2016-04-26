// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Analysis;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(ForEachStatementCodeRefactoringProvider))]
    public class ForEachStatementCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            ForEachStatementSyntax forEachStatement = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<ForEachStatementSyntax>();

            if (forEachStatement == null)
                return;

            SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

            if (semanticModel == null)
                return;

            ChangeType(context, semanticModel, forEachStatement);

            RenameIdentifierNameAccordingToTypeName(context, semanticModel, forEachStatement);

            ChangeTypeAccordingToExpression(context, semanticModel, forEachStatement);

            if (ForEachToForRefactoring.CanRefactor(forEachStatement, semanticModel, context.CancellationToken))
            {
                context.RegisterRefactoring(
                    "Convert foreach to for",
                    cancellationToken => ForEachToForRefactoring.RefactorAsync(context.Document, forEachStatement, cancellationToken));
            }
        }

        private static void ChangeType(
            CodeRefactoringContext context,
            SemanticModel semanticModel,
            ForEachStatementSyntax forEachStatement)
        {
            TypeSyntax type = forEachStatement.Type;

            if (type == null || !type.Span.Contains(context.Span))
                return;

            TypeAnalysisResult result = ForEachStatementAnalysis.AnalyzeType(
                forEachStatement,
                semanticModel,
                context.CancellationToken);

            switch (result)
            {
                case TypeAnalysisResult.Explicit:
                    {
                        context.RegisterRefactoring(
                            $"Change type to 'var'",
                            cancellationToken => TypeSyntaxRefactoring.ChangeTypeToImplicitAsync(context.Document, type, context.CancellationToken));

                        break;
                    }
                case TypeAnalysisResult.ImplicitButShouldBeExplicit:
                    {
                        ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(type, context.CancellationToken).Type;

                        context.RegisterRefactoring(
                            $"Change type to '{typeSymbol.ToDisplayString(TypeSyntaxRefactoring.SymbolDisplayFormat)}'",
                            cancellationToken => TypeSyntaxRefactoring.ChangeTypeToExplicitAsync(context.Document, type, typeSymbol, context.CancellationToken));

                        break;
                    }
            }
        }

        private static void RenameIdentifierNameAccordingToTypeName(
            CodeRefactoringContext context,
            SemanticModel semanticModel,
            ForEachStatementSyntax forEachStatement)
        {
            if (forEachStatement.Type == null)
                return;

            if (!forEachStatement.Identifier.Span.Contains(context.Span))
                return;

            string newName = NamingHelper.CreateIdentifierName(
                forEachStatement.Type,
                semanticModel,
                firstCharToLower: true);

            if (string.Equals(newName, forEachStatement.Identifier.ToString(), StringComparison.Ordinal))
                return;

            ISymbol symbol = semanticModel.GetDeclaredSymbol(forEachStatement, context.CancellationToken);

            context.RegisterRefactoring(
                $"Rename foreach variable to '{newName}'",
                cancellationToken => symbol.RenameAsync(newName, context.Document, cancellationToken));
        }

        private static void ChangeTypeAccordingToExpression(
            CodeRefactoringContext context,
            SemanticModel semanticModel,
            ForEachStatementSyntax forEachStatement)
        {
            if (forEachStatement.Type?.IsVar != false)
                return;

            if (!forEachStatement.Type.Span.Contains(context.Span))
                return;

            ForEachStatementInfo forEachInfo = semanticModel.GetForEachStatementInfo(forEachStatement);

            ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(forEachStatement.Type).ConvertedType;

            if (forEachInfo.ElementType?.Equals(typeSymbol) != false)
                return;

            context.RegisterRefactoring(
                $"Change type to '{forEachInfo.ElementType.ToDisplayString(TypeSyntaxRefactoring.SymbolDisplayFormat)}'",
                cancellationToken => TypeSyntaxRefactoring.ChangeTypeToExplicitAsync(context.Document, forEachStatement.Type, forEachInfo.ElementType, cancellationToken));
        }
    }
}