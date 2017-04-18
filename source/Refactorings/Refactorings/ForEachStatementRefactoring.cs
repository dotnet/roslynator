// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Rename;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ForEachStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ForEachStatementSyntax forEachStatement)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ChangeTypeAccordingToExpression))
                await ChangeTypeAccordingToExpressionAsync(context, forEachStatement).ConfigureAwait(false);

            if (context.IsAnyRefactoringEnabled(
                RefactoringIdentifiers.ChangeExplicitTypeToVar,
                RefactoringIdentifiers.ChangeVarToExplicitType))
            {
                await ChangeTypeAsync(context, forEachStatement).ConfigureAwait(false);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RenameIdentifierAccordingToTypeName))
                await RenameIdentifierAccordingToTypeNameAsync(context, forEachStatement).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceForEachWithFor)
                && context.Span.IsEmpty
                && ReplaceForEachWithForRefactoring.CanRefactor(forEachStatement, await context.GetSemanticModelAsync().ConfigureAwait(false), context.CancellationToken))
            {
                context.RegisterRefactoring(
                    "Replace foreach with for",
                    cancellationToken => ReplaceForEachWithForRefactoring.RefactorAsync(context.Document, forEachStatement, cancellationToken));
            }
        }

        internal static async Task ChangeTypeAsync(
            RefactoringContext context,
            ForEachStatementSyntax forEachStatement)
        {
            TypeSyntax type = forEachStatement.Type;

            if (type?.Span.Contains(context.Span) != true)
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            TypeAnalysisFlags flags = CSharpAnalysis.AnalyzeType(forEachStatement, semanticModel);

            if (flags.IsExplicit())
            {
                if (flags.SupportsImplicit()
                    && flags.IsValidSymbol()
                    && context.IsRefactoringEnabled(RefactoringIdentifiers.ChangeExplicitTypeToVar))
                {
                    context.RegisterRefactoring(
                        "Change type to 'var'",
                        cancellationToken => ChangeTypeRefactoring.ChangeTypeToVarAsync(context.Document, type, cancellationToken));
                }
            }
            else if (flags.SupportsExplicit()
                && flags.IsValidSymbol()
                && context.IsRefactoringEnabled(RefactoringIdentifiers.ChangeVarToExplicitType))
            {
                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, context.CancellationToken);

                context.RegisterRefactoring(
                    $"Change type to '{SymbolDisplay.GetMinimalString(typeSymbol, semanticModel, type.Span.Start)}'",
                    cancellationToken => ChangeTypeRefactoring.ChangeTypeAsync(context.Document, type, typeSymbol, cancellationToken));
            }
        }

        internal static async Task RenameIdentifierAccordingToTypeNameAsync(
            RefactoringContext context,
            ForEachStatementSyntax forEachStatement)
        {
            TypeSyntax type = forEachStatement.Type;

            if (type != null)
            {
                SyntaxToken identifier = forEachStatement.Identifier;

                if (identifier.Span.Contains(context.Span))
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, context.CancellationToken);

                    if (typeSymbol?.IsErrorType() == false)
                    {
                        string oldName = identifier.ValueText;

                        string newName = NameGenerator.Default.CreateUniqueLocalName(
                            typeSymbol,
                            oldName,
                            semanticModel,
                            forEachStatement.SpanStart,
                            cancellationToken: context.CancellationToken);

                        if (newName != null)
                        {
                            ISymbol symbol = semanticModel.GetDeclaredSymbol(forEachStatement, context.CancellationToken);

                            context.RegisterRefactoring(
                                $"Rename '{oldName}' to '{newName}'",
                                cancellationToken => Renamer.RenameSymbolAsync(context.Solution, symbol, newName, default(OptionSet), cancellationToken));
                        }
                    }
                }
            }
        }

        internal static async Task ChangeTypeAccordingToExpressionAsync(
            RefactoringContext context,
            ForEachStatementSyntax forEachStatement)
        {
            TypeSyntax type = forEachStatement.Type;

            if (type?.IsVar == false
                && type.Span.Contains(context.Span))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                ForEachStatementInfo info = semanticModel.GetForEachStatementInfo(forEachStatement);

                if (info.ElementType != null)
                {
                    ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(type).ConvertedType;

                    if (!info.ElementType.Equals(typeSymbol))
                    {
                        context.RegisterRefactoring(
                            $"Change type to '{SymbolDisplay.GetMinimalString(info.ElementType, semanticModel, type.SpanStart)}'",
                            cancellationToken =>
                            {
                                return ChangeTypeRefactoring.ChangeTypeAsync(
                                    context.Document,
                                    type,
                                    info.ElementType,
                                    cancellationToken);
                            });
                    }
                }
            }
        }
    }
}