// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class FieldDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, FieldDeclarationSyntax fieldDeclaration)
        {
            if (fieldDeclaration.Modifiers.Contains(SyntaxKind.ConstKeyword))
            {
                if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseReadOnlyFieldInsteadOfConstant)
                    && fieldDeclaration.Span.Contains(context.Span))
                {
                    context.RegisterRefactoring(
                        "Replace constant with field",
                        ct => UseReadOnlyFieldInsteadOfConstantRefactoring.RefactorAsync(context.Document, fieldDeclaration, ct),
                        RefactoringIdentifiers.UseReadOnlyFieldInsteadOfConstant);
                }

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.InlineConstantDeclaration)
                    && !fieldDeclaration.ContainsDiagnostics)
                {
                    VariableDeclaratorSyntax variableDeclarator = fieldDeclaration
                        .Declaration?
                        .Variables
                        .FirstOrDefault(f => context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(f.Identifier));

                    if (variableDeclarator != null)
                    {
                        context.RegisterRefactoring(
                            "Inline constant",
                            ct => InlineConstantDeclarationRefactoring.RefactorAsync(context.Document, fieldDeclaration, variableDeclarator, ct),
                            RefactoringIdentifiers.InlineConstantDeclaration);
                    }
                }
            }
            else if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseConstantInsteadOfReadOnlyField)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(fieldDeclaration))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                if (UseConstantInsteadOfFieldAnalysis.IsFixable(fieldDeclaration, semanticModel, onlyPrivate: false, cancellationToken: context.CancellationToken))
                {
                    context.RegisterRefactoring(
                        "Use constant instead of field",
                        ct => UseConstantInsteadOfReadOnlyFieldRefactoring.RefactorAsync(context.Document, fieldDeclaration, ct),
                        RefactoringIdentifiers.UseConstantInsteadOfReadOnlyField);
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.InitializeFieldFromConstructor))
                InitializeFieldFromConstructorRefactoring.ComputeRefactoring(context, fieldDeclaration);
        }
    }
}
