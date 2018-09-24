// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseElementAccessRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, InvocationExpressionSyntax invocation)
        {
            if (invocation.IsParentKind(SyntaxKind.ExpressionStatement))
                return;

            SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocation);

            if (!invocationInfo.Success)
                return;

            switch (invocationInfo.NameText)
            {
                case "First":
                    {
                        if (invocationInfo.Arguments.Any())
                            break;

                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        if (!UseElementAccessAnalysis.IsFixableFirst(invocationInfo, semanticModel, context.CancellationToken))
                            break;

                        context.RegisterRefactoring(
                            "Use [] instead of calling 'First'",
                            cancellationToken => UseElementAccessInsteadOfEnumerableMethodRefactoring.UseElementAccessInsteadOfFirstAsync(context.Document, invocation, cancellationToken),
                            RefactoringIdentifiers.UseElementAccessInsteadOfEnumerableMethod);

                        break;
                    }
                case "Last":
                    {
                        if (invocationInfo.Arguments.Any())
                            break;

                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        if (!UseElementAccessAnalysis.IsFixableLast(invocationInfo, semanticModel, context.CancellationToken))
                            break;

                        string propertyName = CSharpUtility.GetCountOrLengthPropertyName(invocationInfo.Expression, semanticModel, context.CancellationToken);

                        if (propertyName == null)
                            break;

                        context.RegisterRefactoring(
                            "Use [] instead of calling 'Last'",
                            cancellationToken => UseElementAccessInsteadOfEnumerableMethodRefactoring.UseElementAccessInsteadOfLastAsync(context.Document, invocation, propertyName, cancellationToken),
                            RefactoringIdentifiers.UseElementAccessInsteadOfEnumerableMethod);

                        break;
                    }
                case "ElementAt":
                    {
                        if (invocationInfo.Arguments.SingleOrDefault(shouldThrow: false)?.Expression?.IsMissing != false)
                            break;

                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        if (!UseElementAccessAnalysis.IsFixableElementAt(invocationInfo, semanticModel, context.CancellationToken))
                            break;

                        context.RegisterRefactoring(
                            "Use [] instead of calling 'ElementAt'",
                            cancellationToken => UseElementAccessInsteadOfEnumerableMethodRefactoring.UseElementAccessInsteadOfElementAtAsync(context.Document, invocation, cancellationToken),
                            RefactoringIdentifiers.UseElementAccessInsteadOfEnumerableMethod);

                        break;
                    }
            }
        }
    }
}
