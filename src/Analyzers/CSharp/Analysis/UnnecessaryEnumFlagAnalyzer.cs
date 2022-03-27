// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
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

            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;
            ITypeSymbol symbol = semanticModel.GetTypeSymbol(bitwiseAnd, cancellationToken);

            if (!symbol.HasAttribute(MetadataNames.System_FlagsAttribute))
                return;

            var enumSymbol = (INamedTypeSymbol)symbol;
            var values = new List<(ExpressionSyntax, ulong)>();

            foreach (ExpressionSyntax expression in bitwiseAnd.AsChain())
            {
                Optional<object> constantValueOpt = semanticModel.GetConstantValue(expression, cancellationToken);

                if (!constantValueOpt.HasValue)
                    continue;

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
}
