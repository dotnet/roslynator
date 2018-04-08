// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseElementAccessInsteadOfEnumerableMethodRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, InvocationExpressionSyntax invocation)
        {
            SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocation);

            if (!invocationInfo.Success)
                return;

            switch (invocationInfo.NameText)
            {
                case "First":
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        if (invocationInfo.Arguments.Any())
                            break;

                        if (!UseElementAccessInsteadOfFirstAnalysis.IsFixable(invocationInfo, semanticModel, context.CancellationToken))
                            break;

                        context.RegisterRefactoring(
                            "Use [] instead of calling 'First'",
                            cancellationToken => UseElementAccessInsteadOfFirstRefactoring.RefactorAsync(context.Document, invocation, cancellationToken));

                        break;
                    }
                case "Last":
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        if (invocationInfo.Arguments.Any())
                            break;

                        if (!UseElementAccessInsteadOfLastAnalysis.IsFixable(invocationInfo, semanticModel, context.CancellationToken))
                            break;

                        string propertyName = CSharpUtility.GetCountOrLengthPropertyName(invocationInfo.Expression, semanticModel, context.CancellationToken);

                        if (propertyName == null)
                            break;

                        context.RegisterRefactoring(
                            "Use [] instead of calling 'Last'",
                            cancellationToken => UseElementAccessInsteadOfLastRefactoring.RefactorAsync(context.Document, invocation, propertyName, cancellationToken));

                        break;
                    }
                case "ElementAt":
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        if (invocationInfo.Arguments.Count != 1)
                            break;

                        if (!UseElementAccessInsteadOfElementAtAnalysis.IsFixable(invocationInfo, semanticModel, context.CancellationToken))
                            break;

                        context.RegisterRefactoring(
                            "Use [] instead of calling 'ElementAt'",
                            cancellationToken => UseElementAccessInsteadOfElementAtRefactoring.RefactorAsync(context.Document, invocation, cancellationToken));

                        break;
                    }
            }
        }
    }
}
