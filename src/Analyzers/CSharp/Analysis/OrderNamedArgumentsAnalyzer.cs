// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class OrderNamedArgumentsAnalyzer : BaseDiagnosticAnalyzer
    {
        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                    Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.OrderNamedArguments);

                return _supportedDiagnostics;
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(f => AnalyzeBaseArgumentList(f), SyntaxKind.ArgumentList);
            context.RegisterSyntaxNodeAction(f => AnalyzeBaseArgumentList(f), SyntaxKind.BracketedArgumentList);
        }

        private static void AnalyzeBaseArgumentList(SyntaxNodeAnalysisContext context)
        {
            if (context.Node.ContainsDiagnostics)
                return;

            var argumentList = (BaseArgumentListSyntax)context.Node;

            SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

            (int first, int last)? range = FindFixableSpan(argumentList, arguments, context.SemanticModel, context.CancellationToken);

            if (range is null)
                return;

            TextSpan span = TextSpan.FromBounds(arguments[range.Value.first].SpanStart, arguments[range.Value.last].Span.End);

            if (argumentList.ContainsDirectives(span))
                return;

            DiagnosticHelpers.ReportDiagnostic(
                context,
                DiagnosticRules.OrderNamedArguments,
                Location.Create(context.Node.SyntaxTree, span));
        }

        internal static (int first, int last)? FindFixableSpan(
            BaseArgumentListSyntax argumentList,
            SeparatedSyntaxList<ArgumentSyntax> arguments,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            int firstIndex = -1;

            for (int i = arguments.Count - 1; i >= 0; i--)
            {
                if (arguments[i].NameColon != null)
                {
                    firstIndex = i;
                }
                else
                {
                    break;
                }
            }

            if (firstIndex < 0)
                return null;

            if (firstIndex >= arguments.Count - 1)
                return null;

            ISymbol symbol = semanticModel.GetSymbol(argumentList.Parent, cancellationToken);

            if (symbol is null)
                return null;

            ImmutableArray<IParameterSymbol> parameters = symbol.ParametersOrDefault();

            Debug.Assert(!parameters.IsDefault, symbol.Kind.ToString());

            if (parameters.IsDefault)
                return null;

            if (parameters.Length < arguments.Count)
                return null;

            if (!IsFixable(firstIndex, arguments, parameters))
                return null;

            var items = new List<(ArgumentSyntax, int)>();

            for (int i = firstIndex; i < arguments.Count; i++)
            {
                IParameterSymbol parameter = parameters.FirstOrDefault(g => g.Name == arguments[i].NameColon.Name.Identifier.ValueText);

                if (parameter is null)
                {
                    return null;
                }

                items.Add((arguments[i], parameters.IndexOf(parameter)));
            }

            items.Sort((x, y) => x.Item2.CompareTo(y.Item2));

            int first = firstIndex;
            int last = arguments.Count - 1;

            while (first < arguments.Count)
            {
                if (items[first - firstIndex].Item1 == arguments[first])
                {
                    first++;
                }
                else
                {
                    while (last > first)
                    {
                        if (items[last - firstIndex].Item1 == arguments[last])
                        {
                            last--;
                        }
                        else
                        {
                            return (first, last);
                        }
                    }
                }
            }

            return null;
        }

        private static bool IsFixable(int firstIndex, SeparatedSyntaxList<ArgumentSyntax> arguments, ImmutableArray<IParameterSymbol> parameters)
        {
            int j = -1;
            string name = arguments[firstIndex].NameColon.Name.Identifier.ValueText;

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].Name == name)
                {
                    j = i;
                    break;
                }
            }

            if (j == -1)
                return false;

            for (int i = firstIndex + 1; i < arguments.Count; i++)
            {
                name = arguments[i].NameColon.Name.Identifier.ValueText;

                while (!string.Equals(
                    name,
                    parameters[j].Name,
                    StringComparison.Ordinal))
                {
                    j++;

                    if (j == parameters.Length)
                    {
                        foreach (IParameterSymbol parameter in parameters)
                        {
                            if (parameter.Name == name)
                                return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
