// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseElementAccessInsteadOfEnumerableMethodRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, InvocationExpressionSyntax invocation)
        {
            ExpressionSyntax expression = invocation.Expression;

            if (expression?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
            {
                ArgumentListSyntax argumentList = invocation.ArgumentList;

                if (argumentList != null)
                {
                    var memberAccess = (MemberAccessExpressionSyntax)expression;

                    switch (memberAccess.Name?.Identifier.ValueText)
                    {
                        case "First":
                            {
                                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                if (argumentList.Arguments.Count == 0
                                    && UseElementAccessInsteadOfFirstRefactoring.CanRefactor(invocation, memberAccess, semanticModel, context.CancellationToken))
                                {
                                    context.RegisterRefactoring(
                                        "Use [] instead of calling 'First'",
                                        cancellationToken => UseElementAccessInsteadOfFirstRefactoring.RefactorAsync(context.Document, invocation, cancellationToken));
                                }

                                break;
                            }
                        case "Last":
                            {
                                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                if (argumentList.Arguments.Count == 0
                                    && UseElementAccessInsteadOfLastRefactoring.CanRefactor(invocation, memberAccess, semanticModel, context.CancellationToken))
                                {
                                    string propertyName = UseElementAccessInsteadOfLastRefactoring.GetCountOrLengthPropertyName(memberAccess.Expression, semanticModel, context.CancellationToken);

                                    if (propertyName != null)
                                    {
                                        context.RegisterRefactoring(
                                            "Use [] instead of calling 'Last'",
                                            cancellationToken => UseElementAccessInsteadOfLastRefactoring.RefactorAsync(context.Document, invocation, propertyName, cancellationToken));
                                    }
                                }

                                break;
                            }
                        case "ElementAt":
                            {
                                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                if (argumentList.Arguments.Count == 1
                                    && UseElementAccessInsteadOfElementAtRefactoring.CanRefactor(invocation, argumentList, memberAccess, semanticModel, context.CancellationToken))
                                {
                                    context.RegisterRefactoring(
                                        "Use [] instead of calling 'ElementAt'",
                                        cancellationToken => UseElementAccessInsteadOfElementAtRefactoring.RefactorAsync(context.Document, invocation, cancellationToken));
                                }

                                break;
                            }
                    }
                }
            }
        }
    }
}
