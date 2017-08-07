// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReorderNamedArgumentsRefactoring
    {
        public static void AnalyzeBaseArgumentList(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.ContainsDiagnostics)
                return;

            var argumentList = (BaseArgumentListSyntax)context.Node;

            SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

            int index = IndexOfFirstFixableParameter(argumentList, arguments, context.SemanticModel, context.CancellationToken);

            if (index != -1)
            {
                TextSpan span = TextSpan.FromBounds(arguments[index].SpanStart, arguments.Last().Span.End);

                if (!argumentList.ContainsDirectives(span))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.ReorderNamedArguments,
                        Location.Create(argumentList.SyntaxTree, span));
                }
            }
        }

        public static int IndexOfFirstFixableParameter(
            BaseArgumentListSyntax argumentList,
            SeparatedSyntaxList<ArgumentSyntax> arguments,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            int firstIndex = -1;

            for (int i = 0; i < arguments.Count; i++)
            {
                if (arguments[i].NameColon != null)
                {
                    firstIndex = i;
                    break;
                }
            }

            if (firstIndex != -1
                && firstIndex != arguments.Count - 1)
            {
                ISymbol symbol = semanticModel.GetSymbol(argumentList.Parent, cancellationToken);

                if (symbol != null)
                {
                    ImmutableArray<IParameterSymbol> parameters = symbol.GetParameters();

                    if (parameters.Length == arguments.Count)
                    {
                        for (int i = firstIndex; i < arguments.Count; i++)
                        {
                            ArgumentSyntax argument = arguments[i];

                            NameColonSyntax nameColon = argument.NameColon;

                            if (nameColon == null)
                                break;

                            if (!string.Equals(
                                nameColon.Name.Identifier.ValueText,
                                parameters[i].Name,
                                StringComparison.Ordinal))
                            {
                                int fixableIndex = i;

                                i++;

                                while (i < arguments.Count)
                                {
                                    if (arguments[i].NameColon == null)
                                        break;

                                    i++;
                                }

                                return fixableIndex;
                            }
                        }
                    }
                }
            }

            return -1;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            BaseArgumentListSyntax argumentList,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<IParameterSymbol> parameters = semanticModel
                .GetSymbol(argumentList.Parent, cancellationToken)
                .GetParameters();

            SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

            int firstIndex = IndexOfFirstFixableParameter(argumentList, arguments, semanticModel, cancellationToken);

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
