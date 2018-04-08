// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReorderNamedArgumentsRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            BaseArgumentListSyntax argumentList,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<IParameterSymbol> parameters = semanticModel
                .GetSymbol(argumentList.Parent, cancellationToken)
                .ParametersOrDefault();

            SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

            int firstIndex = ReorderNamedArgumentsAnalyzer.IndexOfFirstFixableParameter(argumentList, arguments, semanticModel, cancellationToken);

            SeparatedSyntaxList<ArgumentSyntax> newArguments = arguments;

            for (int i = firstIndex; i < arguments.Count; i++)
            {
                IParameterSymbol parameter = parameters[i];

                int index = arguments.IndexOf(f => f.NameColon?.Name.Identifier.ValueText == parameter.Name);

                Debug.Assert(index != -1, parameter.Name);

                if (index != -1
                    && index != i)
                {
                    newArguments = newArguments.ReplaceAt(i, arguments[index]);
                }
            }

            BaseArgumentListSyntax newNode = argumentList
                .WithArguments(newArguments)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(argumentList, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
