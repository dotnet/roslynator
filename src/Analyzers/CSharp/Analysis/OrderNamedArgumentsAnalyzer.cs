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

namespace Roslynator.CSharp.Analysis;

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

        SeparatedSyntaxList<ArgumentSyntax> arguments = ((BaseArgumentListSyntax)context.Node).Arguments;

        if (arguments.Count >= 2)
        {
            (int first, int last) = FindFixableSpan(arguments, context.SemanticModel, context.CancellationToken);

            if (first >= 0
                && last > first)
            {
                DiagnosticHelpers.ReportDiagnostic(
                    context,
                    DiagnosticRules.OrderNamedArguments,
                    Location.Create(
                        context.Node.SyntaxTree,
                        TextSpan.FromBounds(arguments[first].SpanStart, arguments[last].Span.End)));
            }
        }
    }

    internal static (int first, int last) FindFixableSpan(
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

        if (firstIndex >= 0
            && firstIndex < arguments.Count - 1)
        {
            ISymbol symbol = semanticModel.GetSymbol(arguments.First().Parent.Parent, cancellationToken);

            if (symbol is not null)
            {
                ImmutableArray<IParameterSymbol> parameters = symbol.ParametersOrDefault();

                Debug.Assert(!parameters.IsDefault, symbol.Kind.ToString());

                if (!parameters.IsDefault
                    && parameters.Length >= arguments.Count
                    && IsFixable(firstIndex, arguments, parameters))
                {
                    return GetFixableSpan(firstIndex, arguments, parameters);
                }
            }
        }

        return default;
    }

    private static bool IsFixable(
        int firstIndex,
        SeparatedSyntaxList<ArgumentSyntax> arguments,
        ImmutableArray<IParameterSymbol> parameters)
    {
        int j = -1;
        string firstName = arguments[firstIndex].NameColon.Name.Identifier.ValueText;

        for (int i = 0; i < parameters.Length; i++)
        {
            if (parameters[i].Name == firstName)
            {
                j = i;
                break;
            }
        }

        if (j >= 0)
        {
            for (int i = firstIndex + 1; i < arguments.Count; i++)
            {
                string name = arguments[i].NameColon.Name.Identifier.ValueText;

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
        }

        return false;
    }

    private static (int first, int last) GetFixableSpan(
        int firstIndex,
        SeparatedSyntaxList<ArgumentSyntax> arguments,
        ImmutableArray<IParameterSymbol> parameters)
    {
        var sortedArgs = new List<(ArgumentSyntax argument, int ordinal)>();

        for (int i = firstIndex; i < arguments.Count; i++)
        {
            IParameterSymbol parameter = parameters.FirstOrDefault(p => p.Name == arguments[i].NameColon.Name.Identifier.ValueText);

            if (parameter is null)
                return default;

            sortedArgs.Add((arguments[i], parameters.IndexOf(parameter)));
        }

        sortedArgs.Sort((x, y) => x.ordinal.CompareTo(y.ordinal));

        int first = firstIndex;
        int last = arguments.Count - 1;

        while (first < arguments.Count)
        {
            if (sortedArgs[first - firstIndex].argument == arguments[first])
            {
                first++;
            }
            else
            {
                while (last > first)
                {
                    if (sortedArgs[last - firstIndex].argument == arguments[last])
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

        return default;
    }
}
