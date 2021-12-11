// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
                RefactoringDescriptors.UseElementAccessInsteadOfLinqMethod,
                RefactoringDescriptors.InvertLinqMethodCall,
                RefactoringDescriptors.CallExtensionMethodAsInstanceMethod,
                RefactoringDescriptors.CallIndexOfInsteadOfContains))
            {
                ExpressionSyntax expression = invocationExpression.Expression;

                if (expression != null
                    && invocationExpression.ArgumentList != null)
                {
                    if (expression.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                        && ((MemberAccessExpressionSyntax)expression).Name?.Span.Contains(context.Span) == true)
                    {
                        if (context.IsRefactoringEnabled(RefactoringDescriptors.UseElementAccessInsteadOfLinqMethod))
                            await UseElementAccessRefactoring.ComputeRefactoringsAsync(context, invocationExpression).ConfigureAwait(false);

                        if (context.IsRefactoringEnabled(RefactoringDescriptors.InvertLinqMethodCall))
                        {
                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);
                            InvertLinqMethodCallRefactoring.ComputeRefactoring(context, invocationExpression, semanticModel);
                        }

                        if (context.IsRefactoringEnabled(RefactoringDescriptors.CallIndexOfInsteadOfContains))
                            await CallIndexOfInsteadOfContainsRefactoring.ComputeRefactoringAsync(context, invocationExpression).ConfigureAwait(false);
                    }

                    if (context.IsRefactoringEnabled(RefactoringDescriptors.CallExtensionMethodAsInstanceMethod))
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
                                    ct =>
                                    {
                                        return context.Document.ReplaceNodeAsync(
                                            analysis.InvocationExpression,
                                            analysis.NewInvocationExpression,
                                            ct);
                                    },
                                    RefactoringDescriptors.CallExtensionMethodAsInstanceMethod);
                            }
                        }
                    }
                }
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertStringFormatToInterpolatedString)
                && context.SupportsCSharp6)
            {
                await ConvertStringFormatToInterpolatedStringRefactoring.ComputeRefactoringsAsync(context, invocationExpression).ConfigureAwait(false);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertHasFlagCallToBitwiseOperation))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                if (ConvertHasFlagCallToBitwiseOperationAnalysis.IsFixable(invocationExpression, semanticModel, context.CancellationToken))
                {
                    context.RegisterRefactoring(
                        ConvertHasFlagCallToBitwiseOperationRefactoring.Title,
                        ct =>
                        {
                            return ConvertHasFlagCallToBitwiseOperationRefactoring.RefactorAsync(
                                context.Document,
                                invocationExpression,
                                semanticModel,
                                ct);
                        },
                        RefactoringDescriptors.ConvertHasFlagCallToBitwiseOperation);
                }
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.InlineMethod))
                await InlineMethodRefactoring.ComputeRefactoringsAsync(context, invocationExpression).ConfigureAwait(false);
        }
    }
}
