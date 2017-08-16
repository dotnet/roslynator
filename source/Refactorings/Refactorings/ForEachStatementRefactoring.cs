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

            if (context.IsAnyRefactoringEnabled(RefactoringIdentifiers.ReplaceForEachWithFor, RefactoringIdentifiers.ReplaceForEachWithForAndReverseLoop)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(forEachStatement))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                if (ReplaceForEachWithForRefactoring.CanRefactor(forEachStatement, semanticModel, context.CancellationToken))
                {
                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceForEachWithFor))
                    {
                        context.RegisterRefactoring(
                            "Replace foreach with for",
                            cancellationToken => ReplaceForEachWithForRefactoring.RefactorAsync(context.Document, forEachStatement, semanticModel: semanticModel, reverseLoop: false, cancellationToken: cancellationToken));
                    }

                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceForEachWithForAndReverseLoop))
                    {
                        context.RegisterRefactoring(
                            "Replace foreach with for and reverse loop",
                            cancellationToken => ReplaceForEachWithForRefactoring.RefactorAsync(context.Document, forEachStatement, semanticModel: semanticModel, reverseLoop: true, cancellationToken: cancellationToken));
                    }
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

                ITypeSymbol elementType = semanticModel.GetForEachStatementInfo(forEachStatement).ElementType;

                if (elementType?.IsErrorType() == false)
                {
                    ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(type).ConvertedType;

                    if (!elementType.Equals(typeSymbol))
                    {
                        if (elementType.SupportsExplicitDeclaration())
                        {
                            context.RegisterRefactoring(
                                $"Change type to '{SymbolDisplay.GetMinimalString(elementType, semanticModel, type.SpanStart)}'",
                                cancellationToken => ChangeTypeRefactoring.ChangeTypeAsync(context.Document, type, elementType, cancellationToken));
                        }
                        else
                        {
                            context.RegisterRefactoring(
                                "Change type to 'var'",
                                cancellationToken => ChangeTypeRefactoring.ChangeTypeToVarAsync(context.Document, type, cancellationToken));
                        }
                    }
                }
            }
        }
    }
}