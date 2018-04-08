// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
                if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceConstantWithField)
                    && fieldDeclaration.Span.Contains(context.Span))
                {
                    context.RegisterRefactoring(
                        "Replace constant with field",
                        cancellationToken => ReplaceConstantWithFieldRefactoring.RefactorAsync(context.Document, fieldDeclaration, cancellationToken));
                }

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.InlineConstant)
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
                            cancellationToken => InlineConstantRefactoring.RefactorAsync(context.Document, fieldDeclaration, variableDeclarator, cancellationToken));
                    }
                }
            }
            else if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseConstantInsteadOfField)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(fieldDeclaration))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                if (UseConstantInsteadOfFieldAnalysis.IsFixable(fieldDeclaration, semanticModel, onlyPrivate: false, cancellationToken: context.CancellationToken))
                {
                    context.RegisterRefactoring(
                        "Use constant instead of field",
                        cancellationToken => UseConstantInsteadOfFieldRefactoring.RefactorAsync(context.Document, fieldDeclaration, cancellationToken));
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.InitializeFieldFromConstructor))
                InitializeFieldFromConstructorRefactoring.ComputeRefactoring(context, fieldDeclaration);
        }
    }
}
