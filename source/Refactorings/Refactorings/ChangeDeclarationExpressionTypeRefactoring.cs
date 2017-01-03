// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

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

                switch (CSharpAnalysis.AnalyzeType(
                    declarationExpression,
                    semanticModel,
                    context.CancellationToken))
                {
                    case TypeAnalysisResult.Explicit:
                    case TypeAnalysisResult.ExplicitButShouldBeImplicit:
                        {
                            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ChangeExplicitTypeToVar))
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

                            break;
                        }
                    case TypeAnalysisResult.Implicit:
                    case TypeAnalysisResult.ImplicitButShouldBeExplicit:
                        {
                            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ChangeVarToExplicitType))
                            {
                                TypeSyntax type = declarationExpression.Type;

                                var localSymbol = semanticModel.GetDeclaredSymbol(declarationExpression.Designation, context.CancellationToken) as ILocalSymbol;

                                ITypeSymbol typeSymbol = localSymbol.Type;

                                context.RegisterRefactoring(
                                    $"Change type to '{SymbolDisplay.GetMinimalDisplayString(typeSymbol, type.Span.Start, semanticModel)}'",
                                    cancellationToken => ChangeTypeRefactoring.ChangeTypeAsync(context.Document, type, typeSymbol, cancellationToken));
                            }

                            break;
                        }
                }
            }
        }
    }
}