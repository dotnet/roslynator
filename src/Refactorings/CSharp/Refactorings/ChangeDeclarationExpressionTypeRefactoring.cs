// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ChangeDeclarationExpressionTypeRefactoring
    {
        public static async Task ComputeRefactoringsAsync(
            RefactoringContext context,
            DeclarationExpressionSyntax declarationExpression)
        {
            if (declarationExpression.Type?.Span.Contains(context.Span) == true
                && context.IsAnyRefactoringEnabled(
                    RefactoringIdentifiers.ChangeExplicitTypeToVar,
                    RefactoringIdentifiers.ChangeVarToExplicitType))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                TypeAnalysis analysis = TypeAnalysis.AnalyzeType(declarationExpression, semanticModel, context.CancellationToken);

                if (analysis.IsExplicit)
                {
                    if (analysis.SupportsImplicit
                        && context.IsRefactoringEnabled(RefactoringIdentifiers.ChangeExplicitTypeToVar))
                    {
                        context.RegisterRefactoring(
                            "Change type to 'var'",
                            cancellationToken =>
                            {
                                return ChangeTypeRefactoring.ChangeTypeToVarAsync(
                                    context.Document,
                                    declarationExpression.Type,
                                    cancellationToken);
                            });
                    }
                }
                else if (analysis.SupportsExplicit
                    && context.IsRefactoringEnabled(RefactoringIdentifiers.ChangeVarToExplicitType))
                {
                    TypeSyntax type = declarationExpression.Type;

                    var localSymbol = semanticModel.GetDeclaredSymbol(declarationExpression.Designation, context.CancellationToken) as ILocalSymbol;

                    ITypeSymbol typeSymbol = localSymbol.Type;

                    context.RegisterRefactoring(
                        $"Change type to '{SymbolDisplay.ToMinimalDisplayString(typeSymbol, semanticModel, type.SpanStart, SymbolDisplayFormats.Default)}'",
                        cancellationToken => ChangeTypeRefactoring.ChangeTypeAsync(context.Document, type, typeSymbol, cancellationToken));
                }
            }
        }
    }
}