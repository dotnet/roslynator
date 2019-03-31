// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.Refactorings.InlineDefinition;

namespace Roslynator.CSharp.Refactorings
{
    internal static class InvocationExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, InvocationExpressionSyntax invocationExpression)
        {
            if (context.IsAnyRefactoringEnabled(
                RefactoringIdentifiers.UseElementAccessInsteadOfEnumerableMethod,
                RefactoringIdentifiers.InvertLinqMethodCall,
                RefactoringIdentifiers.CallExtensionMethodAsInstanceMethod,
                RefactoringIdentifiers.CallIndexOfInsteadOfContains))
            {
                ExpressionSyntax expression = invocationExpression.Expression;

                if (expression != null
                    && invocationExpression.ArgumentList != null)
                {
                    if (expression.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                        && ((MemberAccessExpressionSyntax)expression).Name?.Span.Contains(context.Span) == true)
                    {
                        if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseElementAccessInsteadOfEnumerableMethod))
                            await UseElementAccessRefactoring.ComputeRefactoringsAsync(context, invocationExpression).ConfigureAwait(false);

                        if (context.IsRefactoringEnabled(RefactoringIdentifiers.InvertLinqMethodCall))
                        {
                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);
                            InvertLinqMethodCallRefactoring.ComputeRefactoring(context, invocationExpression, semanticModel);
                        }

                        if (context.IsRefactoringEnabled(RefactoringIdentifiers.CallIndexOfInsteadOfContains))
                            await CallIndexOfInsteadOfContainsRefactoring.ComputeRefactoringAsync(context, invocationExpression).ConfigureAwait(false);
                    }

                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.CallExtensionMethodAsInstanceMethod))
                    {
                        SyntaxNodeOrToken nodeOrToken = CallExtensionMethodAsInstanceMethodAnalysis.GetNodeOrToken(expression);

                        if (nodeOrToken.Span.Contains(context.Span))
                        {
                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            CallExtensionMethodAsInstanceMethodAnalysisResult analysis = CallExtensionMethodAsInstanceMethodAnalysis.Analyze(invocationExpression, semanticModel, allowAnyExpression: true, cancellationToken: context.CancellationToken);

                            if (analysis.Success)
                            {
                                context.RegisterRefactoring(
                                    CallExtensionMethodAsInstanceMethodRefactoring.Title,
                                    cancellationToken =>
                                    {
                                        return context.Document.ReplaceNodeAsync(
                                            analysis.InvocationExpression,
                                            analysis.NewInvocationExpression,
                                            cancellationToken);
                                    },
                                    RefactoringIdentifiers.CallExtensionMethodAsInstanceMethod);
                            }
                        }
                    }
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceStringFormatWithInterpolatedString)
                && context.SupportsCSharp6)
            {
                await ReplaceStringFormatWithInterpolatedStringRefactoring.ComputeRefactoringsAsync(context, invocationExpression).ConfigureAwait(false);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseBitwiseOperationInsteadOfCallingHasFlag))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                if (UseBitwiseOperationInsteadOfCallingHasFlagAnalysis.IsFixable(invocationExpression, semanticModel, context.CancellationToken))
                {
                    context.RegisterRefactoring(
                        UseBitwiseOperationInsteadOfCallingHasFlagRefactoring.Title,
                        cancellationToken =>
                        {
                            return UseBitwiseOperationInsteadOfCallingHasFlagRefactoring.RefactorAsync(
                                context.Document,
                                invocationExpression,
                                cancellationToken);
                        },
                        RefactoringIdentifiers.UseBitwiseOperationInsteadOfCallingHasFlag);
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.InlineMethod))
                await InlineMethodRefactoring.ComputeRefactoringsAsync(context, invocationExpression).ConfigureAwait(false);
        }
    }
}
