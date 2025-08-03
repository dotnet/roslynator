﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings;

internal static class ForEachStatementRefactoring
{
    public static async Task ComputeRefactoringsAsync(RefactoringContext context, ForEachStatementSyntax forEachStatement)
    {
        if (context.IsAnyRefactoringEnabled(
            RefactoringDescriptors.UseImplicitType,
            RefactoringDescriptors.UseExplicitType,
            RefactoringDescriptors.ChangeTypeAccordingToExpression))
        {
            await ChangeTypeAsync(context, forEachStatement).ConfigureAwait(false);
        }

        if (context.IsRefactoringEnabled(RefactoringDescriptors.RenameIdentifierAccordingToTypeName))
            await RenameIdentifierAccordingToTypeNameAsync(context, forEachStatement).ConfigureAwait(false);

        SemanticModel semanticModel = null;

        if (context.IsAnyRefactoringEnabled(RefactoringDescriptors.ConvertForEachToFor, RefactoringDescriptors.ConvertForEachToForAndReverseLoop)
            && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(forEachStatement))
        {
            semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(forEachStatement.Expression, context.CancellationToken);
            ITypeSymbol elementTypeSymbol = semanticModel.GetTypeSymbol(forEachStatement.Type, context.CancellationToken);

            if (SymbolUtility.HasAccessibleIndexer(typeSymbol, elementTypeSymbol, semanticModel, forEachStatement.SpanStart))
            {
                if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertForEachToFor))
                {
                    context.RegisterRefactoring(
                        "Convert to 'for'",
                        ct => ConvertForEachToForRefactoring.RefactorAsync(context.Document, forEachStatement, semanticModel: semanticModel, reverseLoop: false, cancellationToken: ct),
                        RefactoringDescriptors.ConvertForEachToFor);
                }

                if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertForEachToForAndReverseLoop))
                {
                    context.RegisterRefactoring(
                        "Convert to 'for' and reverse loop",
                        ct => ConvertForEachToForRefactoring.RefactorAsync(context.Document, forEachStatement, semanticModel: semanticModel, reverseLoop: true, cancellationToken: ct),
                        RefactoringDescriptors.ConvertForEachToForAndReverseLoop);
                }
            }
        }

        if (context.IsRefactoringEnabled(RefactoringDescriptors.DeconstructForeachVariable)
            && TextSpan.FromBounds(forEachStatement.Type.SpanStart, forEachStatement.Identifier.Span.End).Contains(context.Span))
        {
            semanticModel ??= await context.GetSemanticModelAsync().ConfigureAwait(false);

            DeconstructForeachVariableRefactoring.ComputeRefactoring(context, forEachStatement, semanticModel);
        }

        if (context.IsRefactoringEnabled(RefactoringDescriptors.UseEnumeratorExplicitly)
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
                && context.IsRefactoringEnabled(RefactoringDescriptors.UseImplicitType))
            {
                context.RegisterRefactoring(CodeActionFactory.ChangeTypeToVar(context.Document, type, equivalenceKey: EquivalenceKey.Create(RefactoringDescriptors.UseImplicitType)));
            }

            if (!forEachStatement.ContainsDiagnostics
                && context.IsRefactoringEnabled(RefactoringDescriptors.ChangeTypeAccordingToExpression))
            {
                ChangeTypeAccordingToExpression(context, forEachStatement, semanticModel);
            }
        }
        else if (analysis.SupportsExplicit
            && context.IsRefactoringEnabled(RefactoringDescriptors.UseExplicitType))
        {
            context.RegisterRefactoring(CodeActionFactory.UseExplicitType(context.Document, type, analysis.Symbol, semanticModel, equivalenceKey: EquivalenceKey.Create(RefactoringDescriptors.UseExplicitType)));
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

        context.RegisterRefactoring(CodeActionFactory.UseExplicitType(context.Document, forEachStatement.Type, elementType, semanticModel, equivalenceKey: EquivalenceKey.Create(RefactoringDescriptors.ChangeTypeAccordingToExpression)));
    }

    internal static async Task RenameIdentifierAccordingToTypeNameAsync(
        RefactoringContext context,
        ForEachStatementSyntax forEachStatement)
    {
        TypeSyntax type = forEachStatement.Type;

        if (type is null)
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

        if (newName is null)
            return;

        ISymbol symbol = semanticModel.GetDeclaredSymbol(forEachStatement, context.CancellationToken);

        context.RegisterRefactoring(
            $"Rename '{oldName}' to '{newName}'",
#if ROSLYN_4_4
            ct => Renamer.RenameSymbolAsync(context.Solution, symbol, default(SymbolRenameOptions), newName, ct),
#else
            ct => Renamer.RenameSymbolAsync(context.Solution, symbol, newName, default(Microsoft.CodeAnalysis.Options.OptionSet), ct),
#endif
            RefactoringDescriptors.RenameIdentifierAccordingToTypeName);
    }
}
