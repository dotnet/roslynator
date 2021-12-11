// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeActions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ConditionalExpressionRefactoring
    {
        internal static readonly string ConvertConditionalExpressionToIfElseRecursiveEquivalenceKey = EquivalenceKey.Create(RefactoringIdentifiers.ConvertConditionalExpressionToIfElse, "Recursive");

        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ConditionalExpressionSyntax conditionalExpression)
        {
            if (context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(conditionalExpression))
            {
                if (context.IsRefactoringEnabled(RefactoringDescriptors.WrapConditionalExpression))
                {
                    if (conditionalExpression.IsSingleLine())
                    {
                        context.RegisterRefactoring(
                            "Wrap ?:",
                            ct => SyntaxFormatter.WrapConditionalExpressionAsync(context.Document, conditionalExpression, ct),
                            RefactoringDescriptors.WrapConditionalExpression);
                    }
                    else if (conditionalExpression.DescendantTrivia(conditionalExpression.Span).All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                    {
                        context.RegisterRefactoring(
                            "Unwrap ?:",
                            ct => SyntaxFormatter.UnwrapExpressionAsync(context.Document, conditionalExpression, ct),
                            RefactoringDescriptors.WrapConditionalExpression);
                    }
                }

                if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertConditionalExpressionToIfElse))
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    (CodeAction codeAction, CodeAction recursiveCodeAction) = ConvertConditionalExpressionToIfElseRefactoring.ComputeRefactoring(
                        context.Document,
                        conditionalExpression,
                        new CodeActionData(ConvertConditionalExpressionToIfElseRefactoring.Title, EquivalenceKey.Create(RefactoringDescriptors.ConvertConditionalExpressionToIfElse)),
                        new CodeActionData(ConvertConditionalExpressionToIfElseRefactoring.RecursiveTitle, ConvertConditionalExpressionToIfElseRecursiveEquivalenceKey),
                        semanticModel,
                        context.CancellationToken);

                    if (codeAction != null)
                        context.RegisterRefactoring(codeAction);

                    if (recursiveCodeAction != null)
                        context.RegisterRefactoring(recursiveCodeAction);
                }
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.InvertConditionalExpression)
                && (context.Span.IsBetweenSpans(conditionalExpression)
                    || context.Span.IsEmptyAndContainedInSpan(conditionalExpression.QuestionToken, conditionalExpression.ColonToken))
                && InvertConditionalExpressionRefactoring.CanRefactor(conditionalExpression))
            {
                context.RegisterRefactoring(
                    "Invert ?:",
                    ct => InvertConditionalExpressionRefactoring.RefactorAsync(context.Document, conditionalExpression, ct),
                    RefactoringDescriptors.InvertConditionalExpression);
            }
        }
    }
}
