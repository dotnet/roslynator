// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;

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

                TypeAnalysisFlags flags = CSharpAnalysis.AnalyzeType(declarationExpression, semanticModel, context.CancellationToken);

                if (flags.IsExplicit())
                {
                    if (flags.SupportsImplicit()
                        && flags.IsValidSymbol()
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
                else if (flags.SupportsExplicit()
                     && flags.IsValidSymbol()
                     && context.IsRefactoringEnabled(RefactoringIdentifiers.ChangeVarToExplicitType))
                {
                    TypeSyntax type = declarationExpression.Type;

                    var localSymbol = semanticModel.GetDeclaredSymbol(declarationExpression.Designation, context.CancellationToken) as ILocalSymbol;

                    ITypeSymbol typeSymbol = localSymbol.Type;

                    context.RegisterRefactoring(
                        $"Change type to '{SymbolDisplay.GetMinimalString(typeSymbol, semanticModel, type.Span.Start)}'",
                        cancellationToken => ChangeTypeRefactoring.ChangeTypeAsync(context.Document, type, typeSymbol, cancellationToken));
                }
            }
        }
    }
}