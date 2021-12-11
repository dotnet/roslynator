// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ConvertMethodGroupToLambdaRefactoring
    {
        public static Task ComputeRefactoringAsync(RefactoringContext context, IdentifierNameSyntax identifierName)
        {
            return ComputeRefactoringAsync(context, (ExpressionSyntax)identifierName);
        }

        public static Task ComputeRefactoringAsync(RefactoringContext context, MemberAccessExpressionSyntax memberAccessExpression)
        {
            return ComputeRefactoringAsync(context, (ExpressionSyntax)memberAccessExpression);
        }

        private static async Task ComputeRefactoringAsync(RefactoringContext context, ExpressionSyntax expression)
        {
            if (!context.Span.IsContainedInSpanOrBetweenSpans(expression))
                return;

            if (!ConvertMethodGroupToAnonymousFunctionAnalysis.CanBeMethodGroup(expression))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            if (semanticModel.GetSymbol(expression, context.CancellationToken) is not IMethodSymbol methodSymbol)
                return;

            context.RegisterRefactoring(
                "Convert to lambda",
                ct =>
                {
                    LambdaExpressionSyntax lambda = ConvertMethodGroupToAnonymousFunctionRefactoring.ConvertMethodGroupToAnonymousFunction(expression, semanticModel, ct);

                    return context.Document.ReplaceNodeAsync(expression, lambda, ct);
                },
                RefactoringDescriptors.ConvertMethodGroupToLambda);
        }
    }
}
