// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnnecessaryEnumFlagAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.UnnecessaryEnumFlag);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(c => AnalyzeBitwiseOrExpression(c), SyntaxKind.BitwiseOrExpression);
    }

    private static void AnalyzeBitwiseOrExpression(SyntaxNodeAnalysisContext context)
    {
        var bitwiseAnd = (BinaryExpressionSyntax)context.Node;

        foreach (ExpressionSyntax expression in bitwiseAnd.AsChain())
        {
            if (!expression.IsKind(SyntaxKind.SimpleMemberAccessExpression))
                return;
        }

        ITypeSymbol symbol = context.SemanticModel.GetTypeSymbol(bitwiseAnd, context.CancellationToken);

        if (!symbol.HasAttribute(MetadataNames.System_FlagsAttribute))
            return;

        var enumSymbol = (INamedTypeSymbol)symbol;
        var values = new List<(ExpressionSyntax, ulong)>();

        foreach (ExpressionSyntax expression in bitwiseAnd.AsChain())
        {
            Optional<object> constantValueOpt = context.SemanticModel.GetConstantValue(expression, context.CancellationToken);

            if (constantValueOpt.HasValue)
            {
                ulong value = SymbolUtility.GetEnumValueAsUInt64(constantValueOpt.Value, enumSymbol);
                var addToValues = true;

                for (int i = values.Count - 1; i >= 0; i--)
                {
                    (ExpressionSyntax expression2, ulong value2) = values[i];

                    if ((value & value2) != 0)
                    {
                        if (value <= value2)
                        {
                            ReportDiagnostic(expression, expression2);
                            addToValues = false;
                        }
                        else
                        {
                            ReportDiagnostic(expression2, expression);
                            values.RemoveAt(i);
                        }
                    }
                }

                if (addToValues)
                    values.Add((expression, value));
            }
        }

        void ReportDiagnostic(ExpressionSyntax expression, ExpressionSyntax expression2)
        {
            context.ReportDiagnostic(
                DiagnosticRules.UnnecessaryEnumFlag,
                expression,
                context.SemanticModel.GetSymbol(expression, context.CancellationToken).Name,
                context.SemanticModel.GetSymbol(expression2, context.CancellationToken).Name);
        }
    }
}
