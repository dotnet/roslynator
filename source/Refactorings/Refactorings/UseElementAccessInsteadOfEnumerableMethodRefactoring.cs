// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseElementAccessInsteadOfEnumerableMethodRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, InvocationExpressionSyntax invocation)
        {
            if (!MemberInvocationExpression.TryCreate(invocation, out MemberInvocationExpression memberInvocation))
                return;

            if (memberInvocation.ArgumentList == null)
                return;

            switch (memberInvocation.NameText)
            {
                case "First":
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        if (memberInvocation.ArgumentList.Arguments.Any())
                            break;

                        if (!UseElementAccessInsteadOfFirstRefactoring.CanRefactor(memberInvocation, semanticModel, context.CancellationToken))
                            break;

                        context.RegisterRefactoring(
                            "Use [] instead of calling 'First'",
                            cancellationToken => UseElementAccessInsteadOfFirstRefactoring.RefactorAsync(context.Document, invocation, cancellationToken));

                        break;
                    }
                case "Last":
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        if (memberInvocation.ArgumentList.Arguments.Any())
                            break;

                        if (!UseElementAccessInsteadOfLastRefactoring.CanRefactor(memberInvocation, semanticModel, context.CancellationToken))
                            break;

                        string propertyName = CSharpUtility.GetCountOrLengthPropertyName(memberInvocation.Expression, semanticModel, context.CancellationToken);

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

                        if (memberInvocation.ArgumentList.Arguments.Count != 1)
                            break;

                        if (!UseElementAccessInsteadOfElementAtRefactoring.CanRefactor(memberInvocation, semanticModel, context.CancellationToken))
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
