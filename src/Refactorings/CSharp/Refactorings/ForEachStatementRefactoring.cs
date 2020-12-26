// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Rename;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ForEachStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ForEachStatementSyntax forEachStatement)
        {
            if (context.IsAnyRefactoringEnabled(
                RefactoringIdentifiers.ChangeExplicitTypeToVar,
                RefactoringIdentifiers.ChangeVarToExplicitType,
                RefactoringIdentifiers.ChangeTypeAccordingToExpression))
            {
                await ChangeTypeAsync(context, forEachStatement).ConfigureAwait(false);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RenameIdentifierAccordingToTypeName))
                await RenameIdentifierAccordingToTypeNameAsync(context, forEachStatement).ConfigureAwait(false);

            if (context.IsAnyRefactoringEnabled(RefactoringIdentifiers.ConvertForEachToFor, RefactoringIdentifiers.ConvertForEachToForAndReverseLoop)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(forEachStatement))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(forEachStatement.Expression, context.CancellationToken);

                if (SymbolUtility.HasAccessibleIndexer(typeSymbol, semanticModel, forEachStatement.SpanStart))
                {
                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.ConvertForEachToFor))
                    {
                        context.RegisterRefactoring(
                            "Convert to 'for'",
                            cancellationToken => ConvertForEachToForRefactoring.RefactorAsync(context.Document, forEachStatement, semanticModel: semanticModel, reverseLoop: false, cancellationToken: cancellationToken),
                            RefactoringIdentifiers.ConvertForEachToFor);
                    }

                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.ConvertForEachToForAndReverseLoop))
                    {
                        context.RegisterRefactoring(
                            "Convert to 'for' and reverse loop",
                            cancellationToken => ConvertForEachToForRefactoring.RefactorAsync(context.Document, forEachStatement, semanticModel: semanticModel, reverseLoop: true, cancellationToken: cancellationToken),
                            RefactoringIdentifiers.ConvertForEachToForAndReverseLoop);
                    }
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseEnumeratorExplicitly)
                && context.Span.IsEmptyAndContainedInSpan(forEachStatement.ForEachKeyword))
            {
                UseEnumeratorExplicitlyRefactoring.ComputeRefactoring(context, forEachStatement);
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

            TypeAnalysis analysis = CSharpTypeAnalysis.AnalyzeType(forEachStatement, semanticModel);

            if (analysis.IsExplicit)
            {
                if (analysis.SupportsImplicit
                    && context.IsRefactoringEnabled(RefactoringIdentifiers.ChangeExplicitTypeToVar))
                {
                    context.RegisterRefactoring(CodeActionFactory.ChangeTypeToVar(context.Document, type, equivalenceKey: RefactoringIdentifiers.ChangeExplicitTypeToVar));
                }

                if (!forEachStatement.ContainsDiagnostics
                    && context.IsRefactoringEnabled(RefactoringIdentifiers.ChangeTypeAccordingToExpression))
                {
                    ChangeTypeAccordingToExpression(context, forEachStatement, semanticModel);
                }
            }
            else if (analysis.SupportsExplicit
                && context.IsRefactoringEnabled(RefactoringIdentifiers.ChangeVarToExplicitType))
            {
                context.RegisterRefactoring(CodeActionFactory.ChangeType(context.Document, type, analysis.Symbol, semanticModel, equivalenceKey: RefactoringIdentifiers.ChangeVarToExplicitType));
            }
        }

        private static void ChangeTypeAccordingToExpression(
            RefactoringContext context,
            ForEachStatementSyntax forEachStatement,
            SemanticModel semanticModel)
        {
            ForEachStatementInfo info = semanticModel.GetForEachStatementInfo(forEachStatement);

            ITypeSymbol elementType = info.ElementType;

            if (elementType?.IsErrorType() != false)
                return;

            Conversion conversion = info.ElementConversion;

            if (conversion.IsIdentity)
                return;

            if (!conversion.IsImplicit)
                return;

            context.RegisterRefactoring(CodeActionFactory.ChangeType(context.Document, forEachStatement.Type, elementType, semanticModel, equivalenceKey: RefactoringIdentifiers.ChangeTypeAccordingToExpression));
        }

        internal static async Task RenameIdentifierAccordingToTypeNameAsync(
            RefactoringContext context,
            ForEachStatementSyntax forEachStatement)
        {
            TypeSyntax type = forEachStatement.Type;

            if (type == null)
                return;

            SyntaxToken identifier = forEachStatement.Identifier;

            if (!identifier.Span.Contains(context.Span))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, context.CancellationToken);

            if (typeSymbol?.IsErrorType() != false)
                return;

            string oldName = identifier.ValueText;

            string newName = NameGenerator.Default.CreateUniqueLocalName(
                typeSymbol,
                oldName,
                semanticModel,
                forEachStatement.SpanStart,
                cancellationToken: context.CancellationToken);

            if (newName == null)
                return;

            ISymbol symbol = semanticModel.GetDeclaredSymbol(forEachStatement, context.CancellationToken);

            context.RegisterRefactoring(
                $"Rename '{oldName}' to '{newName}'",
                cancellationToken => Renamer.RenameSymbolAsync(context.Solution, symbol, newName, default(OptionSet), cancellationToken),
                RefactoringIdentifiers.RenameIdentifierAccordingToTypeName);
        }
    }
}