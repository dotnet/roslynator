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
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(StatementCodeRefactoringProvider))]
    public class StatementCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            StatementSyntax statement = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<StatementSyntax>();

            if (statement == null)
                return;

            if (statement.IsKind(SyntaxKind.LocalDeclarationStatement))
            {
                var localDeclaration = (LocalDeclarationStatementSyntax)statement;

                if (localDeclaration.Declaration?.Span.Contains(context.Span) == true)
                {
                    await VariableDeclarationCodeRefactoringProvider.ComputeRefactoringsAsync(
                        context,
                        localDeclaration.Declaration);
                }
            }
            else if (statement.IsKind(SyntaxKind.UsingStatement))
            {
                var usingStatement = (UsingStatementSyntax)statement;

                if (usingStatement.Declaration?.Span.Contains(context.Span) == true)
                {
                    await VariableDeclarationCodeRefactoringProvider.ComputeRefactoringsAsync(
                        context,
                        usingStatement.Declaration);
                }
            }
            else if (statement.IsKind(SyntaxKind.ForEachStatement)
                && context.Document.SupportsSemanticModel)
            {
                var forEachStatement = (ForEachStatementSyntax)statement;

                SemanticModel semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

                ChangeType(context, semanticModel, forEachStatement);

                RenameIdentifierAccordingToTypeName(context, semanticModel, forEachStatement);

                ChangeTypeAccordingToExpression(context, semanticModel, forEachStatement);
            }

            AddBracesToEmbeddedStatementRefactoring.Refactor(context, statement);
            RemoveBracesFromStatementRefactoring.Refactor(context, statement);
            ExtractStatementRefactoring.Refactor(context, statement);
        }

        internal static void ChangeType(
            CodeRefactoringContext context,
            SemanticModel semanticModel,
            ForEachStatementSyntax forEachStatement)
        {
            TypeSyntax type = forEachStatement.Type;

            if (type?.Span.Contains(context.Span) != true)
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

        internal static void RenameIdentifierAccordingToTypeName(
            CodeRefactoringContext context,
            SemanticModel semanticModel,
            ForEachStatementSyntax forEachStatement)
        {
            if (forEachStatement.Type != null
                && forEachStatement.Identifier.Span.Contains(context.Span))
            {
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

        internal static void ChangeTypeAccordingToExpression(
            CodeRefactoringContext context,
            SemanticModel semanticModel,
            ForEachStatementSyntax forEachStatement)
        {
            if (forEachStatement.Type?.IsVar == false
                && forEachStatement.Type.Span.Contains(context.Span))
            {
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
