// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class DuplicateParameterRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, ParameterListSyntax parameterList)
        {
            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.DuplicateParameter))
            {
                ParameterSyntax parameter = GetParameter(context, parameterList);

                if (parameter != null)
                {
                    context.RegisterRefactoring(
                        "Duplicate parameter",
                        cancellationToken => RefactorAsync(context.Document, parameter, cancellationToken));
                }
            }
        }

        private static ParameterSyntax GetParameter(RefactoringContext context, ParameterListSyntax parameterList)
        {
            if (context.Span.IsEmpty)
            {
                SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;

                foreach (ParameterSyntax parameter in parameters)
                {
                    if (parameter.IsMissing
                        && context.Span.Contains(parameter.Span))
                    {
                        int index = parameters.IndexOf(parameter);

                        if (index > 0
                            && !parameters[index - 1].IsMissing)
                        {
                            return parameter;
                        }
                    }
                }
            }

            return null;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ParameterSyntax parameter,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var parameterList = (ParameterListSyntax)parameter.Parent;

            int index = parameterList.Parameters.IndexOf(parameter);

            ParameterSyntax previousParameter = parameterList.Parameters[index - 1]
                .WithTriviaFrom(parameter);

            SyntaxNode newRoot = oldRoot.ReplaceNode(parameter, previousParameter);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
