// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class SwapParametersRefactoring
    {
        private static ParameterSyntax GetParameterToSwap(CodeRefactoringContext context, ParameterListSyntax parameterList)
        {
            if (parameterList == null)
                throw new ArgumentNullException(nameof(parameterList));

            if (!context.Span.IsEmpty)
            {
                SeparatedSyntaxList<ParameterSyntax>.Enumerator en = parameterList.Parameters.GetEnumerator();

                while (en.MoveNext())
                {
                    if (context.Span.Contains(en.Current.Span))
                    {
                        ParameterSyntax parameter = en.Current;

                        if (en.MoveNext()
                            && context.Span.Contains(en.Current.Span)
                            && (!en.MoveNext() || !context.Span.IntersectsWith(en.Current.Span)))
                        {
                            return parameter;
                        }

                        break;
                    }
                    else if (context.Span.IntersectsWith(en.Current.Span))
                    {
                        break;
                    }
                }
            }

            return null;
        }

        public static void Refactor(CodeRefactoringContext context, ParameterListSyntax parameterList)
        {
            ParameterSyntax parameter = GetParameterToSwap(context, parameterList);

            if (parameter != null)
            {
                context.RegisterRefactoring(
                    "Swap parameters",
                    cancellationToken =>
                    {
                        return RefactorAsync(
                            context.Document,
                            parameter,
                            parameterList,
                            cancellationToken);
                    });
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            ParameterSyntax parameter,
            ParameterListSyntax parameterList,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;

            int index = parameters.IndexOf(parameter);

            ParameterSyntax nextParameter = parameters[index + 1]
                .WithTriviaFrom(parameter);

            parameters = parameters
                .Replace(parameter, nextParameter);

            parameters = parameters
                .Replace(parameters[index + 1], parameter.WithTriviaFrom(nextParameter));

            ParameterListSyntax newNode = parameterList.WithParameters(parameters);

            SyntaxNode newRoot = oldRoot.ReplaceNode(parameterList, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
